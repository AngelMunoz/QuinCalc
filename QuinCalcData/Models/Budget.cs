using System;
using System.Collections.Generic;

namespace QuinCalcData.Models
{
  public class Budget
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Limit { get; set; }
    public decimal Warning { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    // Relationships

    public List<Income> Incomes { get; set; }
    public List<Expense> Expenses { get; set; }
  }
}
