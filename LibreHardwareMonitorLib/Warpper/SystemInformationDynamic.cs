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
    private static System.Threading.Timer _updateTimer = null;

    // private static
    private static int? _cpuTemperature = 0;
    private static int? _cpuLoad = 0;
    private static int? _memLoad = 0;
    private static int _gpuLoad = 0;
    private static int _gpuMem = 0;
    private static int _gpuTemp = 0;
    private static float? _networkUploadSpeed = 0;
    private static float? _networkDownloadSpeed = 0;
    private static List<FanSpeed> _fanSpeeds = null;

    SystemInformationDynamic(Computer computer, ushort ms)
    {
        _computer = computer;
        _updateTimer = new Timer(Update, null, TimeSpan.FromMilliseconds(ms), TimeSpan.FromMilliseconds(ms));
    }

    public static void Update(object obj)
    {
        if (_computer == null)
        {
            return;
        }

        foreach (IHardware hardware in _computer.Hardware)
        {
            switch (hardware.HardwareType)
            {
                case HardwareType.Motherboard:
                    {
                        // _motherboard = hardware.Name; break;
                        initMotherboard(hardware);
                    }
                    break;
                case HardwareType.SuperIO: break;
                case HardwareType.Cpu:
                    initCpu(hardware);
                    break;
                case HardwareType.Memory:
                    initMem(hardware);
                    break;
                // TODO: 不同显卡处理
                case HardwareType.GpuNvidia: break;
                case HardwareType.GpuAmd: break;
                case HardwareType.GpuIntel:
                    initIntelGpu(hardware);
                    break;
                case HardwareType.Storage: break;
                case HardwareType.Network:
                    initNetwork(hardware);
                    break;
                case HardwareType.Cooler: break;
                case HardwareType.EmbeddedController: break;
                case HardwareType.Psu: break;
                case HardwareType.Battery: break;
            }
            ISensor[] sensors = hardware.Sensors;
            Array.Sort(sensors, CompareSensor);

            hardware.Update();
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine(hardware.GetReport());
            Console.WriteLine("------------------------------------------------------------------------");
            // hardware.GetReport();
            // if (hardware.SubHardware != null && hardware.SubHardware.Length > 0) { }
        }
    }

    private static void initIntelGpu(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (sensor.Name.Equals("D3D Video Decode") && (int)sensor.Value != 0)
            {
                _gpuLoad = (int)sensor.Value;
            }
            else if (sensor.Name.Equals("D3D Shared Memory Used"))
            {
                _gpuMem = (int)sensor.Value;
            }
        }
    }

    private static void initMem(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (sensor.Name.Equals("Memory"))
            {
                _memLoad = (int)sensor.Value;
            }
        }
    }

    private static void initCpu(IHardware hardware)
    {

        foreach (ISensor sensor in hardware.Sensors)
        {
            if (sensor.Name.Equals("Core Average"))
            {
                _cpuTemperature = (int)sensor.Value;
            }
            else if (sensor.Name.Equals("CPU Total"))
            {
                _cpuLoad = (int)sensor.Value;

            }
        }
    }

    private static void initNetwork(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (sensor.Name.Equals("Upload Speed") && sensor.SensorType == SensorType.Throughput)
            {
                _networkUploadSpeed = sensor.Value;
            }
            else if (sensor.Name.Equals("Download Speed") && sensor.SensorType == SensorType.Throughput)
            {
                _networkDownloadSpeed = sensor.Value;
            }
        }
    }

    private static void initMotherboard(IHardware hardware)
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


    // private static void _internalUpdate()

    public static void StopTask()
    {
        _updateTimer?.Dispose();
    }


    public static void SetTimerInterval(ushort ms)
    {
        _updateTimer?.Change(TimeSpan.FromMilliseconds(ms), TimeSpan.FromMilliseconds(ms));
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
}
