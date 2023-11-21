using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class Fans
{
    private static List<Fan> _sFans = new(); // RPM/S

    public Fan[] items;
    public int average; // RPM/S

    public Fans() { }

    public static void UpdateFans(Fans fans, IHardware hardware)
    {
        // 1. 更新风扇
        // 2. 主板温度？ TODO：
        _sFans.Clear();

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
                    _sFans.Add(new Fan(sensor.Name, (int)(sensor.Value ?? 0), (int)(sensor.Max ?? 0)));

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

        if (_sFans.Count() == 0)
        {
            // Note: 这个库有bug，有时候初始化获取不到主板Sensor信息，这里去重新初始化
            ComputerSingleton.Reset();
            return;
        }
        fans.average = _sFans.Sum(fan => fan.value) / _sFans.Count();
        fans.items = _sFans.ToArray();

        // TODO:
        _sFans.ForEach((fan) => Logger.Debug($"cpu fan: {fan}"));
    }


    public class Fan
    {
        public int id;
        public int value;
        public int max;

        public Fan() { }

        public Fan(string name, int value, int max)
        {
            id = GetID(name);
            this.value = value;
            this.max = max;
        }

        static public int GetID(string name)
        {
            string idString = name.Replace("Fan #", "");
            int ret = 0;
            try
            {
                ret = int.Parse(idString);
            }
            catch (FormatException e)
            {
                Logger.Debug(e.Message);
                return -1;
            }
            return ret;
        }

        static public string GetName(int id)
        {
            return $"Fan #{id}";
        }

        public override string ToString()
        {
            return $"Fan: ID={id}; Value={value}; Max:{max}";
        }
    }
}