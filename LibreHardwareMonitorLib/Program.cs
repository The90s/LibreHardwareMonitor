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
                    SystemInfomationSpec.init(ComputerSingleton.Hardwares);

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
        public async Task<object> GetMotherboard(dynamic input) => SystemInfomationSpec.MotherBoard;
        public async Task<object> GetGPU(dynamic input) => SystemInfomationSpec.Gpu;

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
