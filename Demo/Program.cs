using System;
using System.Text;
using Nvidia.Nvml;

namespace Demo
{
    class Program
    {
        private static string GetStringFromSByteArray(sbyte[] data)
        {
            if (data == null)
                throw new SystemException("Data can't be null");
            
            byte[] busIdData = Array.ConvertAll(data, (a) => (byte)a);
            return Encoding.Default.GetString(busIdData).Replace("\0", "");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Initialiling nvml library..");
            try
            {
                NvGpu.NvmlInitV2();
                Console.WriteLine("Retrieving device count in this machine ...");
                var deviceCount = NvGpu.NvmlDeviceGetCountV2();
                Console.WriteLine("Listing devices ...");
                for (int i = 0; i < deviceCount; i++)
                {
                    var device = NvGpu.NvmlDeviceGetHandleByIndex((uint)i);
                    if (device == IntPtr.Zero)
                    {
                        throw new SystemException($"Something got wrong retrieving device {i}. Do you have a nvidia card?");
                    }

                    var deviceName = NvGpu.NvmlDeviceGetName(device);
                    Console.WriteLine($"The device name from index {i} is {deviceName}");
                    var info = NvGpu.NvmlDeviceGetPciInfoV3(device);
                    var busId = GetStringFromSByteArray(info.busId);
                    Console.WriteLine($"BusId is {busId}");
                    var temp = NvGpu.NvmlDeviceGetTemperature(device, NvmlTemperatureSensor.NVML_TEMPERATURE_GPU);
                    Console.WriteLine($"Temp is {temp} degrees");
                    var fanSpeed = NvGpu.NvmlDeviceGetFanSpeed(device,0);
                    Console.WriteLine($"Fan speed is {fanSpeed}%");
                    var rates = NvGpu.NvmlDeviceGetUtilizationRates(device);
                    Console.WriteLine("Rates: ");
                    Console.WriteLine("  GPU Utilization: {0}", rates.gpuUtilization);
                    Console.WriteLine("  VRAM Utilization: {0}", rates.memoryUtilization);
                }

                NvGpu.NvmlShutdown();
            }
            catch (SystemException se)
            {
                Console.WriteLine(se.ToString());
                NvGpu.NvmlShutdown();
            }

        }
    }
}
