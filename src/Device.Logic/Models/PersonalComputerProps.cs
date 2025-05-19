using Newtonsoft.Json;

public class PersonalComputerProps
{
    [JsonProperty("operationSystem")] 
    public string? OperatingSystem { get; set; }
}