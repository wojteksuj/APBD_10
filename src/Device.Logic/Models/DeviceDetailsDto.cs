public class DeviceDetailsDto
{
    public string DeviceTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
    public EmployeeDto? CurrentEmployee { get; set; }
}