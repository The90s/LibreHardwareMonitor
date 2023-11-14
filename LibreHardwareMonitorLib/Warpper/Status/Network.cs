using System.Net;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class Network
{
    public float upload;
    public float download;
    // public float Upload => upload;
    // public float Download => download;

    public Network() { }
    public Network(float upload, float download)
    {
        this.upload = upload;
        this.download = download;
    }

    public static void Update(Network network, IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            // Console.WriteLine("Update network: name: {0}; id: {1}; value: {2}", sensor.Name, sensor.Identifier, sensor.Value);
            if (SensorUtils.NameEquels(sensor, "Upload Speed") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                // 转成KB/s
                network.upload = (float)sensor.Value / 1024;
                Logger.Debug($"network upload: {network.upload}");
            }
            else if (SensorUtils.NameStartWith(sensor, "Download Speed") && SensorUtils.TypeIsThroughput(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
            {
                // 转成KB/s
                network.download = (float)sensor.Value / 1024;
                Logger.Debug($"network download: {network.download}");
            }
        }

    }
}