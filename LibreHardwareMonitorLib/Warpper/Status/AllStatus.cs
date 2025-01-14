using System;
using static LibreHardwareMonitor.Warpper.Fans;

namespace LibreHardwareMonitor.Warpper;

public class AllStatus
{
    public CPU cpu = new();
    public GPU gpu = new();
    public Disk disk = new();
    public Memory memory = new();
    public Network network = new();
    public Fan[] fans = Array.Empty<Fan>();
}