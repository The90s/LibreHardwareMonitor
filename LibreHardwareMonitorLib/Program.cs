using System;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

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
                    Logger.Debug($"LibreHardwareMonitorInit start");

                    ComputerSingleton.Init();
                    SystemInfomationSpec.init(ComputerSingleton.Hardwares);

                    // Update system info: network、CPU load、GPU load、temp、fan、etc
                    SystemInformationStatus.init(ComputerSingleton.Instance, intervalMs);
                    Logger.Debug($"LibreHardwareMonitorInit end");
                }
                return true;
            });
        }

        // Logger ---------------------------------------

        public async Task<object> LibreHardwareMonitorLoggerSetLevel(string level) => Logger.SetLevel(level);

        // Update Status Timer ---------------------------------------
        public async Task<object> LibreHardwareMonitorUpdataStatusTimerSetInterval(int intervalMs) => SystemInformationStatus.TimerSetInterval(intervalMs);
        public async Task<object> LibreHardwareMonitorUpdateStatusTimerStop() => SystemInformationStatus.TimerStop();

        // Specification ---------------------------------------
        public async Task<object> Reports() => ComputerReports.GetReports();
        public async Task<object> ReportHardware(string type) => ComputerReports.GetReport(type);
        public async Task<object> ReportMethodboard() => ComputerReports.GetMethodboardReport();
        public async Task<object> ReportCpu() => ComputerReports.GetCpuReport();
        public async Task<object> ReportGPU() => ComputerReports.GetGPUReport();
        public async Task<object> ReportMemory() => ComputerReports.GetMemoryReport();
        public async Task<object> ReportStorage() => ComputerReports.GetStorageReport();
        public async Task<object> ReportNetwork() => ComputerReports.GetNetworkReport();

        // Specification ---------------------------------------
        // TODO: 还有些其他的，暂时没用到，后面需要再去增加
        public async Task<object> GetMotherboard(dynamic input) => SystemInfomationSpec.MotherBoard;
        public async Task<object> GetGPU(dynamic input) => SystemInfomationSpec.Gpu;

        // Status ---------------------------------------
        // CPU
        public async Task<object> CpuStatus(dynamic input) => SystemInformationStatus.GetCpuStatus();
        public async Task<object> CpuTemperature(dynamic input) => SystemInformationStatus.CpuTemperature();
        public async Task<object> CpuLoad(dynamic input) => SystemInformationStatus.CpuLoad();
        public async Task<object> CpuFan(dynamic input) => SystemInformationStatus.CpuFan();

        // GPU
        public async Task<object> GpuStatus(dynamic input) => SystemInformationStatus.GetGpuStatus();
        public async Task<object> GpuTemperature(dynamic input) => SystemInformationStatus.GpuTemperature();
        public async Task<object> GpuLoad(dynamic input) => SystemInformationStatus.GpuLoad();
        public async Task<object> GpuFan(dynamic input) => SystemInformationStatus.GpuFan();

        // Memory
        public async Task<object> MemStatus(dynamic input) => SystemInformationStatus.GetMemStatus();
        public async Task<object> MemLoad(dynamic input) => SystemInformationStatus.MemLoad();
        public async Task<object> MemUsed(dynamic input) => SystemInformationStatus.MemUsed();
        public async Task<object> MemTotal(dynamic input) => SystemInformationStatus.MemTotal();

        // Network
        public async Task<object> NetworkStatus(dynamic input) => SystemInformationStatus.GetNetworkStatus();
        public async Task<object> NetworkUpload(dynamic input) => SystemInformationStatus.NetworkUpload();
        public async Task<object> NetworkDownload(dynamic input) => SystemInformationStatus.NetworkDownload();

    }
}
