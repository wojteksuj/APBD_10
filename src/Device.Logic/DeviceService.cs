namespace Device.Logic;
using Device.Entities;
using Device.Logic.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



public class DeviceService : IDeviceService
{
    private readonly DeviceContext _context;
    private readonly IDeviceValidator _validator;

    public DeviceService(DeviceContext context, IDeviceValidator validator)
    {
        _context = context;
        _validator = validator;
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

        object props;
        try
        {
            var typeName = device.DeviceType?.Name.ToLower();
            var json = JObject.Parse(device.AdditionalProperties);

            props = device.DeviceType?.Name.ToLower() switch
            {
                "smartwatch" => JsonConvert.DeserializeObject<SmartwatchProps>(device.AdditionalProperties) ?? new object(),
                "personalcomputer" => JsonConvert.DeserializeObject<PersonalComputerProps>(device.AdditionalProperties) ?? new object(),
                "embedded" => JsonConvert.DeserializeObject<EmbeddedProps>(device.AdditionalProperties) ?? new object(),
                "monitor" => JsonConvert.DeserializeObject<MonitorProps>(device.AdditionalProperties) ?? new object(),
                "printer" => JsonConvert.DeserializeObject<PrinterProps>(device.AdditionalProperties) ?? new object(),
                _ => new object()
            };

        }
        catch
        {
            props = new object();
        }

        var currentEmployee = device.DeviceEmployees
            .Where(de => de.ReturnDate == null)
            .Select(de => new EmployeeDto
            {
                Id = de.Employee.Id.ToString(),
                FullName = de.Employee.Person.FirstName + " " + de.Employee.Person.MiddleName + " " + de.Employee.Person.LastName,
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
        var error = _validator.ValidateDevice(dto);
        if (error != null) throw new ArgumentException(error);

        var type = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (type == null) throw new ArgumentException("Invalid device type");

        var device = new Device
        {
            Name = dto.DeviceTypeName,
            IsEnabled = dto.IsEnabled,
            DeviceTypeId = type.Id,
            AdditionalProperties = JsonConvert.SerializeObject(dto.AdditionalProperties)
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var error = _validator.ValidateDevice(dto);
        if (error != null) throw new ArgumentException(error);

        var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (device == null) return false;

        var type = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (type == null) throw new ArgumentException("Invalid device type");

        device.DeviceTypeId = type.Id;
        device.IsEnabled = dto.IsEnabled;
        device.AdditionalProperties = JsonConvert.SerializeObject(dto.AdditionalProperties);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.DeviceEmployees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device == null) return false;

        _context.DeviceEmployees.RemoveRange(device.DeviceEmployees); // 👈 Remove related records
        _context.Devices.Remove(device);

        await _context.SaveChangesAsync();
        return true;
    }
}
