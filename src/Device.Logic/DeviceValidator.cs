
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Device.Logic;

public class DeviceValidator : IDeviceValidator
{
    public string? ValidateDevice(CreateDeviceDto dto)
    {
        switch (dto.DeviceTypeName.ToLowerInvariant())
        {
            case "pc":
                return ValidatePC(dto.AdditionalProperties);

            case "smartwatch":
                return ValidateSmartwatch(dto.AdditionalProperties);

            case "embedded":
                return ValidateEmbedded(dto.AdditionalProperties);
            case "monitor":
            case "printer":
                return null;
            default:
                return "Unsupported device type.";
        }
    }

    private string? ValidatePC(object additional)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<PersonalComputerProps>(additional.ToString()!);
            if (string.IsNullOrWhiteSpace(data?.OperatingSystem))
                return "Cannot turn on PersonalComputer without operating system.";
        }
        catch
        {
            return "Invalid structure for PersonalComputer properties.";
        }

        return null;
    }

    private string? ValidateSmartwatch(object additional)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<SmartwatchProps>(additional.ToString()!);
            if (data == null)
                return "Invalid Smartwatch data.";

            if (data.BatteryLevel < 0 || data.BatteryLevel > 100)
                return "Battery level must be between 0 and 100.";

            if (data.BatteryLevel < 11)
                return "Battery level too low to turn on (must be at least 11%).";
        }
        catch
        {
            return "Invalid structure for Smartwatch properties.";
        }

        return null;
    }

    private string? ValidateEmbedded(object additional)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<EmbeddedProps>(additional.ToString()!);
            if (data == null)
                return "Invalid Embedded data.";

            if (!IsValidIp(data.IpAddress))
                return "Invalid IP address format.";

            if (string.IsNullOrEmpty(data.NetworkName) || !data.NetworkName.Contains("MD Ltd."))
                return "Network name must contain 'MD Ltd.'.";
        }
        catch
        {
            return "Invalid structure for Embedded properties.";
        }

        return null;
    }

    private bool IsValidIp(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return false;

        var regex = new Regex(@"^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}$");
        return regex.IsMatch(ip);
    }
}
