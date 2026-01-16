using System;
using System.Collections.Generic;
using System.Management;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
namespace HWKit
{
    public class WmiCpuProvider : HardwareInfoProviderBase
    {
        private sealed class CpuCoreEntry {
            public int CpuIndex;
            public int ThreadIndex;
            public float Frequency;
            public float Load;
            public CpuCoreEntry(int cpuIndex, int coreIndex, float frequency, float load)
            {
                CpuIndex = cpuIndex;
                ThreadIndex = coreIndex;
                Frequency = frequency;
                Load = load;
            }
        }
        uint _refreshInterval = 100;
        Timer _timer;
        CpuCoreEntry[] _entries = null;
        void TimerCB(object? state)
        {
            RunQuery();
        }
        void RunQuery()
        {

            
            var coreCls = new ManagementClass("Win32_PerfFormattedData_Counters_ProcessorInformation");

            var coreCol = coreCls.GetInstances();
            if(coreCol.Count>0)
            {
                var list = new List<CpuCoreEntry>();
                foreach (var procObj in coreCol)
                {
                    var name = (string)procObj["Name"];
                    if (name.Contains("_Total"))
                    {
                        continue;
                    }
                    var tmp = name.Split(",");
                    if (tmp.Length != 2) continue;
                    var cpuIdx = 0;
                    if (!int.TryParse(tmp[0], out cpuIdx)) cpuIdx = 0;
                    var threadIdx = 0;
                    if (!int.TryParse(tmp[1], out threadIdx)) threadIdx = 0;
                    var freq = Convert.ToSingle(procObj["ActualFrequency"]);
                    var load = Convert.ToSingle(procObj["PercentProcessorTime"]);
                    CpuCoreEntry entry = new CpuCoreEntry(cpuIdx, threadIdx, freq, load);
                    list.Add(entry);
                }
                list.Sort((x, y) => { if (x.CpuIndex != y.CpuIndex) return x.CpuIndex.CompareTo(y.CpuIndex); return x.ThreadIndex.CompareTo(y.ThreadIndex); });
                if (_entries==null||_entries.Length!=list.Count)
                {
                    _entries = new CpuCoreEntry[list.Count];
                }
                
                for(var i = 0;i<list.Count;++i)
                {
                    _entries[i] = list[i];
                }
            } else
            {
                _entries = Array.Empty<CpuCoreEntry>();
            }
                
        }
        protected override uint GetRefreshInterval()
        {
            return _refreshInterval;
        }
        protected override void SetRefreshInterval(uint value)
        {
            _refreshInterval = value;
            if (_timer != null)
            {
                _timer.Change(0, _refreshInterval);
            }
        }
        protected override void OnStart()
        {
            if(_timer!=null )
            {
                OnStop();
            }
            RunQuery();
            for (var i = 0; i < _entries.Length; i++)
            {
                CpuCoreEntry entry = _entries[i];
                Publish($"/cpu/{entry.CpuIndex}/thread/{entry.ThreadIndex}/frequency", new Func<float>(() => entry.Frequency));
                Publish($"/cpu/{entry.CpuIndex}/thread/{entry.ThreadIndex}/load",new Func<float>(() => entry.Load));
            }
            _timer = new Timer(TimerCB,null,_refreshInterval,_refreshInterval);

        }
        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
        
}
