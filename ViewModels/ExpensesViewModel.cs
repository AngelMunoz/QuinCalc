using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using QuinCalc.Enums;
using QuinCalc.Services;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class ExpensesViewModel : Conductor<ExpensesDetailViewModel>.Collection.OneActive
  {

    private int _pageNum;
    private int _totalCount;
    private decimal _totalAmount;
    private bool _showNextBtn;
    private bool _showBackBtn;
    private LoadExpenseType _currentLoadType;

    public int PageNum { get => _pageNum; set => Set(ref _pageNum, value); }
    public int TotalCount { get => _totalCount; set => Set(ref _totalCount, value); }
    public decimal TotalAmount { get => _totalAmount; set => Set(ref _totalAmount, value); }
    public bool ShowNextBtn { get => _showNextBtn; set => Set(ref _showNextBtn, value); }
    public bool ShowBackBtn { get => _showBackBtn; set => Set(ref _showBackBtn, value); }
    public LoadExpenseType CurrentLoadType { get => _currentLoadType; set => Set(ref _currentLoadType, value); }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      base.OnInitializeAsync(cancellationToken);
      return Task.WhenAll(LoadExpenses(), LoadTotalAmount());
    }

    private async Task LoadExpenses(int page = 1, int limit = 20, LoadExpenseType loadType = LoadExpenseType.All)
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
        TotalCount = count;
        ShowNextBtn = skip <= TotalCount;
        ShowBackBtn = skip >= limit;
        Items.Clear();
        Items.AddRange(expenses.Select(e => new ExpensesDetailViewModel(e)));
      }
      PageNum = page;
    }

    private async Task CreateExpenseAsync()
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
      await Task.WhenAll(LoadExpenses(), LoadTotalAmount());
      Analytics.TrackEvent("Created Expense");
    }

    private async Task SaveExpenseAsync(ExpensesDetailViewModel args, ExpenseUpdateType type)
    {
      var expense = args.Item;
      using (var exservice = new ExpenseService())
      {
        switch (type)
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
      await Task.WhenAll(LoadExpenses(PageNum, loadType: CurrentLoadType), LoadTotalAmount());
      Analytics.TrackEvent("Saved Expense", new Dictionary<string, string> { { "Save Type", Enum.GetName(typeof(ExpenseUpdateType), type) } });
    }

    private async Task LoadTotalAmount()
    {
      using (var exservice = new ExpenseService())
      {
        TotalAmount = await exservice.GetTotalAmount(CurrentLoadType);
      }
    }
    
    private async void BackBtn_Click()
    {
      Analytics.TrackEvent("Navigated Expense", new Dictionary<string, string> { { "Direction", "Back" } });
      await LoadExpenses(PageNum - 1, loadType: CurrentLoadType);
    }

    private async void NextBtn_Click()
    {
      Analytics.TrackEvent("Navigated Expense", new Dictionary<string, string> { { "Direction", "Forwards" } });
      await LoadExpenses(PageNum + 1, loadType: CurrentLoadType);
    }

    private async void HideDoneCheck_Checked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "Not Done" } });
      CurrentLoadType = LoadExpenseType.NotDone;
      await Task.WhenAll(LoadExpenses(PageNum, loadType: CurrentLoadType), LoadTotalAmount());
    }

    private async void HideDoneCheck_Unchecked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "All" } });
      CurrentLoadType = LoadExpenseType.All;
      await Task.WhenAll(LoadExpenses(PageNum, loadType: CurrentLoadType), LoadTotalAmount());
    }
  }
}
