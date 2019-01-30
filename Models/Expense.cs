using System;

namespace QuinCalc.Models
{
  public class Expense
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset DueDate { get; set; }
  }
}
