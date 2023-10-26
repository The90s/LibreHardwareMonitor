using System;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor;

public class SystemInformationDynamic
{
    private static Computer _computer = null;
    private static Task _updateTask = null;

    private static ushort _cpuTemp = 0;
    private static ushort _gpuLoad = 0;
    private static ushort _gpuTemp = 0;
    private static ushort _networkUp = 0;
    private static ushort _networkDown = 0;

    SystemInformationDynamic(Computer computer, ushort ms)
    {
        // _computer = computer;
        // _updateTask = new Task(Update, ms, TaskCreationOptions.LongRunning);
        // _updateTask.Start();
    }

    public static void Update() {
        if (_computer == null) {
            return;
        }

        foreach(IHardware hardware in _computer.Hardware) {
            hardware.Update();
        }
    }

    public static void stopTask() {
        if (_updateTask != null) {
        }
    }
}
