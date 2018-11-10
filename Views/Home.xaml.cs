using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Home : Page
  {
    public TodoViewModel UpNext { get; set; }
    public ExpenseViewModel UpNextExpense { get; set; }
    public ExpenseViewModel NextQuin { get; set; }

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
      using (var context = new QuincalcContext())
      {
        int dayToCheck = GetDayToCheck();
        decimal totalAmount = await GetTotalAmount(context, dayToCheck);
        NextQuin = new ExpenseViewModel
        {
          DueDate = GetNextQuin(dayToCheck),
          Amount = totalAmount
        };
        UpNext = await GetUpnext(context);
        UpNextExpense = await GetUpNextExpense(context);
      }

      if (UpNext.Name == null)
      {
        UpNextTodoPanel.Visibility = Visibility.Collapsed;
      } else
      {
        UpNextTodoPanel.Visibility = Visibility.Visible;
      }

      if(UpNextExpense.Name == null)
      {
        UpNextExpensePanel.Visibility = Visibility.Collapsed;
      } else
      {
        UpNextExpensePanel.Visibility = Visibility.Visible;
      }

      if(NextQuin.Amount <= 0 && UpNextExpense.Name == null && UpNext.Name == null)
      {
        ClosestExpensePanel.Visibility = Visibility.Collapsed;
        NoExpensesPanel.Visibility = Visibility.Visible;
      } else
      {
        NoExpensesPanel.Visibility = Visibility.Collapsed;
        ClosestExpensePanel.Visibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// Get The closest Expense to *Today* (including Time)
    /// </summary>
    /// <param name="expenses"></param>
    /// <returns></returns>
    private async Task<ExpenseViewModel> GetUpNextExpense(QuincalcContext context)
    {
      var expense = await context.Expenses.OrderBy(e => Math.Abs((e.DueDate - DateTime.Now).Ticks)).FirstOrDefaultAsync();
      return new ExpenseViewModel(expense);
    }

    /// <summary>
    /// Get The Closest Todo according to the provided Date
    /// </summary>
    /// <param name="todos"></param>
    /// <returns></returns>
    private async Task<TodoViewModel> GetUpnext(QuincalcContext context)
    {
      var todo = await context.Todos
        .Where(t => !t.IsDone)
        .OrderBy(e => Math.Abs((e.DueDate - DateTime.Now).Ticks))
        .FirstOrDefaultAsync();
      return new TodoViewModel(todo);
    }

    /// <summary>
    /// Get the closest day to pay the expenses (either day 15 or last day of the month)
    /// </summary>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    private static DateTime GetNextQuin(int dayToCheck)
    {
      return new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayToCheck);
    }

    /// <summary>
    /// Get the Total Amount to pay on the NextQuin (day 15 or last day of the current month)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    private static async Task<decimal> GetTotalAmount(QuincalcContext context, int dayToCheck)
    {
      return await context.Expenses
        .Where(e => (e.DueDate.Day >= DateTime.Now.Day) && (e.DueDate.Day <= GetNextQuin(dayToCheck).Day))
        .SumAsync(e => e.Amount);
    }

    /// <summary>
    /// Gets the Next Day to Check (either 15 or last day of the month)
    /// </summary>
    /// <returns></returns>
    private static int GetDayToCheck()
    {
      return DateTime.Now.Day < 15 ? 15 : DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
    }
  }
}
