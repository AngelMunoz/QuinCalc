using System.Collections.Generic;
using System.Collections.ObjectModel;
using QuinCalc.Enums;
using QuinCalc.Services;
using QuinCalc.ViewModels;
using QuinCalcData.Models;
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
    public LoadExpenseType CurrentLoadType { get; set; } = LoadExpenseType.All;
    public ExpensePageVm ExpensePage { get; set; }

    public Expenses()
    {
      InitializeComponent();
      ExpensePage = new ExpensePageVm
      {
        PageNum = 1,
        ShowBackBtn = false,
        ShowNextBtn = false,
        TotalAmount = 0,
        TotalCount = 0
      };
      Loaded += Expenses_Loaded;
      ExpenseDataGrid.OnExpenseUpdate += ExpenseDataGrid_OnExpenseUpdate;
    }

    private void Expenses_Loaded(object sender, RoutedEventArgs e)
    {
      LoadExpenses();
      LoadTotalAmount();
    }

    private async void LoadExpenses(int page = 1, int limit = 20, LoadExpenseType loadType = LoadExpenseType.All)
    {
      var skip = (page - 1) * limit;
      using (var exservice = new ExpenseService())
      {
        int count = 0;
        List<Expense> expenses = null;
        switch (loadType)
        {
          case LoadExpenseType.All:
            (count, expenses) = await exservice.Find(skip, limit);
            break;
          case LoadExpenseType.Done:
            (count, expenses) = await exservice.FindByIsDone(true, skip, limit);
            break;
          case LoadExpenseType.NotDone:
            (count, expenses) = await exservice.FindByIsDone(false, skip, limit);
            break;
        }
        ExpensePage.TotalCount = count;
        ExpensePage.ShowNextBtn = skip <= ExpensePage.TotalCount;
        ExpensePage.ShowBackBtn = skip >= limit;
        ExpensesList.Clear();
        foreach (var expense in expenses)
        {
          ExpensesList.Add(new ExpenseVm(expense));
        }
      }
      ExpensePage.PageNum = page;
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
      LoadTotalAmount();
    }

    private async void ExpenseDataGrid_OnExpenseUpdate(object sender, (ExpenseVm, ExpenseUpdateType) e)
    {
      var (expense, updateType) = e;
      using (var exservice = new ExpenseService())
      {
        switch (updateType)
        {
          case ExpenseUpdateType.Delete:
            await exservice.Destroy(expense);
            break;
          case ExpenseUpdateType.MarkAsDone:
            expense.IsDone = true;
            await exservice.Update(expense);
            break;
          case ExpenseUpdateType.MarkAsNotDone:
            expense.IsDone = false;
            await exservice.Update(expense);
            break;
          case ExpenseUpdateType.Save:
            await exservice.Update(expense);
            break;
        }
      }
      LoadExpenses(ExpensePage.PageNum, loadType: CurrentLoadType);
      LoadTotalAmount();
    }

    private async void LoadTotalAmount()
    {
      using (var exservice = new ExpenseService())
      {
        ExpensePage.TotalAmount = await exservice.GetTotalAmount(CurrentLoadType);
      }
    }

    private void BackBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadExpenses(ExpensePage.PageNum - 1, loadType: CurrentLoadType);
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadExpenses(ExpensePage.PageNum + 1, loadType: CurrentLoadType);
    }

    private void HideDoneCheck_Checked(object sender, RoutedEventArgs e)
    {
      CurrentLoadType = LoadExpenseType.NotDone;
      LoadExpenses(ExpensePage.PageNum, loadType: CurrentLoadType);
      LoadTotalAmount();
    }

    private void HideDoneCheck_Unchecked(object sender, RoutedEventArgs e)
    {
      CurrentLoadType = LoadExpenseType.All;
      LoadExpenses(ExpensePage.PageNum, loadType: CurrentLoadType);
      LoadTotalAmount();
    }
  }
}
