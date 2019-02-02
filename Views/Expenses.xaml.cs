using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalcData.Models;
using QuinCalc.Services;
using QuinCalc.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Expenses : Page
  {
    public ObservableCollection<ExpenseVm> ExpensesList = new ObservableCollection<ExpenseVm>();
    public int PageNum { get; private set; } = 1;
    public int TotalExpenseCount { get; private set; } = 0;
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
          AddMobileBtn.Visibility = Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Visibility.Collapsed;
          break;
      }
    }

    private async void LoadExpenses(int page = 1, int limit = 5)
    {
      var skip = (page - 1) * limit;
      using (var exservice = new ExpenseService())
      {
        var (count, expenses) = await exservice.Find(skip, limit);
        TotalExpenseCount = count;
        ExpensesList.Clear();
        foreach (var expense in expenses)
        {
          ExpensesList.Add(new ExpenseVm(expense));
        }
      }
      NextBtn.IsEnabled = skip <= TotalExpenseCount;
      BackBtn.IsEnabled = skip >= limit;
      PageNum = page;
    }

    private async void CreateExpenseBtn_Click(object sender, RoutedEventArgs e)
    {
      using (var exservice = new ExpenseService())
      {
        var success = await exservice.Create(new Expense() { DueDate = DateService.GetNextQuin() });
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      LoadExpenses();
    }

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      ExpenseVm current = MDView.SelectedItem as ExpenseVm;
      using (var exservice = new ExpenseService())
      {
        var success = await exservice.Update(current);
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      LoadExpenses(PageNum);
      SaveBtn.IsEnabled = true;
    }

    private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      ExpenseVm current = MDView.SelectedItem as ExpenseVm;
      using (var exservice = new ExpenseService())
      {
        var success = await exservice.Destroy(current);
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      SaveBtn.IsEnabled = true;
      LoadExpenses(PageNum);
    }

    private void MDView_ViewStateChanged(object sender, MasterDetailsViewState e)
    {
      switch (e)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Visibility.Collapsed;
          break;
      }
    }

    private void BackBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadExpenses(PageNum - 1);
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadExpenses(PageNum + 1);
    }
  }
}
