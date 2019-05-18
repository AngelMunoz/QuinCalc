using Microsoft.EntityFrameworkCore;

namespace QuinCalcData.Models
{
  public class QuincalcContext : DbContext
  {
    public readonly string dbsource;

    public QuincalcContext(string source = null): base()
    {
      dbsource = source ?? "Data Source=Quincalc.db";
    }

    public DbSet<Todo> Todos { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(dbsource);
    }

  }
}
