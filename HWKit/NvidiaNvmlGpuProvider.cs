using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nvidia.Nvml;
namespace HWKit
{
    public class NvidiaNvmlGpuProvider : HardwareInfoProviderBase
    {
        private sealed class DeviceAccessor
        {
            IntPtr _handle;
            public DeviceAccessor(IntPtr handle)
            {
                _handle = handle;
            }
            public float Temperature => NvGpu.NvmlDeviceGetTemperature(_handle, NvmlTemperatureSensor.NVML_TEMPERATURE_GPU);
            public float Load => NvGpu.NvmlDeviceGetUtilizationRates(_handle).gpuUtilization;
            public float Frequency => NvGpu.NvmlDeviceGetClock(_handle, NvmlClockType.NVML_CLOCK_GRAPHICS, NvmlClockId.NVML_CLOCK_ID_CURRENT);
            public float VramLoad => NvGpu.NvmlDeviceGetUtilizationRates(_handle).memoryUtilization;
            public float VramFrequency => NvGpu.NvmlDeviceGetClock(_handle, NvmlClockType.NVML_CLOCK_MEM, NvmlClockId.NVML_CLOCK_ID_CURRENT);
            public float SMFrequency => NvGpu.NvmlDeviceGetClock(_handle, NvmlClockType.NVML_CLOCK_SM, NvmlClockId.NVML_CLOCK_ID_CURRENT);
            public float FanLoad => NvGpu.NvmlDeviceGetFanSpeed(_handle);
        }
        private sealed class DeviceFanAccessor
        {
            IntPtr _handle;
            int _index;
            public DeviceFanAccessor(IntPtr handle, int index)
            {
                _handle = handle;
                _index = index;
            }
            public float Load => NvGpu.NvmlDeviceGetFanSpeed(_handle,0);
            public float Speed => NvGpu.NvmlDeviceGetFanSpeedRpm(_handle, 0);
        }
        private bool _started;
        protected override void OnStart()
        {
            if(_started)
            {
                Stop();
            }
            NvGpu.NvmlInitV2();
            var deviceCount = (int)NvGpu.NvmlDeviceGetCountV2();

            for (var i = 0; i < deviceCount; i++)
            {
                var handle = NvGpu.NvmlDeviceGetHandleByIndex((uint)i);
                var accessor = new DeviceAccessor(handle);
                Publish($"/gpu/{i}/temperature", new Func<float>(() => accessor.Temperature));
                Publish($"/gpu/{i}/load", new Func<float>(() => accessor.Load));
                Publish($"/gpu/{i}/frequency", new Func<float>(() => accessor.Frequency));
                Publish($"/gpu/{i}/load/vram", new Func<float>(() => accessor.VramLoad));
                Publish($"/gpu/{i}/frequency/vram", new Func<float>(() => accessor.VramFrequency));
                Publish($"/gpu/{i}/frequency/sm", new Func<float>(() => accessor.SMFrequency));
                Publish($"/gpu/{i}/load/fan", new Func<float>(() => accessor.Load));
                var fanCount = NvGpu.NvmlGetFanCount(handle);
                for(var j = 0 ; j < fanCount; j++)
                {
                    var fanAccessor = new DeviceFanAccessor(handle,j);
                    Publish($"/gpu/{i}/fan/{j}/load", new Func<float>(() => fanAccessor.Load));
                    Publish($"/gpu/{i}/fan/{j}/speed", new Func<float>(() => fanAccessor.Speed));
                }
                
            }

        }
        protected override void OnStop()
        {
            if (!_started)
            {
                return;
            }
            _started = false; // even on error this should go false.
            NvGpu.NvmlShutdown();
            
        }
    }
}
