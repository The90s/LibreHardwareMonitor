using System;

namespace LibreHardwareMonitor;

public class StatusClasses
{

    public enum FanType
    {
        // /lpc/nct6798d/fan/0
        Normal,
        // /lpc/nct6798d/control/0
        Contorl,
    }

    public class FanStatus
    {

        private FanType _type;
        private string _name;
        private int? _value;
        private int? _min;
        private int? _max;

        public FanStatus(string name, int? value, int? min, int? max)
        {
            _name = name;
            _value = value;
            _min = min;
            _max = max;
        }

        string Name { get => _name; }
        int? Value { get => _value; }
        int? Max { get => _max; }
        int? Min { get => _min; }
    }

    public class NetworkStatus
    {
        public float upload;
        public float download;
        public NetworkStatus(float upload, float download)
        {
            this.upload = upload;
            this.download = download;
        }
    }

    public class CpuStatus
    {
        public int load;
        public int temperature;
        public int fan;

        public CpuStatus(int load, int temperature, int fan)
        {
            this.load = load;
            this.temperature = temperature;
            this.fan = fan;
        }
    }

    public class GpuStatus
    {
        public int load;
        public int temperature;
        public int fan;

        public GpuStatus(int load, int temperature, int fan)
        {
            this.load = load;
            this.temperature = temperature;
            this.fan = fan;
        }
    }


    public class MemStatus
    {
        public int load;
        public int used;
        public int total;

        public MemStatus(int load, int used, int total)
        {
            this.load = load;
            this.used = used;
            this.total = total;
        }
    }
    public class DiskStatus
    {
        public int load;
        public float used;
        public long total;
        public float activity;
        public int temperature;
        public float readSpeed;
        public float writeSpeed;

        public DiskStatus(int load, float used, long total, float activity, int temperature, float readSpeed, float writeSpeed)
        {
            this.load = load;
            this.used = used;
            this.total = total;
            this.activity = activity;
            this.temperature = temperature;
            this.readSpeed = readSpeed;
            this.writeSpeed = writeSpeed;
        }
    }
}