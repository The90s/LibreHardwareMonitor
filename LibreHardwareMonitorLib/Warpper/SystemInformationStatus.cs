using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using static LibreHardwareMonitor.StatusClasses;

namespace LibreHardwareMonitor;

public class SystemInformationStatus
{
    private static Computer _computer = null;
    private static Timer _updateTimer = null;

    // CPU 温度
    private static int _cpuTemperature = 0;

    // CPU 负载
    private static int _cpuLoad = 0; // %
    private static int _cpuFan = 0; // RPM/S

    // Memory
    private static int _memLoad = 0; // %
    private static int _memUsed = 0; // MB
    private static int _memTotal = 0; // MB

    // Disk
    private static int _diskLoad = 0; // %
    private static float _diskUsed = 0; // GB
    private static long _diskTotal = 0; // GB
    private static float _diskActivity = 0; // %
    private static int _diskTemperature = 0; //
    private static float _diskReadSpeed = 0; // KB/S
    private static float _diskWriteSpeed = 0; // KB/S


    // GPU
    private static int _gpuLoad = 0;
    private static int _gpuTemperature = 0;
    private static int _gpuFan = 0;
    private static int _gpuMem = 0; // TODO: 暂时不需要

    // Network
    private static float _networkUpload = 0; // kb/s
    private static float _networkDownload = 0;// kb/s
    private static List<FanStatus> _fanSpeeds = new(); // TODO:

    // helper variable
    private static bool _isGpuNvidia = false;
    private static bool _isGpuAmd = false;
    private static bool _isGpuIntel = false;
    private static string _mainDiskName = string.Empty;

    private static object _computerUpdateLock = new();

    private SystemInformationStatus() { }

    public static void init(Computer computer, int intervalMs)
    {
        Logger.Debug($"SystemInformationDynamic:init ThreadID: {Thread.CurrentThread.ManagedThreadId}");
        _computer = computer;
        if (_updateTimer == null)
        {
            _updateTimer = new Timer(updateStatus, null, TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
        }

        // Check GPU
        foreach (IHardware hardware in _computer.Hardware)
        {
            switch (hardware.HardwareType)
            {
                case HardwareType.GpuNvidia:
                    _isGpuNvidia = true;
                    break;
                case HardwareType.GpuAmd:
                    _isGpuAmd = true;
                    break;
                case HardwareType.GpuIntel:
                    _isGpuIntel = true;
                    break;
            }
        }
        initSpec(computer);
    }

    public static void initSpec(Computer computer)
    {
        // init Memeory (Total)
        SMBios sMBios = computer.SMBios;
        foreach (var memoryDevices in sMBios.MemoryDevices)
        {
            _memTotal += memoryDevices.Size;
        }
        Logger.Debug($"Total Memory: {_memTotal}"); // MB

        // init Disk (Total)
    }

    public static CpuStatus GetCpuStatus()
    {
        lock (_computerUpdateLock) { return new(_cpuLoad, _cpuTemperature, _cpuFan); }
    }
    public static int CpuTemperature()
    {
        lock (_computerUpdateLock) { return _cpuTemperature; }
    }
    public static int CpuLoad() { lock (_computerUpdateLock) { return _cpuLoad; } }
    public static int CpuFan() { lock (_computerUpdateLock) { return _cpuFan; } }
    public static GpuStatus GetGpuStatus()
    {
        lock (_computerUpdateLock) { return new(_gpuLoad, _gpuTemperature, _gpuFan); }
    }
    public static int GpuLoad() { lock (_computerUpdateLock) { return _gpuLoad; } }
    public static int GpuTemperature() { lock (_computerUpdateLock) { return _gpuTemperature; } }
    public static int GpuFan() { lock (_computerUpdateLock) { return _cpuFan; } }

    // Memory
    public static MemStatus GetMemStatus()
    {
        lock (_computerUpdateLock) { return new(_memLoad, _memUsed, _memTotal); }
    }
    public static int MemLoad() { lock (_computerUpdateLock) { return _memLoad; } }
    public static float MemUsed() { lock (_computerUpdateLock) { return _memUsed; } }
    public static int MemTotal() { lock (_computerUpdateLock) { return _memTotal; } }

    // Disk
    public static DiskStatus GetDiskStatus()
    {
        lock (_computerUpdateLock) { return new(_diskLoad, _diskUsed, _diskTotal, _diskActivity, _diskTemperature, _diskReadSpeed, _diskWriteSpeed); }
    }
    public static int DiskLoad() { lock (_computerUpdateLock) { return _diskLoad; } }
    public static float DiskUsed() { lock (_computerUpdateLock) { return _diskUsed; } }
    public static long DiskTotal() { lock (_computerUpdateLock) { return _diskTotal; } }
    public static float DiskActivity() { lock (_computerUpdateLock) { return _diskActivity; } }
    public static int DiskTemperature() { lock (_computerUpdateLock) { return _diskTemperature; } }
    public static float DiskReadSpeed() { lock (_computerUpdateLock) { return _diskReadSpeed; } }
    public static float DiskWriteSpeed() { lock (_computerUpdateLock) { return _diskWriteSpeed; } }

    // Network
    public static NetworkStatus GetNetworkStatus()
    {
        lock (_computerUpdateLock) { return new(_networkUpload, _networkDownload); }
    }
    public static float NetworkUpload() { lock (_computerUpdateLock) { return _networkUpload; } }
    public static float NetworkDownload() { lock (_computerUpdateLock) { return _networkDownload; } }

    private static void updateStatus(object obj)
    {
        if (_computer == null)
        {
            return;
        }

        lock (_computerUpdateLock)
        {
            foreach (IHardware hardware in _computer.Hardware)
            {
                // Test：测试更新需要的时间
                // Logger.Debug($"updateStatus start: {hardware.Name}");
                hardware.Update();
                // Logger.Debug($"updateStatus end: {hardware.Name}");
                switch (hardware.HardwareType)
                {
                    case HardwareType.Motherboard:
                        updateMotherboard(hardware);
                        break;
                    case HardwareType.SuperIO: break;
                    case HardwareType.Cpu:
                        updateCpu(hardware);
                        break;
                    case HardwareType.Memory:
                        updateMem(hardware);
                        break;
                    case HardwareType.GpuNvidia: break;
                    case HardwareType.GpuAmd: break;
                    case HardwareType.GpuIntel:
                        // 显卡后面单独处理
                        break;
                    case HardwareType.Storage:
                        updateDisk(hardware);
                        break;
                    case HardwareType.Network:
                        updateNetwork(hardware);
                        break;
                    case HardwareType.Cooler: break;
                    case HardwareType.EmbeddedController: break;
                    case HardwareType.Psu: break;
                    case HardwareType.Battery: break;
                }
            }

            // 不同显卡处理
            updateGpu(_computer.Hardware);
        }
    }

    private static void updateCpu(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Intel CPU Name: Core Average
            // AMD CPU Name: CCDs Average (Tdie)
            if ((SensorUtils.NameEquels(sensor, "Core Average") || SensorUtils.NameEquels(sensor, "CCDs Average (Tdie)")) && SensorUtils.TypeIsTemperature(sensor))
            {
                _cpuTemperature = (int)(sensor.Value ?? 0);
                Logger.Debug($"cpu temperature: {_cpuTemperature}");
            }
            else if (SensorUtils.NameEquels(sensor, "CPU Total") && SensorUtils.TypeIsLoad(sensor))
            {
                _cpuLoad = (int)(sensor.Value ?? 0);
                Logger.Debug($"cpu load: {_cpuLoad}");
            }
        }
    }

    private static void updateGpu(IList<IHardware> hardwares)
    {
        if (_isGpuNvidia)
        {
            foreach (IHardware hardware in hardwares)
            {
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (hardware.HardwareType != HardwareType.GpuNvidia)
                    {
                        continue;
                    }
                    if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsLoad(sensor))
                    {
                        _gpuLoad = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Nvidia GPU load: {_gpuLoad}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsTemperature(sensor))
                    {
                        _gpuTemperature = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Nvidia GPU temperature: {_gpuTemperature}");
                    }
                    else if (SensorUtils.NameStartWith(sensor, "GPU Fan") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: 这里就取了一个, 同一个显卡的风扇转速可能差不多
                        _gpuFan = (int)sensor.Value;
                        Logger.Debug($"Nvidia GPU Fan; Name: {sensor.Name}; Value : {_gpuFan}");
                    }
                }
            }
        }
        else if (_isGpuAmd)
        {
            foreach (IHardware hardware in hardwares)
            {
                if (hardware.HardwareType != HardwareType.GpuAmd)
                {
                    continue;
                }
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsLoad(sensor) && (int)sensor.Value != 0)
                    {
                        _gpuLoad = (int)sensor.Value;
                        Logger.Debug($"AMD GPU load: {_gpuLoad}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsTemperature(sensor))
                    {
                        _gpuTemperature = (int)(sensor.Value ?? 0);
                        Logger.Debug($"AMD GPU temperature: {_gpuTemperature}");
                    }
                    else if (SensorUtils.NameStartWith(sensor, "GPU Fan") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: 这里就取了一个, 同一个显卡的风扇转速可能差不多
                        _gpuFan = (int)sensor.Value;
                        // Note: 有可能没达到显示的启动触发历零界点，风扇不会启动, 风扇历速度就为0
                        // Radeon RX 580 Series (/gpu-amd/0) 获取不到
                        // |  +- GPU Fan        :        0        0        0 (/gpu-amd/0/fan/0)
                        Logger.Debug($"AMD GPU Fan; Name: {sensor.Name}, Value: {_gpuFan}");
                    }
                }
            }
        }
        else if (_isGpuIntel)
        {
            foreach (IHardware hardware in hardwares)
            {
                if (hardware.HardwareType != HardwareType.GpuIntel)
                {
                    continue;
                }
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (SensorUtils.NameEquels(sensor, "D3D 3D") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: D3D Video Decode 名字会有重复的；多加判断：数字是不是0
                        // |  +- D3D Video Decode :  1.67533        0  13.7776 (/gpu-intel-integrated/xxxxxx/load/9)   xxxxx
                        // |  +- D3D Video Decode :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/10)
                        _gpuLoad = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Intel GPU load: {_gpuLoad}");
                    }
                }
            }

            // intel GPU的温度就CPU的温度
            _gpuTemperature = _cpuTemperature;
            // intel GPU的风扇就CPU的风扇
            _gpuFan = _cpuFan;
            Logger.Debug($"Intel GPU temperature: {_gpuTemperature}");
            Logger.Debug($"Intel GPU Fan: {_gpuFan}");
        }
        Logger.Debug($"GPU temperature: {_gpuTemperature}");
        Logger.Debug($"GPU Fan: {_gpuFan}");
    }

    private static void updateMem(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (SensorUtils.NameEquels(sensor, "Memory") && SensorUtils.TypeIsLoad(sensor))
            {
                _memLoad = (int)(sensor.Value ?? 0);
                Logger.Debug($"mem load: {_memLoad}");
            }
            else if (SensorUtils.NameEquels(sensor, "Memory Used") && SensorUtils.TypeIsData(sensor))
            {
                _memUsed = (int)((sensor.Value ?? 0) * 1024);
                Logger.Debug($"mem used: {_memUsed}");
            }
        }
        Logger.Debug($"mem total: {_memTotal}");
    }

    private static void updateDisk(IHardware hardware)
    {
        AbstractStorage storage = (AbstractStorage)hardware;
        if (string.IsNullOrEmpty(_mainDiskName))
        {

            foreach (var deviceInfo in storage.DriveInfos)
            {
                if (!deviceInfo.IsReady)
                    continue;
                try
                {
                    if (deviceInfo.Name.Equals("C:\\"))
                    {
                        _mainDiskName = hardware.Name;
                        Logger.Debug($"update disk: Driver Name: {_mainDiskName}");
                        break;
                    }
                }
                // catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        if (_diskTotal == 0 && _mainDiskName.Equals(hardware.Name))
        {
            foreach (var deviceInfo in storage.DriveInfos)
            {
                if (!deviceInfo.IsReady)
                    continue;

                try
                {
                    _diskTotal += deviceInfo.TotalSize;
                    Logger.Debug($"update disk Disk Driver Total Size: name {deviceInfo.Name}; Total Size {deviceInfo.TotalSize}");

                }
                // catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            Logger.Debug($"Disk Driver Total Size: {_diskTotal} KB | {_diskTotal / 1024 / 1024 / 1024} G");
        }

        if (_mainDiskName.Equals(hardware.Name))
        {
            foreach (var sensor in hardware.Sensors)
            {
                if (SensorUtils.NameEquels(sensor, "Used Space") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    _diskLoad = (int)(sensor.Value ?? 0);
                    _diskUsed = _diskTotal / 100 * _diskLoad;
                    Logger.Debug($"disk load: {_diskLoad}%");
                    Logger.Debug($"disk used: {_diskUsed} KB");
                }
                else if (SensorUtils.NameEquels(sensor, "Temperature") && SensorUtils.TypeIsTemperature(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    _diskTemperature = (int)(sensor.Value ?? 0);
                    Logger.Debug($"disk temperature: {_diskTemperature}");
                }
                // TODO:
                else if (SensorUtils.NameEquels(sensor, "Total Activity") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    _diskActivity = sensor.Value ?? 0;
                    Logger.Debug($"disk total activity: {_diskActivity}");
                }
                else if (SensorUtils.NameEquels(sensor, "Read Rate") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    _diskReadSpeed = (sensor.Value ?? 0) / 1024;
                    Logger.Debug($"disk read speed: {_diskReadSpeed}");
                }
                else if (SensorUtils.NameEquels(sensor, "Write Rate") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    _diskWriteSpeed = (sensor.Value ?? 0) / 1024;
                    Logger.Debug($"disk write speed: {_diskWriteSpeed}");
                }
            }
        }
    }

    private static void updateNetwork(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Console.WriteLine("Update network: name: {0}; id: {1}; value: {2}", sensor.Name, sensor.Identifier, sensor.Value);
            if (SensorUtils.NameEquels(sensor, "Upload Speed") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                // 转成KB/s
                _networkUpload = (float)sensor.Value / 1024;
                Logger.Debug($"network upload: {_networkUpload}");
            }
            else if (SensorUtils.NameStartWith(sensor, "Download Speed") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                // 转成KB/s
                _networkDownload = (float)sensor.Value / 1024;
                Logger.Debug($"network download: {_networkDownload}");
            }
        }
    }

    private static void updateMotherboard(IHardware hardware)
    {
        // 1. 更新风扇
        // 2. 主板温度？ TODO：
        _fanSpeeds.Clear();
        int cpuFan = 0;

        foreach (IHardware subHardware in hardware.SubHardware)
        {
            subHardware.Update();
            ISensor[] sensors = subHardware.Sensors;
            Array.Sort(sensors, CompareSensor);

            foreach (ISensor sensor in sensors)
            {
                //          Name                value      Min      Max Identifier
                // |  |  +- Fan #1         :  1628.47  1584.51  1920.34 (/lpc/nct6798d/fan/0)
                // Logger.Debug(String.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", ' ', sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier));
                if (SensorUtils.NameStartWith(sensor, "Fan #") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    // TODO: 还不确定多个风扇中 哪个是CPU风扇，哪个是风道风扇
                    // 这里只是做了一个简单处理，取了一个最大速度的
                    cpuFan = (int)sensor.Value > cpuFan ? (int)sensor.Value : cpuFan;
                    Logger.Debug($"cpu fan Name: {sensor.Name}; Value: {sensor.Value}");
                }
            }
            // c:\Users\15519\AppData\Local\Temp\SGPicFaceTpBq\13672\006A6463.png
            // String.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier)
            //          Name                value      Min      Max Identifier
            // |  |  +- Fan #1         :  1628.47  1584.51  1920.34 (/lpc/nct6798d/fan/0)
            // |  |  +- Fan #2         :        0        0        0 (/lpc/nct6798d/fan/1)
            // |  |  +- Fan #3         :        0        0        0 (/lpc/nct6798d/fan/2)
            // |  |  +- Fan #4         :        0        0        0 (/lpc/nct6798d/fan/3)
            // |  |  +- Fan #5         :        0        0        0 (/lpc/nct6798d/fan/4)
            // |  |  +- Fan #6         :        0        0        0 (/lpc/nct6798d/fan/5)
            // |  |  +- Fan #7         :        0        0        0 (/lpc/nct6798d/fan/6)
            // |  |  +- Fan #1         :  67.8431  66.2745  81.5686 (/lpc/nct6798d/control/0)
            // |  |  +- Fan #2         :   34.902   32.549  57.6471 (/lpc/nct6798d/control/1)
            // |  |  +- Fan #3         :       60       60       60 (/lpc/nct6798d/control/2)
            // |  |  +- Fan #4         :       60       60       60 (/lpc/nct6798d/control/3)
            // |  |  +- Fan #5         :       60       60       60 (/lpc/nct6798d/control/4)
            // |  |  +- Fan #6         :      100      100      100 (/lpc/nct6798d/control/5)
            // |  |  +- Fan #7         :      100      100      100 (/lpc/nct6798d/control/6)
        }
        _cpuFan = cpuFan;
        Logger.Debug($"cpu fan: {_cpuFan}");
    }

    private static int CompareSensor(ISensor a, ISensor b)
    {
        int c = a.SensorType.CompareTo(b.SensorType);
        if (c == 0)
            return a.Index.CompareTo(b.Index);

        return c;
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
