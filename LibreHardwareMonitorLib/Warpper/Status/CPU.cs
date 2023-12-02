using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class CPU
{
    private static List<int> _cpuSpeeds = new(); // MHz
    public int load; // %
    public float temperature; //
    public int speedAverage; // Frequency
    public float power; // W

    public CPU() { }
    public CPU(int load, float temperature, int speedAverage)
    {
        this.load = load;
        this.temperature = temperature;
        this.speedAverage = speedAverage;
    }

    public static void Update(CPU cpu, IHardware hardware)
    {
        _cpuSpeeds.Clear();
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Intel CPU Name: Core Package
            // AMD CPU Name: Package
            if ((SensorUtils.NameEquels(sensor, "Core Package") || SensorUtils.NameEquels(sensor, "Package")) && SensorUtils.TypeIsTemperature(sensor))
            {
                cpu.temperature = sensor.Value ?? 0;
                Logger.Debug($"cpu temperature: {cpu.temperature}");
            }
            else if (SensorUtils.NameEquels(sensor, "CPU Total") && SensorUtils.TypeIsLoad(sensor))
            {
                cpu.load = (int)(sensor.Value ?? 0);
                Logger.Debug($"cpu load: {cpu.load}");
            }
            else if ((SensorUtils.NameStartWith(sensor, "CPU Core #") || SensorUtils.NameStartWith(sensor, "Core #3")) && SensorUtils.TypeIsClock(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                _cpuSpeeds.Add((int)(sensor.Value ?? 0));
                Logger.Debug($"cpu {sensor.Name} speed: {sensor.Value}");
            }
            else if ((SensorUtils.NameEquels(sensor, "CPU Package") || SensorUtils.NameEquels(sensor, "Package")) && SensorUtils.TypeIsPower(sensor))
            {
                cpu.power = (int)(sensor.Value ?? 0);
                Logger.Debug($"cpu power: {sensor.Value}");
            }
        }
        if (_cpuSpeeds.Count() != 0)
        {
            cpu.speedAverage = (int)_cpuSpeeds.Average();
        }
    }
}