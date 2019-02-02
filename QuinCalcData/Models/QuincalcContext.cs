using Microsoft.EntityFrameworkCore;

namespace QuinCalcData.Models
{
  public class QuincalcContext : DbContext
  {
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Data Source=Quincalc.db");
    }

  }
}
