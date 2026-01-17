using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HWKit
{
    public class CoreTempCpuProvider : HardwareInfoProviderBase
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr MapViewOfFile(IntPtr handle, uint dwDesiredAccess,uint offsetHigh, uint offsetLow,int size );
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnmapViewOfFile(IntPtr pointer);

        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr handle);
        const uint FILE_MAP_READ = 0x04;
       
        [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi,Pack =1)]
        private struct CoreTempSharedDataEx
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] uiLoad;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public uint[] uiTjMax;
            public uint uiCoreCnt;
            public uint uiCPUCnt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public float[] fTemp;
            public float fVID;
            public float fCPUSpeed;
            public float fFSBSpeed;
            public float fMultiplier;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sCPUName;
            public byte ucFahrenheit;
            public byte ucDeltaToTjMax;
            // uiStructVersion = 2
            public byte ucTdpSupported;
            public byte ucPowerSupported;
            public uint uiStructVersion;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public uint[] uiTdp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public float[] fPower;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public float[] fMultipliers;
        }
        private interface IAccessor
        {
            float Value { get; }
        }
        private sealed class CoreLoadAccessor : IAccessor
        {
            IntPtr _ptr;
            int _index;
            public CoreLoadAccessor(IntPtr basePtr, int index)
            {
                _ptr = basePtr;
                _index = index;
            }
            public float Value { 
                get {
                    var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_ptr);
                    return data.uiLoad[_index];
                } 
            }
        }
        private sealed class CoreTemperatureAccessor : IAccessor
        {
            IntPtr _ptr;
            int _index;
            public CoreTemperatureAccessor(IntPtr basePtr, int index)
            {
                _ptr = basePtr;
                _index = index;
            }
            public float Value
            {
                get
                {
                    var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_ptr);
                    if(data.ucFahrenheit>0)
                    {
                        return (float)((data.fTemp[_index] - 32) / 1.8);
                    }
                    return data.fTemp[_index];
                }
            }
        }
        private sealed class CoreFrequencyAccessor : IAccessor
        {
            IntPtr _ptr;
            int _index;
            public CoreFrequencyAccessor(IntPtr basePtr, int index)
            {
                _ptr = basePtr;
                _index = index;
            }
            public float Value
            {
                get
                {
                    var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_ptr);
                    return data.fMultipliers[_index]*data.fFSBSpeed;
                }
            }
        }
        private sealed class CoreMultiplierAccessor : IAccessor
        {
            IntPtr _ptr;
            int _index;
            public CoreMultiplierAccessor(IntPtr basePtr, int index)
            {
                _ptr = basePtr;
                _index = index;
            }
            public float Value
            {
                get
                {
                    var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_ptr);
                    return data.fMultipliers[_index];
                }
            }
        }
      
        IntPtr _sharedHandle = IntPtr.Zero;
        IntPtr _sharedPtr = IntPtr.Zero;
        List<IAccessor> accessors = new List<IAccessor>();
        protected override void OnStart()
        {
            if (_sharedHandle!=IntPtr.Zero)
            {
                OnStop();
            }
            var handle = OpenFileMapping(FILE_MAP_READ, true, "CoreTempMappingObjectEx");
            if(handle!=IntPtr.Zero)
            {
                System.Diagnostics.Debug.Assert(4740 == Marshal.SizeOf<CoreTempSharedDataEx>(),"Something is wrong with your struct");
                _sharedPtr = MapViewOfFile(handle,FILE_MAP_READ,0,0,Marshal.SizeOf<CoreTempSharedDataEx>());
            }
            if(_sharedPtr==IntPtr.Zero)
            {
                CloseHandle(handle);
                throw new SystemException("Unable to map view");
            }
            var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_sharedPtr);
            Publish($"/cpu/frequency", new Func<float>(() => {
                var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_sharedPtr);
                return data.fCPUSpeed;
            }));
            Publish($"/bus/frequency", new Func<float>(() => {
                var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_sharedPtr);
                return data.fFSBSpeed;
            }));
            Publish($"/cpu/multiplier", new Func<float>(() => {
                var data = Marshal.PtrToStructure<CoreTempSharedDataEx>(_sharedPtr);
                return data.fMultiplier;
            }));
            int coreIndex = 0;
            for (var i = 0; i < data.uiCPUCnt; i++) {
                for (int j = 0; j < data.uiCoreCnt; j++)
                {
                    var loadAcc = new CoreLoadAccessor(_sharedPtr, coreIndex);
                    Publish($"/cpu/{i}/core/{j}/load", new Func<float>(() => {
                        return loadAcc.Value;
                    }));
                    var multAcc = new CoreMultiplierAccessor(_sharedPtr, coreIndex);
                    Publish($"/cpu/{i}/core/{j}/multiplier", new Func<float>(() => {
                        return multAcc.Value;
                    }));
                    var tempAcc = new CoreTemperatureAccessor(_sharedPtr, coreIndex);
                    Publish($"/cpu/{i}/core/{j}/temperature", new Func<float>(() => {
                        return tempAcc.Value;
                    }));
                    var freqAcc = new CoreFrequencyAccessor(_sharedPtr, coreIndex);
                    Publish($"/cpu/{i}/core/{j}/frequency", new Func<float>(() => {
                        return freqAcc.Value;
                    }));
                    ++coreIndex;
                }
            }
            
            //for (var i = 0; i < _entries.Length; i++)
            //{
            //    CpuEntry entry = _entries[i];
            //    Publish($"/cpu/{i}/frequency", new Func<float>(() => entry.Frequency));
            //    Publish($"/cpu/{i}/load", new Func<float>(() => entry.Load));
            //}
            //_timer = new Timer(TimerCB, null, _refreshInterval, _refreshInterval);

        }
        protected override void OnStop()
        {
            if (_sharedHandle==IntPtr.Zero)
            {
                return;
            }
            accessors.Clear();
            UnmapViewOfFile(_sharedPtr);
            _sharedPtr = IntPtr.Zero;
            CloseHandle(_sharedHandle);
            _sharedHandle = IntPtr.Zero;
        }
    }
}
