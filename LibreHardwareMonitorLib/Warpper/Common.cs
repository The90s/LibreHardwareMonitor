using System;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class Common
{
    public static int CompareSensor(ISensor a, ISensor b)
    {
        int c = a.SensorType.CompareTo(b.SensorType);
        if (c == 0)
            return a.Index.CompareTo(b.Index);

        return c;
    }
}
