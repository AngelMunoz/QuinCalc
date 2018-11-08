using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Expenses : Page
  {
    public ObservableCollection<ExpenseViewModel> ExpensesList = new ObservableCollection<ExpenseViewModel>();

    public Expenses()
    {
      InitializeComponent();
      LoadExpenses();
    }

    private async void LoadExpenses()
    {
      using (var context = new QuincalcContext())
      {
        ExpensesList = await GetExpenseVMs(context);
      }
    }

    /// <summary>
    /// Load from the Database the Expense View Models Apply Skip + limit if necessary
    /// </summary>
    /// <param name="context"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private async Task<ObservableCollection<ExpenseViewModel>> GetExpenseVMs(QuincalcContext context, int skip = 0, int limit = 15)
    {
      var expenses = await context.Expenses
                .OrderBy(e => e.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return new ObservableCollection<ExpenseViewModel>(expenses.Select(e => new ExpenseViewModel(e)));
    }

    /// <summary>
    /// Remove Expenses From List
    /// </summary>
    /// <param name="selectedExpenses"></param>
    /// <returns></returns>
    private async Task RemoveExpenses(List<object> selectedExpenses)
    {
      throw new NotImplementedException();
    }
  }
}
