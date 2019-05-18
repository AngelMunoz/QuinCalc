using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using QuinCalc.Core.Enums;
using QuinCalc.Core.Services;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private int pageLimit = 5;

    public int PageNum { get => _pageNum; set => Set(ref _pageNum, value); }
    public int PageLimit { get => pageLimit; set => Set(ref pageLimit, value); }
    public int TotalCount { get => _totalCount; set => Set(ref _totalCount, value); }
    public decimal TotalAmount { get => _totalAmount; set => Set(ref _totalAmount, value); }
    public bool ShowNextBtn { get => _showNextBtn; set => Set(ref _showNextBtn, value); }
    public bool ShowBackBtn { get => _showBackBtn; set => Set(ref _showBackBtn, value); }
    public LoadExpenseType CurrentLoadType { get => _currentLoadType; set => Set(ref _currentLoadType, value); }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      LoadExpenses();
      _ = LoadTotalAmountAsync();
      return base.OnInitializeAsync(cancellationToken);
    }

    private void LoadExpenses(int page = 1, int limit = 5, LoadExpenseType loadType = LoadExpenseType.All)
    {
      var skip = (page - 1) * limit;
      using (var exservice = new ExpenseService())
      {
        int count = 0;
        IEnumerable<Expense> expenses = null;
        switch (loadType)
        {
          case LoadExpenseType.All:
            (count, expenses) = exservice.Find(skip, limit);
            break;
          case LoadExpenseType.Done:
            (count, expenses) = exservice.FindByIsDone(true, skip, limit);
            break;
          case LoadExpenseType.NotDone:
            (count, expenses) = exservice.FindByIsDone(false, skip, limit);
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
        var success = await exservice.CreateAsync(new Expense() { DueDate = DateService.GetNextQuin() });
        if (!success)
        {
          Debug.WriteLine("Failed to Add Expense");
          return;
        }
      }
      await Task.WhenAll(Task.Run(() => LoadExpenses(PageNum, PageLimit, CurrentLoadType)), LoadTotalAmountAsync());
      Analytics.TrackEvent("Created Expense");
    }

    private async Task SaveExpenseAsync(ExpensesDetailViewModel args, ExpenseUpdateType type)
    {
      Analytics.TrackEvent("Saved Expense", new Dictionary<string, string> { { "Save Type", Enum.GetName(typeof(ExpenseUpdateType), type) } });
      var expense = args.Item;
      var result = false;
      using (var exservice = new ExpenseService())
      {
        switch (type)
        {
          case ExpenseUpdateType.Delete:
            result = await exservice.DestroyAsync(expense);
            break;
          case ExpenseUpdateType.MarkAsDone:
            expense.IsDone = true;
            result = await exservice.UpdateAsync(expense);
            break;
          case ExpenseUpdateType.MarkAsNotDone:
            expense.IsDone = false;
            result = await exservice.UpdateAsync(expense);
            break;
          case ExpenseUpdateType.Save:
            result = await exservice.UpdateAsync(expense);
            break;
        }
      }
      if (!result)
      {
        Debug.WriteLine("Couldn't update Expense");
      }
      await Task.Run(() => LoadExpenses(PageNum, PageLimit, CurrentLoadType));
    }

    private async Task LoadTotalAmountAsync()
    {
      using (var exservice = new ExpenseService())
      {
        TotalAmount = await exservice.GetTotalAmount(CurrentLoadType);
      }
    }

    private async void BackBtn_Click()
    {
      Analytics.TrackEvent("Navigated Expense", new Dictionary<string, string> { { "Direction", "Back" } });
      await Task.Run(() => LoadExpenses(PageNum - 1, PageLimit, loadType: CurrentLoadType));
    }

    private async void NextBtn_Click()
    {
      Analytics.TrackEvent("Navigated Expense", new Dictionary<string, string> { { "Direction", "Forwards" } });
      await Task.Run(() => LoadExpenses(PageNum + 1, PageLimit, loadType: CurrentLoadType));
    }

    private async void HideDoneCheck_Checked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "Not Done" } });
      CurrentLoadType = LoadExpenseType.NotDone;
      await Task.WhenAll(Task.Run(() => LoadExpenses(PageNum, PageLimit, loadType: CurrentLoadType)), LoadTotalAmountAsync());
    }

    private async void HideDoneCheck_Unchecked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "All" } });
      CurrentLoadType = LoadExpenseType.All;
      await Task.WhenAll(Task.Run(() => LoadExpenses(PageNum, PageLimit, loadType: CurrentLoadType)), LoadTotalAmountAsync());
    }
  }
}
