namespace Device.Logic;
using Device.Entities;
using Device.Logic.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class DeviceService : IDeviceService
{
    private readonly DeviceContext _context;

    public DeviceService(DeviceContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        return await _context.Devices
            .Select(d => new DeviceDto
            {
                Id = d.Id.ToString(),
                Name = d.Name
            })
            .ToListAsync();
    }

    public async Task<DeviceDetailsDto?> GetByIdAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees)
                .ThenInclude(de => de.Employee)
                    .ThenInclude(e => e.Person)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null) return null;

        var props = JsonConvert.DeserializeObject<Dictionary<string, object>>(device.AdditionalProperties)
                    ?? new Dictionary<string, object>();

        var currentEmployee = device.DeviceEmployees
            .Where(de => de.ReturnDate == null)
            .Select(de => new EmployeeDto
            {
                Id = de.Employee.Id.ToString(),
                FullName = $"{de.Employee.Person.FirstName} {de.Employee.Person.MiddleName} {de.Employee.Person.LastName}",
            })
            .FirstOrDefault();

        return new DeviceDetailsDto
        {
            DeviceTypeName = device.DeviceType?.Name ?? "Unknown",
            IsEnabled = device.IsEnabled,
            AdditionalProperties = props,
            CurrentEmployee = currentEmployee
        };
    }

    public async Task<int> CreateAsync(CreateDeviceDto dto)
    {
        var type = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (type == null) throw new ArgumentException("Invalid device type");

        var rawProps = dto.AdditionalProperties.ToDictionary(
            kvp => kvp.Key,
            kvp => JsonSerializer.Deserialize<object>(kvp.Value.GetRawText())!
        );

        var device = new Entities.Device
        {
            Name = dto.Name,
            IsEnabled = dto.IsEnabled,
            DeviceTypeId = type.Id,
            AdditionalProperties = JsonConvert.SerializeObject(rawProps)
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (device == null) return false;

        var type = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (type == null) throw new ArgumentException("Invalid device type");

        var rawProps = dto.AdditionalProperties.ToDictionary(
            kvp => kvp.Key,
            kvp => JsonSerializer.Deserialize<object>(kvp.Value.GetRawText())!
        );

        device.DeviceTypeId = type.Id;
        device.IsEnabled = dto.IsEnabled;
        device.AdditionalProperties = JsonConvert.SerializeObject(rawProps);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.DeviceEmployees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null) return false;

        _context.DeviceEmployees.RemoveRange(device.DeviceEmployees);
        _context.Devices.Remove(device);

        await _context.SaveChangesAsync();
        return true;
    }
}
