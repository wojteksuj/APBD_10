using System.ComponentModel.DataAnnotations.Schema;
namespace Device.Entities;

[Table("Position")]

public class Position
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int MinExpYears { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
