using System;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class SensorUtils
{
    // ------------------------ helper function --------------------------

    public static bool NameEquels(ISensor sensor, string value) => sensor.Name.Equals(value);
    public static bool NameStartWith(ISensor sensor, string value) => sensor.Name.StartsWith(value);
    public static bool TypeIs(ISensor sensor, SensorType type) => sensor.SensorType == type;
    public static bool TypeIsLoad(ISensor sensor) => TypeIs(sensor, SensorType.Load); // %
    public static bool TypeIsFan(ISensor sensor) => TypeIs(sensor, SensorType.Fan);
    public static bool TypeIsTemperature(ISensor sensor) => TypeIs(sensor, SensorType.Temperature); // °C
    public static bool TypeIsData(ISensor sensor) => TypeIs(sensor, SensorType.Data); // GB
    public static bool TypeIsThroughput(ISensor sensor) => TypeIs(sensor, SensorType.Throughput); // B/s
    public static bool ValueIsNotNullAndZero(ISensor sensor) => sensor.Value != null && (int)sensor.Value != 0;

    public static void PrintSensorDetails(ISensor sensor)
    {
        Console.WriteLine($"Sensor Name: {0}; Value: {1}; ID: {2}; Type: {3}", sensor.Name, sensor.Value ?? 0, sensor.Identifier, sensor.SensorType);
    }

}
