using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class Memory
{
    public int load;
    public int used;
    public int total;
    public static int sTotal;

    public Memory() { }
    public Memory(int load, int used, int total)
    {
        this.load = load;
        this.used = used;
        this.total = total;
    }

    public static void initTotal(Computer computer)
    {
        if (sTotal != 0)
        {
            return;
        }
        // init Memeory (Total)
        SMBios sMBios = computer.SMBios;
        foreach (var memoryDevices in sMBios.MemoryDevices)
        {
            sTotal += memoryDevices.Size;
        }
        Logger.Debug($"Total Memory: {sTotal}"); // MB
    }

    public static void Update(Memory memory, IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (SensorUtils.NameEquels(sensor, "Memory") && SensorUtils.TypeIsLoad(sensor))
            {
                memory.load = (int)(sensor.Value ?? 0);
                Logger.Debug($"mem load: {memory.load}");
            }
            else if (SensorUtils.NameEquels(sensor, "Memory Used") && SensorUtils.TypeIsData(sensor))
            {
                memory.used = (int)((sensor.Value ?? 0) * 1024);
                Logger.Debug($"mem used: {memory.used}");
            }
        }
        memory.total = sTotal;
        Logger.Debug($"mem total: {memory.total}");
    }
}