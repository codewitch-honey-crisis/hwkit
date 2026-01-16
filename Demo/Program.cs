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
        enum ADL_PMLOG_SENSORS
        {
            ADL_SENSOR_MAXTYPES = 0,
            ADL_PMLOG_CLK_GFXCLK = 1,
            ADL_PMLOG_CLK_MEMCLK = 2,
            ADL_PMLOG_CLK_SOCCLK = 3,
            ADL_PMLOG_CLK_UVDCLK1 = 4,
            ADL_PMLOG_CLK_UVDCLK2 = 5,
            ADL_PMLOG_CLK_VCECLK = 6,
            ADL_PMLOG_CLK_VCNCLK = 7,
            ADL_PMLOG_TEMPERATURE_EDGE = 8,
            ADL_PMLOG_TEMPERATURE_MEM = 9,
            ADL_PMLOG_TEMPERATURE_VRVDDC = 10,
            ADL_PMLOG_TEMPERATURE_VRMVDD = 11,
            ADL_PMLOG_TEMPERATURE_LIQUID = 12,
            ADL_PMLOG_TEMPERATURE_PLX = 13,
            ADL_PMLOG_FAN_RPM = 14,
            ADL_PMLOG_FAN_PERCENTAGE = 15,
            ADL_PMLOG_SOC_VOLTAGE = 16,
            ADL_PMLOG_SOC_POWER = 17,
            ADL_PMLOG_SOC_CURRENT = 18,
            ADL_PMLOG_INFO_ACTIVITY_GFX = 19,
            ADL_PMLOG_INFO_ACTIVITY_MEM = 20,
            ADL_PMLOG_GFX_VOLTAGE = 21,
            ADL_PMLOG_MEM_VOLTAGE = 22,
            ADL_PMLOG_ASIC_POWER = 23,
            ADL_PMLOG_TEMPERATURE_VRSOC = 24,
            ADL_PMLOG_TEMPERATURE_VRMVDD0 = 25,
            ADL_PMLOG_TEMPERATURE_VRMVDD1 = 26,
            ADL_PMLOG_TEMPERATURE_HOTSPOT = 27,
            ADL_PMLOG_TEMPERATURE_GFX = 28,
            ADL_PMLOG_TEMPERATURE_SOC = 29,
            ADL_PMLOG_GFX_POWER = 30,
            ADL_PMLOG_GFX_CURRENT = 31,
            ADL_PMLOG_TEMPERATURE_CPU = 32,
            ADL_PMLOG_CPU_POWER = 33,
            ADL_PMLOG_CLK_CPUCLK = 34,
            ADL_PMLOG_THROTTLER_STATUS = 35,   // GFX
            ADL_PMLOG_CLK_VCN1CLK1 = 36,
            ADL_PMLOG_CLK_VCN1CLK2 = 37,
            ADL_PMLOG_SMART_POWERSHIFT_CPU = 38,
            ADL_PMLOG_SMART_POWERSHIFT_DGPU = 39,
            ADL_PMLOG_BUS_SPEED = 40,
            ADL_PMLOG_BUS_LANES = 41,
            ADL_PMLOG_TEMPERATURE_LIQUID0 = 42,
            ADL_PMLOG_TEMPERATURE_LIQUID1 = 43,
            ADL_PMLOG_CLK_FCLK = 44,
            ADL_PMLOG_THROTTLER_STATUS_CPU = 45,
            ADL_PMLOG_SSPAIRED_ASICPOWER = 46, // apuPower
            ADL_PMLOG_SSTOTAL_POWERLIMIT = 47, // Total Power limit
            ADL_PMLOG_SSAPU_POWERLIMIT = 48, // APU Power limit
            ADL_PMLOG_SSDGPU_POWERLIMIT = 49, // DGPU Power limit
            ADL_PMLOG_TEMPERATURE_HOTSPOT_GCD = 50,
            ADL_PMLOG_TEMPERATURE_HOTSPOT_MCD = 51,
            ADL_PMLOG_THROTTLE_PERCENTAGE_TEMP_GFX = 52,
            ADL_PMLOG_THROTTLE_PERCENTAGE_TEMP_MEM = 53,
            ADL_PMLOG_THROTTLE_PERCENTAGE_TEMP_VR = 54,
            ADL_PMLOG_THROTTLE_PERCENTAGE_POWER = 55,
            ADL_PMLOG_THROTTLE_PERCENTAGE_TDC = 56,
            ADL_PMLOG_THROTTLE_PERCENTAGE_VMAX = 57,
            ADL_PMLOG_BUS_CURR_MAX_SPEED = 58,
            ADL_PMLOG_RESERVED_1 = 59, //Currently Unused
            ADL_PMLOG_RESERVED_2 = 60, //Currently Unused
            ADL_PMLOG_RESERVED_3 = 61, //Currently Unused
            ADL_PMLOG_RESERVED_4 = 62, //Currently Unused
            ADL_PMLOG_RESERVED_5 = 63, //Currently Unused
            ADL_PMLOG_RESERVED_6 = 64, //Currently Unused
            ADL_PMLOG_RESERVED_7 = 65, //Currently Unused
            ADL_PMLOG_RESERVED_8 = 66, //Currently Unused
            ADL_PMLOG_RESERVED_9 = 67, //Currently Unused
            ADL_PMLOG_RESERVED_10 = 68, //Currently Unused
            ADL_PMLOG_RESERVED_11 = 69, //Currently Unused
            ADL_PMLOG_RESERVED_12 = 70, //Currently Unused
            ADL_PMLOG_CLK_NPUCLK = 71,      //NPU frequency
            ADL_PMLOG_NPU_BUSY_AVG = 72,    //NPU busy
            ADL_PMLOG_BOARD_POWER = 73,
            ADL_PMLOG_TEMPERATURE_INTAKE = 74,
            ADL_PMLOG_MAX_SENSORS_REAL
        }
        private static string SensorToString(uint sensor)
        {
            switch ((ADL_PMLOG_SENSORS)sensor)
            {
                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_GFXCLK:
                    return "Graphics Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_MEMCLK:
                    return "Memory Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_SOCCLK:
                    return "SOC Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_UVDCLK1:
                    return "UVD1 Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_UVDCLK2:
                    return "UVD2 Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_VCECLK:
                    return "VCE Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_VCNCLK:
                    return "VCN Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_EDGE:
                    return "EDGE Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_MEM:
                    return "Memory Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_VRVDDC:
                    return "VDDC VR Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_VRSOC:
                    return "SOC VR Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_VRMVDD:
                    return "MVDD VR Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_VRMVDD0:
                    return "MVDD0 VR Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_VRMVDD1:
                    return "MVDD1 VR Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_LIQUID:
                    return "Liquid Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_PLX:
                    return "PLX Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_FAN_RPM:
                    return "Fan RPM";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_FAN_PERCENTAGE:
                    return "Fan Percentage";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SOC_VOLTAGE:
                    return "SOC Voltage";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SOC_POWER:
                    return "SOC Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SOC_CURRENT:
                    return "SOC Current";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_INFO_ACTIVITY_GFX:
                    return "GFX Activity";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_INFO_ACTIVITY_MEM:
                    return "MEM Activity";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_GFX_VOLTAGE:
                    return "GFX Voltage";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_MEM_VOLTAGE:
                    return "MEM Voltage";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_ASIC_POWER:
                    return "Asic Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_HOTSPOT:
                    return "HOTSPOT Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_GFX:
                    return "GFX Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_SOC:
                    return "SOC Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_TEMPERATURE_CPU:
                    return "CPU Temperature";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_GFX_POWER:
                    return "GFX Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_GFX_CURRENT:
                    return "GFX Current";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CPU_POWER:
                    return "CPU Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_CLK_CPUCLK:
                    return "CPU Clock";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_THROTTLER_STATUS:
                    return "Throttler Status";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SMART_POWERSHIFT_CPU:
                    return "Powershift CPU";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SMART_POWERSHIFT_DGPU:
                    return "Powershift DGPU";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_BUS_SPEED:
                    return "Bus Speed";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_BUS_LANES:
                    return "Bus Lanes";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_BOARD_POWER:
                    return "Total Board Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SSPAIRED_ASICPOWER:
                    return "APU Power";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SSTOTAL_POWERLIMIT:
                    return "Total Power limit";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SSAPU_POWERLIMIT:
                    return "APU Power limit";

                case ADL_PMLOG_SENSORS.ADL_PMLOG_SSDGPU_POWERLIMIT:
                    return "DGPU Power limit";


                default:
                    return null;

            }

        }

        static bool DoAmdGpus()
        {
            int ADLRet = -1;
            int NumberOfAdapters = 0;
            int NumberOfDisplays = 0;

            if (null != ADL.ADL_Main_Control_Create)
            {
                // Second parameter is 1: Get only the present adapters
                ADLRet = ADL.ADL_Main_Control_Create(ADL.ADL_Main_Memory_Alloc, 1);
            }
            if (ADL.ADL_SUCCESS == ADLRet)
            {
                if (null != ADL.ADL_Adapter_NumberOfAdapters_Get)
                {
                    ADL.ADL_Adapter_NumberOfAdapters_Get(ref NumberOfAdapters);
                }
                Console.WriteLine("Number Of Adapters: " + NumberOfAdapters.ToString() + "\n");

                if (0 < NumberOfAdapters)
                {
                    // Get OS adpater info from ADL
                    ADLAdapterInfoArray OSAdapterInfoData;
                    OSAdapterInfoData = new ADLAdapterInfoArray();

                    if (null != ADL.ADL_Adapter_AdapterInfo_Get)
                    {
                        IntPtr AdapterBuffer = IntPtr.Zero;
                        int size = Marshal.SizeOf(OSAdapterInfoData);
                        AdapterBuffer = Marshal.AllocCoTaskMem((int)size);
                        Marshal.StructureToPtr(OSAdapterInfoData, AdapterBuffer, false);

                        if (null != ADL.ADL_Adapter_AdapterInfo_Get)
                        {
                            ADLRet = ADL.ADL_Adapter_AdapterInfo_Get(AdapterBuffer, size);
                            if (ADL.ADL_SUCCESS == ADLRet)
                            {
                                OSAdapterInfoData = (ADLAdapterInfoArray)Marshal.PtrToStructure(AdapterBuffer, OSAdapterInfoData.GetType());
                                int IsActive = 0;

                                for (int i = 0; i < NumberOfAdapters; i++)
                                {
                                    ADLPMLogStartInput logStartInput = default;
                                    ADLPMLogStartOutput logStartOutput = default;
                                    ADLPMLogData logData = default;
                                    uint hDevice = 0;
                                    ADLPMLogSupportInfo support = default;
                                    ADLRet = -1;
                                    if (null != ADL.ADL2_Device_PMLog_Device_Create)
                                    {
                                        ADLRet = ADL.ADL2_Device_PMLog_Device_Create(IntPtr.Zero, OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, out hDevice);
                                    }
                                    if (ADL.ADL_SUCCESS == ADLRet)
                                    {
                                        ADLRet = -1;

                                        if (null != ADL.ADL2_Adapter_PMLog_Support_Get)
                                        {
                                            ADLRet = ADL.ADL2_Adapter_PMLog_Support_Get(IntPtr.Zero, OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, out support);
                                        }
                                        if (ADL.ADL_SUCCESS == ADLRet)
                                        {
                                            int j = 0;
                                            while (support.usSensors[j] != 0)
                                            {
                                                logStartInput.usSensors[j] = support.usSensors[j];
                                                j++;
                                            }
                                            logStartInput.usSensors[j] = 0;
                                            logStartInput.ulSampleRate = 100;
                                            ADLRet = -1;
                                            if (null != ADL.ADL2_Adapter_PMLog_Support_Start)
                                            {
                                                ADLRet = ADL.ADL2_Adapter_PMLog_Support_Start(IntPtr.Zero, OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref logStartInput, out logStartOutput, hDevice);
                                            }
                                            if (ADL.ADL_SUCCESS == ADLRet)
                                            {
                                                logData = Marshal.PtrToStructure<ADLPMLogData>(logStartOutput.pLoggingAddress);

                                                for (j = 0; j < logData.ulValues.Length; j += 2)
                                                {
                                                    if (logData.ulValues[j] == 0) break;
                                                    Console.WriteLine("    {0} = {1}", SensorToString(logData.ulValues[j]), logData.ulValues[j + 1]);
                                                }

                                                ADLRet = -1;

                                                // Check if the adapter is active
                                                if (null != ADL.ADL_Adapter_Active_Get)
                                                    ADLRet = ADL.ADL_Adapter_Active_Get(OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref IsActive);

                                                if (ADL.ADL_SUCCESS == ADLRet)
                                                {
                                                    Console.WriteLine("Adapter is   : " + (0 == IsActive ? "DISABLED" : "ENABLED"));
                                                    Console.WriteLine("Adapter Index: " + OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex.ToString());
                                                    Console.WriteLine("Adapter UDID : " + OSAdapterInfoData.ADLAdapterInfo[i].UDID);
                                                    Console.WriteLine("Bus No       : " + OSAdapterInfoData.ADLAdapterInfo[i].BusNumber.ToString());
                                                    Console.WriteLine("Driver No    : " + OSAdapterInfoData.ADLAdapterInfo[i].DriverNumber.ToString());
                                                    Console.WriteLine("Function No  : " + OSAdapterInfoData.ADLAdapterInfo[i].FunctionNumber.ToString());
                                                    Console.WriteLine("Vendor ID    : " + OSAdapterInfoData.ADLAdapterInfo[i].VendorID.ToString());
                                                    Console.WriteLine("Adapter Name : " + OSAdapterInfoData.ADLAdapterInfo[i].AdapterName);
                                                    Console.WriteLine("Display Name : " + OSAdapterInfoData.ADLAdapterInfo[i].DisplayName);
                                                    Console.WriteLine("Present      : " + (0 == OSAdapterInfoData.ADLAdapterInfo[i].Present ? "No" : "Yes"));
                                                    Console.WriteLine("Exist        : " + (0 == OSAdapterInfoData.ADLAdapterInfo[i].Exist ? "No" : "Yes"));
                                                    Console.WriteLine("Driver Path  : " + OSAdapterInfoData.ADLAdapterInfo[i].DriverPath);
                                                    Console.WriteLine("Driver Path X: " + OSAdapterInfoData.ADLAdapterInfo[i].DriverPathExt);
                                                    Console.WriteLine("PNP String   : " + OSAdapterInfoData.ADLAdapterInfo[i].PNPString);

                                                    // Obtain information about displays
                                                    ADLDisplayInfo oneDisplayInfo = new ADLDisplayInfo();

                                                    if (null != ADL.ADL_Display_DisplayInfo_Get)
                                                    {
                                                        IntPtr DisplayBuffer = IntPtr.Zero;
                                                        j = 0;

                                                        // Force the display detection and get the Display Info. Use 0 as last parameter to NOT force detection
                                                        ADLRet = ADL.ADL_Display_DisplayInfo_Get(OSAdapterInfoData.ADLAdapterInfo[i].AdapterIndex, ref NumberOfDisplays, out DisplayBuffer, 1);
                                                        if (ADL.ADL_SUCCESS == ADLRet)
                                                        {
                                                            List<ADLDisplayInfo> DisplayInfoData = new List<ADLDisplayInfo>();
                                                            for (j = 0; j < NumberOfDisplays; j++)
                                                            {
                                                                oneDisplayInfo = (ADLDisplayInfo)Marshal.PtrToStructure(new IntPtr(DisplayBuffer.ToInt64() + j * Marshal.SizeOf(oneDisplayInfo)), oneDisplayInfo.GetType());
                                                                DisplayInfoData.Add(oneDisplayInfo);
                                                            }
                                                            Console.WriteLine("\nTotal Number of Displays supported: " + NumberOfDisplays.ToString());
                                                            Console.WriteLine("\nDispID  AdpID  Type OutType  CnctType Connected  Mapped  InfoValue DisplayName ");

                                                            for (j = 0; j < NumberOfDisplays; j++)
                                                            {
                                                                int InfoValue = DisplayInfoData[j].DisplayInfoValue;
                                                                string StrConnected = (1 == (InfoValue & 1)) ? "Yes" : "No ";
                                                                string StrMapped = (2 == (InfoValue & 2)) ? "Yes" : "No ";
                                                                int AdpID = DisplayInfoData[j].DisplayID.DisplayLogicalAdapterIndex;
                                                                string StrAdpID = (AdpID < 0) ? "--" : AdpID.ToString("d2");

                                                                Console.WriteLine(DisplayInfoData[j].DisplayID.DisplayLogicalIndex.ToString() + "        " +
                                                                                        StrAdpID + "      " +
                                                                                        DisplayInfoData[j].DisplayType.ToString() + "      " +
                                                                                        DisplayInfoData[j].DisplayOutputType.ToString() + "      " +
                                                                                        DisplayInfoData[j].DisplayConnector.ToString() + "        " +
                                                                                        StrConnected + "        " +
                                                                                        StrMapped + "      " +
                                                                                        InfoValue.ToString("x4") + "   " +
                                                                                        DisplayInfoData[j].DisplayName.ToString());
                                                            }
                                                            Console.WriteLine();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("ADL_Display_DisplayInfo_Get() returned error code " + ADLRet.ToString());
                                                        }
                                                        // Release the memory for the DisplayInfo structure
                                                        if (IntPtr.Zero != DisplayBuffer)
                                                            Marshal.FreeCoTaskMem(DisplayBuffer);
                                                        // get the perf info

                                                    }
                                                }
                                            }
                                            ADL.ADL2_Device_PMLog_Device_Destroy(IntPtr.Zero, hDevice);
                                        }
                                        else
                                        {
                                            Console.WriteLine("ADL_Adapter_Active_Get() returned error code " + ADLRet.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("ADL_Adapter_AdapterInfo_Get() returned error code " + ADLRet.ToString());
                            }
                        }
                        // Release the memory for the AdapterInfo structure
                        if (IntPtr.Zero != AdapterBuffer)
                            Marshal.FreeCoTaskMem(AdapterBuffer);
                    }
                }
                if (null != ADL.ADL_Main_Control_Destroy)
                    ADL.ADL_Main_Control_Destroy();
            }
            else
            {
                // Console.WriteLine("ADL_Main_Control_Create() returned error code " + ADLRet.ToString());
                // Console.WriteLine("\nCheck if ADL is properly installed!\n");
                return false;
            }
            return true;
        }
        static bool DoNvidiaGpus()
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
                    var fanSpeed = NvGpu.NvmlDeviceGetFanSpeed(device, 0);
                    Console.WriteLine($"Fan speed is {fanSpeed}%");
                    var rates = NvGpu.NvmlDeviceGetUtilizationRates(device);
                    Console.WriteLine("Rates: ");
                    Console.WriteLine("  GPU Utilization: {0}", rates.gpuUtilization);
                    Console.WriteLine("  VRAM Utilization: {0}", rates.memoryUtilization);
                }

                NvGpu.NvmlShutdown();
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (SystemException se)
            {
                Console.WriteLine(se.ToString());
                NvGpu.NvmlShutdown();
            }
            catch
            {
                NvGpu.NvmlShutdown();
            }
            return false;
        }
        public static void DoCpus()
        {
            var procCls = new ManagementClass("Win32_PerfFormattedData_Counters_ProcessorInformation");

            var procCol = procCls.GetInstances();
            int i = 0;
            foreach (var procObj in procCol)
            {
                var freq = Convert.ToInt32(procObj["ActualFrequency"]);
                var tim = procObj["PercentProcessorTime"];
                Console.WriteLine($"Core {i++} Frequency: {((double)freq) / 1000}Ghz, Load: {tim}%");
            }


        }
        private static string GetStringFromSByteArray(sbyte[] data)
        {
            if (data == null)
                throw new SystemException("Data can't be null");

            byte[] busIdData = Array.ConvertAll(data, (a) => (byte)a);
            return Encoding.Default.GetString(busIdData).Replace("\0", "");
        }

        static void Main2(string[] args)
        {
            var procCls = new ManagementClass("Win32_Processor");
            foreach (var procInst in procCls.GetInstances())
            {
                Console.WriteLine(procInst.GetText(TextFormat.Mof));
            }
            var perfCls = new ManagementClass("Win32_PerfFormattedData_Counters_ProcessorInformation");
            foreach (var perfInst in perfCls.GetInstances())
            {
                Console.WriteLine(perfInst.GetText(TextFormat.Mof));
            }

        }
        static void Main(string[] args)
        {
            // DoNvidiaGpus();
            // DoAmdGpus();
            // DoCpus();
            
            var hardware = new Dictionary<string, Func<float>>();
            var cpuProvider = new CoreTempCpuProvider(); // new WmiCpuProvider(); 
            cpuProvider.PublishReading += (sender, args) => { hardware[args.Path] = args.Getter; };
            cpuProvider.RefreshInterval = 1000;
            cpuProvider.Start();

            var gpuProvider = new NvidiaNvmlGpuProvider();
            gpuProvider.PublishReading += (sender, args) => { hardware[args.Path] = args.Getter; };
            gpuProvider.RefreshInterval = 1000;
            gpuProvider.Start();

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
