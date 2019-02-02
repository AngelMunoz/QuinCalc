using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.Services;
using QuinCalc.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Home : Page
  {
    public TodoVm UpNextTodo { get; set; }
    public ExpenseVm UpNextExpense { get; set; }
    public ExpenseVm UpNextBiweek { get; set; }

    public Home()
    {
      InitializeComponent();
      LoadUpComing();
    }

    /// <summary>
    /// Loads useful Data from the Database when called
    /// </summary>
    /// <returns></returns>
    private async void LoadUpComing()
    {
      var home = new HomeVm();
      using (var context = new QuincalcContext())
      {
        var dayToCheck = DateService.GetDayToCheck();
        var biweekAmount = await GetBiweekAmount(context, dayToCheck);
        var totalMonthly = await GetMonthlyAmount(context);

        home.UpNextBiweek = new ExpenseVm
        {
          DueDate = DateService.GetNextQuin(dayToCheck),
          Amount = biweekAmount
        };

        home.UpNextMonthly = new ExpenseVm
        {
          DueDate = DateService.GetNextQuin(DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month)),
          Amount = totalMonthly
        };

        using (var ExService = new ExpenseService())
        {
          var expense = await ExService.FindClosest();
          home.ShowExpense = expense == null ? false : true;
          home.UpNextExpense = new ExpenseVm(expense);
        }

        using (var TodService = new TodoService())
        {
          var todo = await TodService.FindClosest();
          home.ShowTodo = todo == null ? false : true;
          home.UpNextTodo = new TodoVm(todo);
        }
      }
      DataContext = home;
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
