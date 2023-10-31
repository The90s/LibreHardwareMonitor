#undef PRINT_UPDATE_DATE
// #define PRINT_UPDATE_DATE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class SystemInformationDynamic
{
    private static Hardware.Cpu.Vendor _vendor;
    private static Computer _computer = null;
    private static Timer _updateTimer = null;

    // CPU 温度
    private static int? _cpuTemperature = 0;

    // CPU 负载
    private static int? _cpuLoad = 0;
    private static int? _memLoad = 0;
    private static int? _gpuLoad = 0;
    private static int? _gpuTemperature = 0;
    private static int? _gpuMem = 0; // TODO: 暂时不需要
    private static float? _networkSpeedUpload = 0;
    private static float? _networkSpeedDownload = 0;
    private static List<FanSpeed> _fanSpeeds = new();
    private static bool _isGpuNvidia = false;
    private static bool _isGpuAmd = false;
    private static bool _isGpuIntel = false;


    private SystemInformationDynamic() { }

    public static void init(Computer computer, int intervalMs)
    {
#if PRINT_UPDATE_DATE
        Console.WriteLine("SystemInformationDynamic:init ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
#endif
        _computer = computer;
        if (_updateTimer == null)
        {
            _updateTimer = new Timer(UpdateData, null, TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
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
    }

    public static int CpuTemperature => _cpuTemperature ?? 0;
    public static int CpuLoad => _cpuLoad ?? 0;
    public static int GpuLoad => _gpuLoad ?? 0;
    public static int GpuTemperature => _gpuTemperature ?? 0;
    public static int MemLoad => _memLoad ?? 0;
    public static float NetworkSpeedUpload => _networkSpeedUpload ?? 0;
    public static float NetworkSpeedDownload => _networkSpeedDownload ?? 0;
    public static NetworkSpeed GetNetworkSpeed() => new NetworkSpeed(_networkSpeedUpload ?? 0, _networkSpeedDownload ?? 0);

    public static void UpdateData(object obj)
    {
        if (_computer == null)
        {
            return;
        }

#if PRINT_UPDATE_DATE
        Console.WriteLine("UpdateData: ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
#endif

        foreach (IHardware hardware in _computer.Hardware)
        {
            hardware.Update();
            switch (hardware.HardwareType)
            {
                case HardwareType.Motherboard:
                    // _motherboard = hardware.Name; break;
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
                case HardwareType.Storage: break;
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

    private static void updateCpu(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Intel CPU Name: Core Average
            // AMD CPU Name: CCDs Average (Tdie)
            if ((SensorsNameEquels(sensor, "Core Average") || SensorsNameEquels(sensor, "CCDs Average (Tdie)")) && SensorsTypeIsTemperature(sensor))
            {
                _cpuTemperature = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                Console.WriteLine("cpu temperature: {0}", _cpuTemperature);
#endif
            }
            else if (SensorsNameEquels(sensor, "CPU Total") && SensorsTypeIsLoad(sensor))
            {
                _cpuLoad = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                Console.WriteLine("cpu load: {0}", _cpuLoad);
#endif
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
                    if (SensorsNameEquels(sensor, "GPU Core") && SensorsTypeIsLoad(sensor))
                    {
                        _gpuLoad = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                        Console.WriteLine("Nvidia GPU load: {0}", _cpuLoad);
#endif
                    }
                    else if (SensorsNameEquels(sensor, "GPU Core") && SensorsTypeIsTemperature(sensor))
                    {
                        _gpuTemperature = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                        Console.WriteLine("Nvidia GPU temperature: {0}", _cpuLoad);
#endif
                    }
                }
            }
        }
        else if (_isGpuAmd)
        {
            // TODO: AMD显卡还没有测试
            foreach (IHardware hardware in hardwares)
            {
                if (hardware.HardwareType != HardwareType.GpuAmd)
                {
                    continue;
                }
                foreach (ISensor sensor in hardware.Sensors)
                {
                    // TODO: 这是示例，后面会改
                    if (SensorsNameEquels(sensor, "GPU Core") && SensorsTypeIsLoad(sensor) && (int)sensor.Value != 0)
                    {
                        _gpuLoad = (int)sensor.Value;
#if PRINT_UPDATE_DATE
                        Console.WriteLine("AMD GPU load: {0}", _cpuLoad);
#endif
                    }
                    else if (SensorsNameEquels(sensor, "GPU Core") && SensorsTypeIsTemperature(sensor))
                    {
                        _gpuTemperature = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                        Console.WriteLine("AMD GPU temperature: {0}", _cpuLoad);
#endif
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
                    if (SensorsNameEquels(sensor, "D3D Video Decode") && SensorsTypeIsLoad(sensor) && (int)sensor.Value != 0)
                    {
                        // Note: D3D Video Decode 名字会有重复的；多加判断：数字是不是0
                        // |  +- D3D Video Decode :  1.67533        0  13.7776 (/gpu-intel-integrated/xxxxxx/load/9)   xxxxx
                        // |  +- D3D Video Decode :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/10)
                        _gpuLoad = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                        Console.WriteLine("Intel GPU load: {0}", _cpuLoad);
#endif
                    }
                    // else if (SensorsNameEquels(sensor, "D3D Shared Memory Used"))
                    // {
                    //     _gpuMem = (int)sensor.Value;
                    //     Console.WriteLine("Intel GPU mem load: {0}", _cpuLoad);
                    // }
                }
            }

            // intel GPU的温度就CPU的温度
            _gpuTemperature = _cpuTemperature;
#if PRINT_UPDATE_DATE
            Console.WriteLine("Intel GPU temperature: {0}", _gpuTemperature);
#endif
        }
    }

    private static void updateMem(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (SensorsNameEquels(sensor, "Memory") && SensorsTypeIsLoad(sensor))
            {
                _memLoad = (int)(sensor.Value ?? 0);
#if PRINT_UPDATE_DATE
                Console.WriteLine("mem load: {0}", _memLoad);
#endif
            }
        }
    }

    private static void updateNetwork(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Console.WriteLine("Update NetWork: name: {0}; id: {1}; value: {2}", sensor.Name, sensor.Identifier, sensor.Value);
            if (SensorsNameEquels(sensor, "Upload Speed") && sensor.SensorType == SensorType.Throughput)
            {
                if (sensor.Value != null && sensor.Value != 0)
                {
                    _networkSpeedUpload = sensor.Value / 1024;
#if PRINT_UPDATE_DATE
                    Console.WriteLine("Update NetWork: upload: {0}", _networkSpeedUpload);
#endif
                }
            }
            else if (SensorsNameStartWith(sensor, "Download Speed") && sensor.SensorType == SensorType.Throughput)
            {
                if (sensor.Value != null && sensor.Value != 0)
                {
                    _networkSpeedDownload = sensor.Value / 1024;
#if PRINT_UPDATE_DATE
                    Console.WriteLine("Update NetWork: download: {0}", _networkSpeedDownload);
#endif
                }
            }
        }
    }

    private static void updateMotherboard(IHardware hardware)
    {
        _fanSpeeds.Clear();

        foreach (IHardware subHardware in hardware.SubHardware)
        {

            ISensor[] sensors = hardware.Sensors;
            Array.Sort(sensors, CompareSensor);

            foreach (ISensor sensor in sensors)
            {
                //          Name                value      Min      Max Identifier
                // |  |  +- Fan #1         :  1628.47  1584.51  1920.34 (/lpc/nct6798d/fan/0)
                if (sensor.Name.StartsWith("Fan #") && sensor.Identifier.ToString().Contains("fan") && (int)sensor.Value != 0)
                {
                    // Console.WriteLine("update motherBoard: name:{0}, value:{1}, min:{2}, max:{3}", sensor.Name ?? string.Empty, (int)(sensor.Value ?? 0), (int)(sensor.Min ?? 0), (int)(sensor.Max ?? 0));
                    _fanSpeeds.Add(new FanSpeed(sensor.Name, (int)sensor.Value, (int)sensor.Min, (int)sensor.Max));
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
            // TODO: 可能要递归
            // initMotherboard(subHardware);
        }
    }

    private static int CompareSensor(ISensor a, ISensor b)
    {
        int c = a.SensorType.CompareTo(b.SensorType);
        if (c == 0)
            return a.Index.CompareTo(b.Index);

        return c;
    }

    public static void StopTimer()
    {
        _updateTimer?.Dispose();
        _updateTimer = null;
    }


    public static void SetTimerInterval(int intervalMs)
    {
        _updateTimer?.Change(TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
    }

    // ------------------------ helper function --------------------------

    public static bool SensorsNameEquels(ISensor sensor, string value) => sensor.Name.Equals(value);
    public static bool SensorsNameStartWith(ISensor sensor, string value) => sensor.Name.StartsWith(value);
    public static bool SensorsTypeIs(ISensor sensor, SensorType type) => sensor.SensorType == type;
    public static bool SensorsTypeIsLoad(ISensor sensor) => SensorsTypeIs(sensor, SensorType.Load);
    public static bool SensorsTypeIsTemperature(ISensor sensor) => SensorsTypeIs(sensor, SensorType.Temperature);
    // public static bool SensorsIdentifierStartWith(ISensor sensor, string value) => sensor.Identifier.ToString().StartsWith(value);
    // public static bool SensorsIdentifierContains(ISensor sensor, string value) => sensor.Identifier.ToString().Contains(value);

    public static void PrintSensorDetails(ISensor sensor)
    {
        Console.WriteLine($"Sensor Name: {0}; Value: {1}; ID: {2}; Type: {3}", sensor.Name, sensor.Value ?? 0, sensor.Identifier, sensor.SensorType);
    }

    enum FanType
    {
        // /lpc/nct6798d/fan/0
        Normal,
        // /lpc/nct6798d/control/0
        Contorl,
    }

    class FanSpeed
    {

        FanType Type;
        private string _name;
        private int? _value;
        private int? _min;
        private int? _max;

        public FanSpeed(string name, int? value, int? min, int? max)
        {
            _name = name;
            _value = value;
            _min = min;
            _max = max;
        }

        string Name { get => _name; }
        int? Value { get => _value; }
        int? Max { get => _max; }
        int? Min { get => _min; }
    }

    public class NetworkSpeed
    {
        public float uploadSpeed;
        public float downloadSpeed;
        public NetworkSpeed(float uploadSpeed, float downloadSpeed)
        {
            this.uploadSpeed = uploadSpeed;
            this.downloadSpeed = downloadSpeed;
        }
    }
}
