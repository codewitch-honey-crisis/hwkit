using Ati.Adl;

using HWKit;

using Nvidia.Nvml;

using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // DoNvidiaGpus();
            // DoAmdGpus();
            // DoCpus();
            
            var hardware = new Dictionary<string, Func<float>>();
            var cpuProvider = new CoreTempCpuProvider(); // new WmiCpuProvider(); 
            cpuProvider.PublishReading += (sender, args) => { hardware[args.Path] = args.Getter; };
            cpuProvider.RefreshInterval = 1000;
            try
            {
                cpuProvider.Start();
            }
            catch { }

            var nvidiaGpuProvider = new NvidiaNvmlGpuProvider();
            nvidiaGpuProvider.PublishReading += (sender, args) => { hardware[args.Path] = args.Getter; };
            nvidiaGpuProvider.RefreshInterval = 1000;
            try
            {
                nvidiaGpuProvider.Start();
            }
            catch { }

            var amdGpuProvider = new AmdAdlGpuProvider();
            amdGpuProvider.PublishReading += (sender, args) => { hardware[args.Path] = args.Getter; };
            amdGpuProvider.RefreshInterval = 1000;
            //try
            //{
                amdGpuProvider.Start();
            //}
            //catch { }

            while (!Console.KeyAvailable)
            {
                foreach (var kvp in hardware)
                {
                    Console.WriteLine($"{kvp.Key} => {kvp.Value()}");
                }
                Console.Out.Flush();
                Thread.Sleep(cpuProvider.RefreshInterval==0?1000:(int)cpuProvider.RefreshInterval);
                Console.Clear();

            }
        }

    }
}
