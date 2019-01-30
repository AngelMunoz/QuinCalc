using Microsoft.EntityFrameworkCore;
using Windows.Storage;

namespace QuinCalc.Models
{
  public class QuincalcContext : DbContext
  {
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var folder = ApplicationData.Current.LocalFolder;
      optionsBuilder.UseSqlite($"Data Source={folder.Path}\\Quincalc.db");
    }

  }
}
