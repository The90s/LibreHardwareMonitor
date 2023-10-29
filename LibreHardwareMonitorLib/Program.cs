using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;
using System.Dynamic;

namespace LibreHardwareMonitor
{

    public class Program
    {
        public async Task<object> LibreHardwareMonitorInit(int intervalMs)
        {
            return await Task.Run(() =>
            {
                if (!ComputerSingleton.IsOpen)
                {
                    // int intervalMs = (int)input.intervalMs;
                    Console.WriteLine("LibreHardwareMonitorInit: ms: {0}", intervalMs);

                    ComputerSingleton.Init();
                    SystemInfomationStatic.init(ComputerSingleton.Hardwares);

                    // update info: network、CPU load、GPU load、temp
                    SystemInformationDynamic.init(ComputerSingleton.Instance, intervalMs);
                    Console.WriteLine("LibreHardwareMonitorInit");
                }
                return true;
            });
        }

        public void LibreHardwareMonitorSetUpdateDataTimerInterval(int intervalMs)
        {
            SystemInformationDynamic.SetTimerInterval(intervalMs);
        }
        public void LibreHardwareMonitorStopUpdateDataTimerInterval(int intervalMs)
        {
            SystemInformationDynamic.StopTimer();
        }


        // public async Task<object> ComputerPrintInfoTree(dynamic input)
        // {
        //     return await Task.Run(() =>
        //     {
        //         ComputerSingleton.Instance.Accept(new UpdateVisitor());
        //         // LibreHardwareMonitorInit(_interval);
        //         // computer.Accept(new UpdateVisitor());

        //         var stringBuilder = new StringBuilder(1024);
        //         foreach (IHardware hardware in ComputerSingleton.Hardwares)
        //         {
        //             hardware.Update();
        //             // if (hardware.HardwareType == HardwareType.Motherboard)
        //             // {

        //             //     Console.WriteLine($"foreach name: {hardware.Name}; id: {hardware.Identifier}; ToString: {hardware.ToString()}; type: {hardware.GetType}");
        //             // }
        //             Console.WriteLine($"foreach name: {hardware.Name}; id: {hardware.Identifier}; ToString: {hardware.ToString()}; type: {hardware.HardwareType}");
        //             // if (hardware.HardwareType == HardwareType.Motherboard)
        //             {
        //                 // Console.WriteLine("[dll]: is motherboard");
        //                 ReportHardwareSensorTree(hardware, stringBuilder, "|  ");
        //             }
        //             Console.WriteLine("Hardware: {0}", hardware.Name);
        //         }
        //         // return stringBuilder.ToString();
        //         return "foo";
        //     });
        // }



        // public static void ReportHardwareSensorTree(IHardware hardware, StringBuilder stringBuilder, string space)
        // {
        //     Console.WriteLine("[dll]: ReportHardwareSensorTree");
        //     stringBuilder.Append(string.Format("\r{0}|", space));
        //     stringBuilder.Append(string.Format("\r{0}+- {1} ({2})\r", space, hardware.Name, hardware.Identifier));
        //     Console.WriteLine("[dll]: Name:{0}; id: {1}", hardware.Name, hardware.Identifier);
        //     Console.WriteLine("[dll]: ToString: {0}, type: {1}", hardware.ToString(), hardware.HardwareType);
        //     Console.WriteLine("[dll]: Sensors: {0}, type: {1}", hardware.Sensors.Length, hardware.Properties);
        //     Console.WriteLine("[dll]: SubHardware: {0}, type: {1}", hardware.SubHardware.Length, hardware.Properties);

        //     foreach (KeyValuePair<string, string> item in hardware.Properties)
        //     {
        //         Console.WriteLine("[dll]: Properties key: {0}, value: {1}", item.Key, item.Value);

        //     }
        //     // Console.WriteLine("[dll]: {0}, ({1})", hardware.ToString(), hardware.HardwareType);

        //     ISensor[] sensors = hardware.Sensors;
        //     Array.Sort(sensors, CompareSensor);

        //     foreach (ISensor sensor in sensors)
        //     {
        //         stringBuilder.Append(string.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier));
        //         Console.WriteLine("[dll[: foreach sensor {0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier);
        //     }
        //     foreach (IHardware subHardware in hardware.SubHardware)
        //     {
        //         Console.WriteLine("[dll]: subHardware: {0}, name: {1}", subHardware.HardwareType, subHardware.Name);
        //         ReportHardwareSensorTree(subHardware, stringBuilder, "|  ");
        //     }
        // }

        // public async Task<object> ComputerPrintInfoTree(dynamic input)
        // {
        //     return await Task.Run(() =>
        //        {

        //            Console.WriteLine("ComputerPrintInfoTree:run ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
        //            // await LibreHardwareMonitorInit(_interval);
        //            // computer.Accept(new UpdateVisitor());

        //            var stringBuilder = new StringBuilder(1024);
        //            foreach (IHardware hardware in ComputerSingleton.Hardwares)
        //            {
        //                hardware.Update();
        //                //    if (hardware.HardwareType == HardwareType.Motherboard)
        //                //        Console.WriteLine($"foreach name: {hardware.Name}; id: {hardware.Identifier}; ToString: {hardware.ToString()}; type: {hardware.GetType}");
        //                ReportHardwareSensorTree(hardware, stringBuilder, "|  ");
        //                //     Console.WriteLine("Hardware: {0}", hardware.Name);
        //            }
        //            // return computer.GetReport();
        //            return stringBuilder.ToString();
        //        });
        // }

        // private static void ReportHardwareSensorTree(IHardware hardware, StringBuilder stringBuilder, string space)
        // {
        //     stringBuilder.Append(String.Format("\r{0}|", space));
        //     stringBuilder.Append(String.Format("\r{0}+- {1} ({2})\r", space, hardware.Name, hardware.Identifier));
        //     Console.WriteLine("{0}|", space);
        //     Console.WriteLine("{0}+- {1} ({2})", space, hardware.Name, hardware.Identifier);

        //     ISensor[] sensors = hardware.Sensors;
        //     Array.Sort(sensors, CompareSensor);

        //     foreach (ISensor sensor in sensors)
        //     {
        //         stringBuilder.Append(String.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier));
        //         Console.WriteLine("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier);
        //     }
        //     foreach (IHardware subHardware in hardware.SubHardware)
        //         ReportHardwareSensorTree(subHardware, stringBuilder, "|  ");
        // }

        // private static int CompareSensor(ISensor a, ISensor b)
        // {
        //     int c = a.SensorType.CompareTo(b.SensorType);
        //     if (c == 0)
        //         return a.Index.CompareTo(b.Index);

        //     return c;
        // }

        // public async Task<object> ComputerReport(dynamic input)
        // {
        //     return ComputerSingleton.Instance.GetReport();
        // }

        // Reports

        public async Task<object> GetReport(string input)
        {
            if (input.ToLower().Equals("all") || string.IsNullOrEmpty(input))
            {
                return ComputerReportsWarpper.GetReports();
            }
            else
            {
                if (Enum.TryParse<HardwareType>(input, out HardwareType type))
                {
                    return ComputerReportsWarpper.GetReport(type);
                }
                else
                {
                    return string.Format($"Error HardwareType:{0}, please check input parameter", input);
                }
            }
        }

        public async Task<object> GetComputerReports() => ComputerReportsWarpper.GetReports();

        public async Task<object> GetMethodboardReport() => ComputerReportsWarpper.GetMethodboardReport();

        public async Task<object> GetCpuReport() => ComputerReportsWarpper.GetCpuReport();

        public async Task<object> GetGPUReport() => ComputerReportsWarpper.GetGPUReport();

        public async Task<object> GetMemoryReport() => ComputerReportsWarpper.GetMemoryReport();

        public async Task<object> GetStorageReport() => ComputerReportsWarpper.GetStorageReport();

        public async Task<object> GetNetworkReport() => ComputerReportsWarpper.GetNetworkReport();
        // TODO: 还有些其他的，暂时没用到，后面需要再去增加


        // spec
        public async Task<object> GetMotherboard(dynamic input) => SystemInfomationStatic.MotherBoard;
        public async Task<object> GetGPU(dynamic input) => SystemInfomationStatic.Gpu;

        // dynmic Data
        public async Task<object> CpuTemperature(dynamic input) => SystemInformationDynamic.CpuTemperature;
        public async Task<object> CpuLoad(dynamic input) => SystemInformationDynamic.CpuLoad;
        public async Task<object> GpuTemperature(dynamic input) => SystemInformationDynamic.GpuTemperature;
        public async Task<object> GpuLoad(dynamic input) => SystemInformationDynamic.GpuLoad;
        public async Task<object> MemLoad(dynamic input) => SystemInformationDynamic.MemLoad;
        public async Task<object> NetworkSpeedUpload(dynamic input) => SystemInformationDynamic.NetworkSpeedUpload;
        public async Task<object> NetworkSpeedDownload(dynamic input) => SystemInformationDynamic.NetworkSpeedDownload;
        public async Task<object> NetworkSpeed(dynamic input) => SystemInformationDynamic.GetNetworkSpeed();

    }
}
