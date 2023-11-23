using System;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;

namespace LibreHardwareMonitor.Warpper;

public class Disk
{
    private static string _mainDiskName = string.Empty;
    public int load;
    public float used;
    public long total;
    public float activity;
    public int temperature;
    public float readSpeed;
    public float writeSpeed;

    // public int Load => load;
    // public float Used => used;
    // public long Total => total;
    // public float Activity => activity;
    // public int Temperature => temperature;
    // public float ReadSpeed => readSpeed;
    // public float WriteSpeed => writeSpeed;


    public Disk()
    {

    }
    public Disk(int load, float used, long total, float activity, int temperature, float readSpeed, float writeSpeed)
    {
        this.load = load;
        this.used = used;
        this.total = total;
        this.activity = activity;
        this.temperature = temperature;
        this.readSpeed = readSpeed;
        this.writeSpeed = writeSpeed;
    }

    public static void Update(Disk disk, IHardware hardware)
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

        if (disk.total == 0 && _mainDiskName.Equals(hardware.Name))
        {
            foreach (var deviceInfo in storage.DriveInfos)
            {
                if (!deviceInfo.IsReady)
                    continue;

                try
                {
                    disk.total += deviceInfo.TotalSize;
                    Logger.Debug($"update disk Disk Driver Total Size: name {deviceInfo.Name}; Total Size {deviceInfo.TotalSize}");

                }
                // catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            disk.total /= 1024 * 1024 * 1024;
            Logger.Debug($"Disk Driver Total Size: {disk.total} KB | {disk.total} G");
        }

        if (_mainDiskName.Equals(hardware.Name))
        {
            foreach (var sensor in hardware.Sensors)
            {
                if (SensorUtils.NameEquels(sensor, "Used Space") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    disk.load = (int)(sensor.Value ?? 0);
                    disk.used = disk.total / 100 * disk.load;
                    Logger.Debug($"disk load: {disk.load}%");
                    Logger.Debug($"disk used: {disk.used} GB");
                }
                else if (SensorUtils.NameEquels(sensor, "Temperature") && SensorUtils.TypeIsTemperature(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    disk.temperature = (int)(sensor.Value ?? 0);
                    Logger.Debug($"disk temperature: {disk.temperature}");
                }
                // TODO:
                else if (SensorUtils.NameEquels(sensor, "Total Activity") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    disk.activity = sensor.Value ?? 0;
                    Logger.Debug($"disk total activity: {disk.activity}");
                }
                else if (SensorUtils.NameEquels(sensor, "Read Rate") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    disk.readSpeed = (sensor.Value ?? 0) / 1024;
                    Logger.Debug($"disk read speed: {disk.readSpeed}");
                }
                else if (SensorUtils.NameEquels(sensor, "Write Rate") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                {
                    disk.writeSpeed = (sensor.Value ?? 0) / 1024;
                    Logger.Debug($"disk write speed: {disk.writeSpeed}");
                }
            }
        }
    }
}
