namespace Device.Logic.Models;

public class MonitorProps
{
    public List<PortDto> Ports { get; set; } = new();
}

public class PortDto
{
    public string Type { get; set; }
    public string Version { get; set; }
}
