using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor.Warpper;

public class GPU
{
    // enum GPUType
    // {
    //     Nvidia,
    //     AMD,
    //     // TODO: 区分 Intel 集显和Intel 独显
    //     // 独显的还没有测试守
    //     Intel,
    // }
    // private static GPUType type = GPUType.Intel;

    // 先简单用着，后面可能
    private static bool _isGpuNvidia = false;
    private static bool _isGpuAmd = false;
    private static bool _isGpuIntel = false;

    public int load;
    public int temperature;
    public int speed;
    public int fan;
    public float power;
    // public int Load => load;
    // public int Temperature => temperature;
    // public int Fan => fan;

    public GPU() { }
    public GPU(int load, int temperature, int fan)
    {
        this.load = load;
        this.temperature = temperature;
        this.fan = fan;
    }

    internal static void InitType(Computer instance)
    {
        foreach (IHardware hardware in instance.Hardware)
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
    public static void Update(GPU gpu, IList<IHardware> hardwares, CPU cpu)
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
                    if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsLoad(sensor))
                    {
                        gpu.load = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Nvidia GPU load: {gpu.load}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsTemperature(sensor))
                    {
                        gpu.temperature = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Nvidia GPU temperature: {gpu.temperature}");
                    }
                    else if (SensorUtils.NameStartWith(sensor, "GPU Fan") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: 这里就取了一个, 同一个显卡的风扇转速可能差不多
                        gpu.fan = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Nvidia GPU Fan; Name: {sensor.Name}; Value : {gpu.fan}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Package") && SensorUtils.TypeIsPower(sensor))
                    {
                        gpu.power = (int)(sensor.Value ?? 0);
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsClock(sensor))
                    {
                        gpu.speed = (int)(sensor.Value ?? 0);
                    }
                }
            }
        }
        else if (_isGpuAmd)
        {
            foreach (IHardware hardware in hardwares)
            {
                if (hardware.HardwareType != HardwareType.GpuAmd)
                {
                    continue;
                }
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsLoad(sensor) && (int)sensor.Value != 0)
                    {
                        gpu.load = (int)(sensor.Value ?? 0);
                        Logger.Debug($"AMD GPU load: {gpu.load}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsTemperature(sensor))
                    {
                        gpu.temperature = (int)(sensor.Value ?? 0);
                        Logger.Debug($"AMD GPU temperature: {gpu.temperature}");
                    }
                    else if (SensorUtils.NameStartWith(sensor, "GPU Fan") && SensorUtils.TypeIsFan(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: 这里就取了一个, 同一个显卡的风扇转速可能差不多
                        gpu.fan = (int)(sensor.Value ?? 0);
                        // Note: 有可能没达到显示的启动触发历零界点，风扇不会启动, 风扇历速度就为0
                        // Radeon RX 580 Series (/gpu-amd/0) 获取不到
                        // |  +- GPU Fan        :        0        0        0 (/gpu-amd/0/fan/0)
                        Logger.Debug($"AMD GPU Fan; Name: {sensor.Name}, Value: {gpu.fan}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Package") && SensorUtils.TypeIsPower(sensor))
                    {
                        gpu.power = (int)(sensor.Value ?? 0);
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Core") && SensorUtils.TypeIsClock(sensor))
                    {
                        gpu.speed = (int)(sensor.Value ?? 0);
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
                    if (SensorUtils.NameEquels(sensor, "D3D 3D") && SensorUtils.TypeIsLoad(sensor) && SensorUtils.ValueIsNotNullAndZero(sensor))
                    {
                        // Note: D3D Video Decode 名字会有重复的；多加判断：数字是不是0
                        // |  +- D3D Video Decode :  1.67533        0  13.7776 (/gpu-intel-integrated/xxxxxx/load/9)   xxxxx
                        // |  +- D3D Video Decode :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/10)
                        gpu.load = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Intel GPU load: {gpu.load}");
                    }
                    else if (SensorUtils.NameEquels(sensor, "GPU Power") && SensorUtils.TypeIsPower(sensor))
                    {
                        gpu.power = (int)(sensor.Value ?? 0);
                        Logger.Debug($"Intel GPU power: {gpu.load}");
                    }
                }
            }

            // Note: 先设置集显频率为Intel的频率
            gpu.speed = cpu.speedAverage;
            // intel GPU的温度就CPU的温度
            gpu.temperature = (int)cpu.temperature;
            // intel GPU的风扇就CPU的风扇
            if (cpu.fans != null && cpu.fans.Length > 0)
            {
                gpu.fan = cpu.fanAverage;
            }
            Logger.Debug($"Intel GPU temperature: {gpu.temperature}");
            Logger.Debug($"Intel GPU Fan: {gpu.fan}");
        }
        Logger.Debug($"GPU temperature: {gpu.temperature}");
        Logger.Debug($"GPU Fan: {gpu.fan}");
    }

}