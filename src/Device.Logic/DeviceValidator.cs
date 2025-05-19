using Device.Logic.Models;

namespace Device.Logic;

public class DeviceValidator : IDeviceValidator
{
    public string? ValidateDevice(CreateDeviceDto dto)
    {
        if (dto == null)
            return "Device data is missing.";

        if (string.IsNullOrWhiteSpace(dto.DeviceTypeName))
            return "Device type must be provided.";

        if (string.IsNullOrWhiteSpace(dto.Name))
            return "Device name must be provided.";

        if (dto.AdditionalProperties == null)
            return "Additional properties are required.";

        return null; 
    }
}