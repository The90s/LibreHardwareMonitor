﻿using System.Threading.Tasks;
using LibreHardwareMonitor.Warpper;

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
                    SystemInformationStatus.init(intervalMs);
                    Logger.Debug($"LibreHardwareMonitorInit end");
                    // Logger.Debug($"BIOS: {ComputerSingleton.Instance.SMBios.GetReport()}");
                }
                return true;
            });
        }
        public async Task<object> LibreHardwareMonitorIsInited() => ComputerSingleton.IsOpen;

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
        public async Task<object> Monitor(dynamic input) => SystemInfomationSpec.Monitor();


        // Status ---------------------------------------
        public async Task<object> AllStatus(dynamic input) => SystemInformationStatus.AllStatus();
        // CPU
        public async Task<object> CpuStatus(dynamic input) => SystemInformationStatus.GetCpuStatus();

        // GPU
        public async Task<object> GpuStatus(dynamic input) => SystemInformationStatus.GetGpuStatus();

        // Memory
        public async Task<object> MemoryStatus(dynamic input) => SystemInformationStatus.GetMemoryStatus();

        // Disk
        public async Task<object> DiskStatus(dynamic input) => SystemInformationStatus.GetDiskStatus();

        // Fans:
        public async Task<object> FansStatus(dynamic input) => SystemInformationStatus.GetFansStatus();

        // Network
        public async Task<object> NetworkStatus(dynamic input) => SystemInformationStatus.GetNetworkStatus();

    }
}
