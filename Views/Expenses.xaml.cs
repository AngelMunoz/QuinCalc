using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Expenses : Page
  {
    public ObservableCollection<ExpenseViewModel> ExpensesList = new ObservableCollection<ExpenseViewModel>();

    public int skipExpenses = 0;
    public int limitExpenses = 15;

    public Expenses()
    {
      InitializeComponent();
      LoadExpenses();
      CheckViewState();
    }

    private void CheckViewState()
    {
      switch (MDView.ViewState)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          break;
      }
    }

    private async void LoadExpenses()
    {
      ExpensesList.Clear();
      using (var context = new QuincalcContext())
      {
        var expenses = await GetExpenseVMs(context);
        foreach (var expense in expenses)
        {
          ExpensesList.Add(expense);
        }
      }

      if (ExpensesList.Count < limitExpenses)
      {
        NextBtn.IsEnabled = false;
      }
      else
      {
        NextBtn.IsEnabled = true;
      }

      if (skipExpenses < 15)
      {
        BackBtn.IsEnabled = false;
      }
      else
      {
        BackBtn.IsEnabled = true;
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

    private async void CreateExpenseBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      using (var context = new QuincalcContext())
      {
        await context.Expenses.AddAsync(new Expense() { DueDate = DateTime.Now });
        await context.SaveChangesAsync();
      }
      LoadExpenses();
    }

    private void SaveBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      ExpenseViewModel current = MDView.SelectedItem as ExpenseViewModel;
      using (var context = new QuincalcContext())
      {
        context.Expenses.Update(current);
        context.SaveChangesAsync();
      }
      SaveBtn.IsEnabled = true;
    }

    private void DeleteBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      ExpenseViewModel current = MDView.SelectedItem as ExpenseViewModel;
      using (var context = new QuincalcContext())
      {
        context.Expenses.Remove(current);
        context.SaveChangesAsync();
      }
      SaveBtn.IsEnabled = true;
      LoadExpenses();
    }

    private void MDView_ViewStateChanged(object sender, MasterDetailsViewState e)
    {
      switch (e)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          break;
      }
    }

    private void BackBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void NextBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {

    }
  }
}
