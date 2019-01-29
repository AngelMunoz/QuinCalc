using System;
using QuinCalc.Helpers;
using QuinCalc.Models;

namespace QuinCalc.ViewModels
{
  public class ExpenseVm : NotificationBase<Expense>
  {
    public ExpenseVm(Expense expense = null) : base(expense)
    {
      DueDate = DateTime.UtcNow;
    }

    public string Name
    {
      get { return This.Name; }
      set { SetProperty(This.Name, value, () => This.Name = value); }
    }

    public decimal Amount
    {
      get { return This.Amount; }
      set { SetProperty(This.Amount, value, () => This.Amount = value); }
    }

    public DateTime DueDate
    {
      get { return This.DueDate; }
      set { SetProperty(This.DueDate, value, () => This.DueDate = value); }
    }
  }
}
