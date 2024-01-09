using Microsoft.EntityFrameworkCore;
using RestDesign.Data.Fakers;

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
    var customers = CustomerFaker.Generate();
    var employees = EmployeeFaker.Generate();
    var projects = ProjectFaker.Generate();
    var tickets = TicketFaker.Generate();

    bldr.Entity<Customer>()
      .HasData(
        customers
      );

    bldr.Entity<Employee>()
      .HasData(
        employees
      );

    bldr.Entity<Project>()
      .HasData(
        projects
        );

    bldr.Entity<Ticket>()
      .HasData(
        tickets
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
