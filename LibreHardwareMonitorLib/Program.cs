using System.Threading.Tasks;
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
        public async Task<object> CpuTemperature(dynamic input) => SystemInformationStatus.CpuTemperature();
        public async Task<object> CpuLoad(dynamic input) => SystemInformationStatus.CpuLoad();
        public async Task<object> CpuFans(dynamic input) => SystemInformationStatus.CpuFans();

        public async Task<object> CpuFanAverage(dynamic input) => SystemInformationStatus.CpuFanAverage();
        public async Task<object> CpuSpeedAverage(dynamic input) => SystemInformationStatus.CpuSpeedAverage();

        // GPU
        public async Task<object> GpuStatus(dynamic input) => SystemInformationStatus.GetGpuStatus();
        public async Task<object> GpuTemperature(dynamic input) => SystemInformationStatus.GpuTemperature();
        public async Task<object> GpuLoad(dynamic input) => SystemInformationStatus.GpuLoad();
        public async Task<object> GpuFan(dynamic input) => SystemInformationStatus.GpuFan();

        // Memory
        public async Task<object> MemoryStatus(dynamic input) => SystemInformationStatus.GetMemoryStatus();
        public async Task<object> MemLoad(dynamic input) => SystemInformationStatus.MemLoad();
        public async Task<object> MemUsed(dynamic input) => SystemInformationStatus.MemUsed();
        public async Task<object> MemTotal(dynamic input) => SystemInformationStatus.MemTotal();

        // Disk
        public async Task<object> DiskStatus(dynamic input) => SystemInformationStatus.GetDiskStatus();
        public async Task<object> DiskLoad(dynamic input) => SystemInformationStatus.DiskLoad();
        public async Task<object> DiskUsed(dynamic input) => SystemInformationStatus.DiskUsed();
        public async Task<object> DiskTotal(dynamic input) => SystemInformationStatus.DiskTotal();
        public async Task<object> DiskActivity(dynamic input) => SystemInformationStatus.DiskActivity();
        public async Task<object> DiskTemperature(dynamic input) => SystemInformationStatus.DiskTemperature();
        public async Task<object> DiskReadSpeed(dynamic input) => SystemInformationStatus.DiskReadSpeed();
        public async Task<object> DiskWriteSpeed(dynamic input) => SystemInformationStatus.DiskWriteSpeed();

        // Network
        public async Task<object> NetworkStatus(dynamic input) => SystemInformationStatus.GetNetworkStatus();
        public async Task<object> NetworkUpload(dynamic input) => SystemInformationStatus.NetworkUpload();
        public async Task<object> NetworkDownload(dynamic input) => SystemInformationStatus.NetworkDownload();

    }
}
