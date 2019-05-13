using Caliburn.Micro;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class ExpensesDetailViewModel : Screen
  {
    public ExpensesDetailViewModel(Expense item)
    {
      Item = item;
    }

    public Expense Item { get; }
  }
}
