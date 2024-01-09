using Microsoft.EntityFrameworkCore;

namespace RestDesign.Data;

public class BillingContext : DbContext
{
  public BillingContext(DbContextOptions opt) : base(opt)
  {

  }

  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<Ticket> Tickets => Set<Ticket>();
  public DbSet<Employee> Employees => Set<Employee>();
  public DbSet<Project> Projects => Set<Project>();

  protected override void OnModelCreating(ModelBuilder bldr)
  {
    bldr.Entity<Customer>()
      .HasData(
        Fakers.CustomerFaker.Generate(100)
      );

    bldr.Entity<Employee>()
      .HasData(
        Fakers.EmployeeFaker.Generate(10)
      );

    bldr.Entity<Project>()
      .HasData(
        Fakers.ProjectFaker.Generate(25)
        );

    bldr.Entity<Ticket>()
      .HasData(
        Fakers.TicketFaker.Generate(100)
        );
  }

  protected override void OnConfiguring(DbContextOptionsBuilder bldr)
  {
    base.OnConfiguring(bldr);

    bldr.UseInMemoryDatabase("BillingDb");
  }

  public async Task ClearDatabaseAsync()
  {
    await Database.EnsureDeletedAsync();
    await Database.EnsureCreatedAsync();
  }

  public async Task<bool> SaveAllAsync()
  {
    try
    {
      return await SaveChangesAsync() > 0;
    }
    catch (Exception)
    {
      return false;
    }
  }
}
