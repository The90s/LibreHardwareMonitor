namespace LibreHardwareMonitor;

public class Fan
{

    // public FanType _type;
    public string name;
    public int value;
    public int max;

    public Fan(string name, int value, int max)
    {
        this.name = name;
        this.value = value;
        this.max = max;
    }
    public override string ToString()
    {
        return $"Fan: Name={name}; Value={value}; Max:{max}";
    }
}