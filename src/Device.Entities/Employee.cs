using System.ComponentModel.DataAnnotations.Schema;
namespace Device.Entities;

[Table("Employee")]
public class Employee
{
    public int Id { get; set; }
    public decimal Salary { get; set; }
    public int PositionId { get; set; }
    public int PersonId { get; set; }
    public DateTime HireDate { get; set; }

    public Position Position { get; set; } = null!;
    public Person Person { get; set; } = null!;
    public ICollection<DeviceEmployee> DeviceEmployees { get; set; } = new List<DeviceEmployee>();
}
