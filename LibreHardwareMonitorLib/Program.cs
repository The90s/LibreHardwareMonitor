using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace LibreHardwareMonitor
{

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
            Console.WriteLine("visitComputer: {0}", computer);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            Console.WriteLine("visitHardware: {0}", hardware.GetType);
            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
    // public static class Program
    public class Program
    {
        public static int foobar()
        {
            return 10;
        }

        static Computer computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true,
            IsBatteryEnabled = true,
            IsPsuEnabled = true,
        };
        static bool _init = false;

        static void ComputerInit()
        {
            if (!_init)
            {
                computer.Open();
                computer.Accept(new UpdateVisitor());
            }
            _init = true;
        }

        public async Task<object> FooBar(dynamic input)
        {
            ComputerInit();
            // computer.Accept(new UpdateVisitor());

            var stringBuilder = new StringBuilder(1024);
            foreach (IHardware hardware in computer.Hardware)
            {
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
    }
}
