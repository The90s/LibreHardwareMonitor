using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class CPU
{
    private static List<int> _cpuSpeeds = new(); // MHz
    private static List<Fan> _cpuFans = new(); // RPM/S
    public int load; // %
    public float temperature; //
    public Fan[] fans;
    public int fanAverage; // RPM/S
    public int speedAverage; // Frequency
    public float power; // W

    // public int Load => load;
    // public float Temperature => temperature;
    // // public Fan[] Fans => fans;
    // public Fan[] Fans => _cpuFans.ToArray();
    // public int FanAverage => fanAverage;
    // public int SpeedAverage => speedAverage;

    public CPU() { }
    public CPU(int load, float temperature, int[] fans, int fanAverage, int speedAverage)
    {
        this.load = load;
        this.temperature = temperature;
        // this.fans = fans;
        this.fanAverage = fanAverage;
        this.speedAverage = speedAverage;
    }

    public static void Update(CPU cpu, IHardware hardware)
    {
        _cpuSpeeds.Clear();
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Intel CPU Name: Core Average
            // AMD CPU Name: CCDs Average (Tdie)
            if ((SensorUtils.NameEquels(sensor, "Core Average") || SensorUtils.NameEquels(sensor, "CCDs Average (Tdie)")) && SensorUtils.TypeIsTemperature(sensor))
            {
                cpu.temperature = sensor.Value ?? 0;
                Logger.Debug($"cpu temperature: {cpu.temperature}");
            }
            else if (SensorUtils.NameEquels(sensor, "CPU Total") && SensorUtils.TypeIsLoad(sensor))
            {
                cpu.load = (int)(sensor.Value ?? 0);
                Logger.Debug($"cpu load: {cpu.load}");
            }
            else if (SensorUtils.NameStartWith(sensor, "CPU Core #") && SensorUtils.TypeIsClock(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                _cpuSpeeds.Add((int)(sensor.Value ?? 0));
            }
            else if (SensorUtils.NameEquels(sensor, "CPU Package") && SensorUtils.TypeIsPower(sensor))
            {
                cpu.power = (int)(sensor.Value ?? 0);
            }
        }
        if (_cpuSpeeds.Count() != 0)
        {
            cpu.speedAverage = (int)_cpuSpeeds.Average();
        }
    }

    public static void UpdateFans(CPU cpu, IHardware hardware)
    {
        // 1. 更新风扇
        // 2. 主板温度？ TODO：
        _cpuFans.Clear();

        foreach (IHardware subHardware in hardware.SubHardware)
        {
            Logger.Debug($"subHardware type: {subHardware.HardwareType}");
            if (subHardware.HardwareType == HardwareType.Cooler)
            { }
            subHardware.Update();
            ISensor[] sensors = subHardware.Sensors;
            Array.Sort(sensors, Common.CompareSensor);

            foreach (ISensor sensor in sensors)
            {
                //          Name                value      Min      Max Identifier
                // |  |  +- Fan #1         :  1628.47  1584.51  1920.34 (/lpc/nct6798d/fan/0)
                // Logger.Debug(String.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", ' ', sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier));
                if (SensorUtils.NameStartWith(sensor, "Fan #") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    // TODO: 还不确定多个风扇中 哪个是CPU风扇，哪个是风道风扇
                    // 这里只是做了一个简单处理，取了一个最大速度的
                    // _cpuFans.Add((int)sensor.Value);
                    _cpuFans.Add(new Fan(sensor.Name, (int)(sensor.Value ?? 0), (int)(sensor.Max ?? 0)));

                    Logger.Debug($"cpu fan Name: {sensor.Name}; Value: {sensor.Value}; Max: {sensor.Max}");
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

        if (_cpuFans.Count() == 0)
        {
            // Note: 这个库有bug，有时候初始化获取不到主板Sensor信息，这里去重新初始化
            ComputerSingleton.Reset();
            return;
        }
        cpu.fanAverage = _cpuFans.Sum(fan => fan.value) / _cpuFans.Count();
        cpu.fans = _cpuFans.ToArray();

        // TODO:
        _cpuFans.ForEach((fan) => Logger.Debug($"cpu fan: {fan}"));
    }
}