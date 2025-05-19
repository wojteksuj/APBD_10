using System.Text.Json;

public class CreateDeviceDto
{
    public string Name { get; set; } = null!;
    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
}