using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Services;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      using (var context = new QuincalcContext())
      {
        var dayToCheck = DateService.GetDayToCheck();
        var biweekAmount = await GetBiweekAmount(context, dayToCheck);
        var totalMonthly = await GetMonthlyAmount(context);

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

        using (var ExService = new ExpenseService())
        {
          var expense = await ExService.FindClosest();
          ShowExpense = expense == null ? false : true;
          UpNextExpense = expense;
        }

        using (var TodService = new TodoService())
        {
          var todo = await TodService.FindClosest();
          ShowTodo = todo == null ? false : true;
          UpNextTodo = todo;
        }
      }

    }

    /// <summary>
    /// Get the Total Amount to pay on the NextQuin (day 15 or last day of the current month)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    private static async Task<decimal> GetBiweekAmount(QuincalcContext context, int dayToCheck)
    {
      return await context.Expenses
        .Where(e =>
          (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
          (e.DueDate.Day <= DateService.GetNextQuin(dayToCheck).Day)
         )
        .SumAsync(e => e.Amount);
    }

    /// <summary>
    /// Get the Total Amount to pay on the rest of the month (last day of the current month)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task<decimal> GetMonthlyAmount(QuincalcContext context)
    {
      return await context.Expenses
        .Where(e =>
          (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
          (e.DueDate.Day <= DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month))
         )
        .SumAsync(e => e.Amount);
    }
  }
}
