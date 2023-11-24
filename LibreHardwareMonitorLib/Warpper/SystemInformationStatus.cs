using System;
using System.Threading;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class SystemInformationStatus
{
    private static Timer _updateTimer = null;

    private static AllStatus _allStatus = new();

    private static object _computerUpdateLock = new();

    private SystemInformationStatus() { }

    public static void init(int intervalMs)
    {
        Logger.Debug($"SystemInformationDynamic:init ThreadID: {Thread.CurrentThread.ManagedThreadId}");
        if (_updateTimer == null)
        {
            _updateTimer = new Timer(updateStatus, null, TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
        }

        // Check GPU
        GPU.InitType(ComputerSingleton.Instance);
        // initSpec(ComputerSingleton.Instance);
        Memory.initTotal(ComputerSingleton.Instance);
    }

    public static object AllStatus() { lock (_computerUpdateLock) { return _allStatus; } }

    // CPU
    public static CPU GetCpuStatus() { lock (_computerUpdateLock) { return _allStatus.cpu; } }

    // GPU
    public static GPU GetGpuStatus() { lock (_computerUpdateLock) { return _allStatus.gpu; } }

    // Memory
    public static Memory GetMemoryStatus() { lock (_computerUpdateLock) { return _allStatus.memory; } }

    // Disk
    public static Disk GetDiskStatus() { lock (_computerUpdateLock) { return _allStatus.disk; } }

    // Fans
    public static Fans.Fan[] GetFansStatus() { lock (_computerUpdateLock) { return _allStatus.fans; } }

    // Network
    public static Network GetNetworkStatus() { lock (_computerUpdateLock) { return _allStatus.network; } }

    private static void updateStatus(object obj)
    {

        lock (_computerUpdateLock)
        {
            foreach (IHardware hardware in ComputerSingleton.Instance.Hardware)
            {
                // Test：测试更新需要的时间
                // Logger.Debug($"updateStatus start: {hardware.Name}");
                hardware.Update();
                // Logger.Debug($"updateStatus end: {hardware.Name}");
                switch (hardware.HardwareType)
                {
                    case HardwareType.Motherboard:
                        Fans.UpdateFans(ref _allStatus.fans, hardware);
                        break;
                    case HardwareType.SuperIO: break;
                    case HardwareType.Cpu:
                        CPU.Update(_allStatus.cpu, hardware);
                        break;
                    case HardwareType.Memory:
                        Memory.Update(_allStatus.memory, hardware);
                        break;
                    case HardwareType.GpuNvidia: break;
                    case HardwareType.GpuAmd: break;
                    case HardwareType.GpuIntel:
                        // Note:显卡后面单独处理:有可能是集成显示，需要在更新CPU之后在去处理
                        break;
                    case HardwareType.Storage:
                        Disk.Update(_allStatus.disk, hardware);
                        break;
                    case HardwareType.Network:
                        Network.Update(_allStatus.network, hardware);
                        break;
                    case HardwareType.Cooler: break;
                    case HardwareType.EmbeddedController: break;
                    case HardwareType.Psu: break;
                    case HardwareType.Battery: break;
                }
            }

            // 不同显卡处理
            // updateGpu(ComputerSingleton.Instance.Hardware);
            GPU.Update(_allStatus.gpu, ComputerSingleton.Instance.Hardware, _allStatus.cpu);
        }
    }

    public static object TimerStop()
    {
        _updateTimer?.Dispose();
        _updateTimer = null;
        return null;
    }

    public static object TimerSetInterval(int intervalMs)
    {
        _updateTimer?.Change(TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
        return null;
    }
}
