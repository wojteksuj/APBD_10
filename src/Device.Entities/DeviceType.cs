namespace Device.Entities;
using System.ComponentModel.DataAnnotations.Schema;


[Table("DeviceType")]
public class DeviceType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
