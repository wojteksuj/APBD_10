namespace Device.Entities;
using System.ComponentModel.DataAnnotations.Schema;

[Table("DeviceEmployee")]
public class DeviceEmployee
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Device Device { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
