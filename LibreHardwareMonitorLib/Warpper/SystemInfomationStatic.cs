using System;
using System.Collections;
using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class SystemInformationStatic
{
    private static bool _init = false;
    private static string _motherboard = null;
    private static string _cpu = null;
    private static string _superIO = null;
    private static string _memory = null;
    private static string _gpuNvidia = null;
    private static string _gpuAmd = null;
    private static string _storage = null;
    private static string _network = null;
    private static string _cooler = null; // ??
    private static string _embeddedController = null; // ??
    private static string _psu = null; // 电源
    private static string _battery = null; // 电池

    public static string MotherBoard => _motherboard;
    public static string Cpu => _cpu;
    public static string SuperIO = _superIO;
    private static string _gpuIntel;

    // TODO: 显示的是什么？用户要看的是什么
    public static string Memory { get { return _memory; } }
    public static string Gpu
    {
        get
        {
            if (_gpuNvidia != null)
            {
                return _gpuNvidia;
            }
            else if (_gpuAmd != null)
            {
                return _gpuAmd;
            }
            else
            {
                return _gpuIntel;
            }
        }
    }
    public static string Storage => _storage;
    public static string Network => _network;
    public static string Battery => _battery;

    public static void init(IList<IHardware> hardwares)
    {

        if (_init)
        {
            return;
        }

        foreach (IHardware hardware in hardwares)
        {
            switch (hardware.HardwareType)
            {
                case HardwareType.Motherboard: _motherboard = hardware.Name; break;
                case HardwareType.SuperIO: _superIO = hardware.Name; break;
                case HardwareType.Cpu: _cpu = hardware.Name; break;
                case HardwareType.Memory: _memory = hardware.Name; break;
                case HardwareType.GpuNvidia: _gpuNvidia = hardware.Name; break;
                case HardwareType.GpuAmd: _gpuAmd = hardware.Name; break;
                case HardwareType.GpuIntel: _gpuIntel = hardware.Name; break;
                case HardwareType.Storage: _storage = hardware.Name; break;
                case HardwareType.Network: _network = hardware.Name; break;
                case HardwareType.Cooler: _cooler = hardware.Name; break;
                case HardwareType.EmbeddedController: _embeddedController = hardware.Name; break;
                case HardwareType.Psu: _psu = hardware.Name; break;
                case HardwareType.Battery: _battery = hardware.Name; break;
            }

        }

        _init = true;
    }

}
