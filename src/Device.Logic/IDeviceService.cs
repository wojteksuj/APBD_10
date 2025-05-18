namespace Device.Logic;
using Models;



public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDetailsDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDeviceDto dto);
    Task<bool> UpdateAsync(int id, UpdateDeviceDto dto);
    Task<bool> DeleteAsync(int id);
}