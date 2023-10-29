using System;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class ComputerReportsWarpper
{
    public static string GetReports() => ComputerSingleton.Instance.GetReport();

    public static string GetMethodboardReport() => GetReport(HardwareType.Motherboard);

    public static string GetCpuReport() => GetReport(HardwareType.Cpu);

    public static string GetGPUReport() => GetReport(HardwareType.GpuNvidia) + "\n" + GetReport(HardwareType.GpuAmd) + "\n" + GetReport(HardwareType.GpuIntel);

    public static string GetMemoryReport() => GetReport(HardwareType.Memory);

    public static string GetStorageReport() => GetReport(HardwareType.Storage);

    public static string GetNetworkReport() => GetReport(HardwareType.Network);

    public static string GetReport(HardwareType type)
    {
        StringBuilder stringBuilder = new();
        foreach (IHardware hardware in ComputerSingleton.Hardwares)
        {
            if (hardware.HardwareType == type)
            {
                stringBuilder.Append(hardware.GetReport());
            }

        }

        return stringBuilder.ToString();
    }
}
