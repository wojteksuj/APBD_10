using Device.Logic.Models;

namespace Device.Logic;

public interface IDeviceValidator
{
    string? ValidateDevice(CreateDeviceDto device);
}