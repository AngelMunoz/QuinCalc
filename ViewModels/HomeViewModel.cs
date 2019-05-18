using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Core.Services;
using QuinCalcData.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class HomeViewModel : ViewModelBase
  {
    private Todo upNextTodo;
    private Expense upNextExpense;
    private Expense upNextBiweek;
    private Expense upNextMonthly;
    private bool showExpense;
    private bool showTodo;

    public Todo UpNextTodo { get => upNextTodo; set => Set(ref upNextTodo, value); }
    public Expense UpNextExpense { get => upNextExpense; set => Set(ref upNextExpense, value); }
    public Expense UpNextBiweek { get => upNextBiweek; set => Set(ref upNextBiweek, value); }
    public Expense UpNextMonthly { get => upNextMonthly; set => Set(ref upNextMonthly, value); }
    public bool ShowExpense { get => showExpense; set => Set(ref showExpense, value); }
    public bool ShowTodo { get => showTodo; set => Set(ref showTodo, value); }

    public HomeViewModel(INavigationService navigationService) : base(navigationService)
    {

    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      base.OnInitializeAsync(cancellationToken);
      return LoadUpComing();
    }

    private async Task LoadUpComing()
    {
      var dayToCheck = DateService.GetDayToCheck();
      using (var ExService = new ExpenseService())
      {
        var expense = await ExService.FindClosest();
        var biweekAmount = await ExService.GetBiweekAmount(dayToCheck);
        var totalMonthly = await ExService.GetMonthlyAmount();
        ShowExpense = expense == null ? false : true;
        UpNextExpense = expense;
        UpNextBiweek = new Expense
        {
          DueDate = DateService.GetNextQuin(dayToCheck),
          Amount = biweekAmount
        };

        UpNextMonthly = new Expense
        {
          DueDate = DateService.GetNextQuin(DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month)),
          Amount = totalMonthly
        };
      }

      using (var TodService = new TodoService())
      {
        var todo = await TodService.FindClosestAsync();
        ShowTodo = todo == null ? false : true;
        UpNextTodo = todo;
      }
    }
  }
}
