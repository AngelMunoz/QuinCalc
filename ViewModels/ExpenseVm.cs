using System;
using QuinCalc.Helpers;
using QuinCalc.Models;

namespace QuinCalc.ViewModels
{
  public class ExpenseVm : NotificationBase<Expense>
  {
    public ExpenseVm(Expense expense = null) : base(expense)
    {
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

    public DateTimeOffset DueDate
    {
      get { return This.DueDate; }
      set { SetProperty(This.DueDate, value, () => This.DueDate = value); }
    }
  }
}
