using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public sealed class ComputerSingleton
{

    private static Computer instance = new Computer
    {
        IsCpuEnabled = true,
        IsGpuEnabled = true,
        IsMemoryEnabled = true,
        IsMotherboardEnabled = true,
        IsControllerEnabled = true,
        IsNetworkEnabled = true,
        IsStorageEnabled = true,
        // 电池
        IsBatteryEnabled = true,
        // 电源
        IsPsuEnabled = true,
    };

    private ComputerSingleton() { }
    private static readonly object _lock = new();
    private static bool _isOpen = false;
    public static bool IsOpen => _isOpen;
    public static Computer Instance
    {
        get
        {
            if (!_isOpen)
            {
                instance.Open();

            }
            return instance;
        }
    }

    public static IList<IHardware> Hardwares
    {
        get
        {
            if (!_isOpen)
            {
                instance.Open();

            }
            return instance.Hardware;
        }
    }

    public static void Init()
    {
        if (!_isOpen)
        {
            instance.Open();
            instance.Accept(new UpdateVisitor());
            _isOpen = true;
        }
    }

    public static void Reset()
    {
        // Note: 这个库初始化有bug，有时候获取不到主板下面的风扇信息（大概六七次会遇到一次）
        // 所以这里有一个提供一个重置的函数
        Logger.Debug("computer reset");
        instance = new Computer()
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true,
            // 电池
            IsBatteryEnabled = true,
            // 电源
            IsPsuEnabled = true,
        };
        instance.Open();
        instance.Accept(new UpdateVisitor());

    }
}

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}
