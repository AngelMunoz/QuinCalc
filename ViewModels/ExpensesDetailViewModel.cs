using Caliburn.Micro;
using QuinCalcData.Models;

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
