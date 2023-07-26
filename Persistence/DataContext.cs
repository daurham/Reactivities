using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  public class DataContext : DbContext // "DbContext" base class is a combination of the "Unit Of Work" and "Repository" patterns
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    // created a constructor

    public DbSet<Activity> Activities { get; set; } 
  }
  // Once this property is created, we want to notify our Program class
}