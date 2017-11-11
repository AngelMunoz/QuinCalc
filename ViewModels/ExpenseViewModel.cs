using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuinCalc.Models;

namespace QuinCalc.ViewModels
{
  public class ExpenseViewModel : NotificationBase<Expense>
  {
    public ExpenseViewModel(Expense expense = null) : base(expense) { }

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
