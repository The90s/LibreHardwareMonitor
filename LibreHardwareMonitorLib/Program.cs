using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor
{

    public class Program
    {
        private readonly ushort _interval = 1000;
        static void LibreHardwareMonitorInit(ushort ms)
        {
            ComputerSingleton.Init();
            SystemInfomationStatic.init(ComputerSingleton.Hardwares);
            // TODO: start timer with ms
            // update info: network、CPU load、GPU load、temp
        }

        public async Task<object> FooBar(dynamic input)
        {
            LibreHardwareMonitorInit(_interval);
            // computer.Accept(new UpdateVisitor());

            var stringBuilder = new StringBuilder(1024);
            foreach (IHardware hardware in ComputerSingleton.Hardwares)
            {
                if (hardware.HardwareType == HardwareType.Motherboard)
                    hardware.Update();
                Console.WriteLine($"foreach name: {hardware.Name}; id: {hardware.Identifier}; ToString: {hardware.ToString()}; type: {hardware.GetType}");
                ReportHardwareSensorTree(hardware, stringBuilder, "|  ");
                //     Console.WriteLine("Hardware: {0}", hardware.Name);
            }
            // return computer.GetReport();
            return stringBuilder.ToString();

        }

        private static void ReportHardwareSensorTree(IHardware hardware, StringBuilder stringBuilder, string space)
        {
            stringBuilder.Append(String.Format("\r{0}|", space));
            stringBuilder.Append(String.Format("\r{0}+- {1} ({2})\r", space, hardware.Name, hardware.Identifier));
            // Console.WriteLine("{0}|", space);
            // Console.WriteLine("{0}+- {1} ({2})", space, hardware.Name, hardware.Identifier);

            ISensor[] sensors = hardware.Sensors;
            //Array.Sort(sensors, CompareSensor);

            foreach (ISensor sensor in sensors)
                stringBuilder.Append(String.Format("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier));
            // Console.WriteLine("{0}|  +- {1,-14} : {2,8:G6} {3,8:G6} {4,8:G6} ({5})", space, sensor.Name, sensor.Value, sensor.Min, sensor.Max, sensor.Identifier);

            foreach (IHardware subHardware in hardware.SubHardware)
                ReportHardwareSensorTree(subHardware, stringBuilder, "|  ");
        }


        public static string GetMotherboard()
        {
            return SystemInfomationStatic.MotherBoard;
        }
        public static string GetGPU()
        {
            return SystemInfomationStatic.Gpu;
        }

    }
}
