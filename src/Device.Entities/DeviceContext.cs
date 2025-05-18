using Microsoft.EntityFrameworkCore;

namespace Device.Entities;

public class DeviceContext : DbContext
{
    public DeviceContext(DbContextOptions<DeviceContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<DeviceType> DeviceTypes { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<Person> People { get; set; } = null!;
    public DbSet<DeviceEmployee> DeviceEmployees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Device>()
            .HasOne(d => d.DeviceType)
            .WithMany(dt => dt.Devices)
            .HasForeignKey(d => d.DeviceTypeId);
        
        modelBuilder.Entity<DeviceEmployee>()
            .HasOne(de => de.Device)
            .WithMany(d => d.DeviceEmployees)
            .HasForeignKey(de => de.DeviceId);
        
        modelBuilder.Entity<DeviceEmployee>()
            .HasOne(de => de.Employee)
            .WithMany(e => e.DeviceEmployees)
            .HasForeignKey(de => de.EmployeeId);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Person)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PersonId);
    }
}