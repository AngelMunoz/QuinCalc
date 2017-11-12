using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using QuinCalc.Models;
using System.Collections.ObjectModel;
using QuinCalc.ViewModels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Views;
using System.Diagnostics;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuinCalc
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public TodoViewModel UpNext { get; set; }
    public ExpenseViewModel UpNextExpense { get; set; }
    public ObservableCollection<TodoViewModel> Todos { get; set; }
    public ObservableCollection<ExpenseViewModel> Expenses { get; set; }
    public ExpenseViewModel NextQuin { get; set; }

    public int skipTodos = 0;
    public int limitTodos = 15;

    public int skipExpenses = 0;
    public int limitExpenses = 15;

    public MainPage()
    {
      this.InitializeComponent();
      this.LoadCollections();
    }

    /// <summary>
    /// Loads useful Data from the Database when called
    /// </summary>
    /// <returns></returns>
    private async Task LoadCollections()
    {
      using (var context = new QuincalcContext())
      {
        Todos = await GetTodoVMs(context);
        Expenses = await GetExpenseVMs(context);
        NextQuin = await GetNextQuin(context);
      }
      UpNext = GetUpnext(Todos);
      UpNextExpense = GetUpNextExpense(Expenses);
    }

    private static async Task<ExpenseViewModel> GetNextQuin(QuincalcContext context)
    {
      int dayToCheck = GetDayToCheck();
      decimal totalAmount = await GetTotalAmount(context, dayToCheck);
      var vm = new ExpenseViewModel
      {
        DueDate = GetNextQuin(dayToCheck),
        Amount = totalAmount
      };
      return vm;
    }

    /// <summary>
    /// Use this method to Change Page 
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    public async void ChangePage(string vm, int skip = 0, int limit = 15)
    {
      using (var context = new QuincalcContext())
      {
        switch (vm)
        {
          case "Todos":
            Todos = await GetTodoVMs(context, skip, limit);
            break;
          case "Expenses":
            Expenses = await GetExpenseVMs(context, skip, limit);
            break;
          default:
            await new Windows.UI.Popups.MessageDialog("Woops That Should't Have Happened").ShowAsync();
            break;
        }
      }
    }

    /// <summary>
    /// Get The closest Expense to *Today* (including Time)
    /// </summary>
    /// <param name="expenses"></param>
    /// <returns></returns>
    private ExpenseViewModel GetUpNextExpense(ObservableCollection<ExpenseViewModel> expenses)
    {
      return expenses.OrderBy(e => Math.Abs((e.DueDate - DateTime.Now).Ticks)).FirstOrDefault();
    }

    /// <summary>
    /// Get The Closest Todo according to the provided Date
    /// </summary>
    /// <param name="todos"></param>
    /// <returns></returns>
    private TodoViewModel GetUpnext(ObservableCollection<TodoViewModel> todos)
    {
      return todos.OrderBy(t => Math.Abs((t.DueDate - DateTime.Now).Ticks)).FirstOrDefault();
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
        .OrderBy(e => e.DueDate >= DateTime.Now && e.DueDate <= GetNextQuin(dayToCheck))
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
                .OrderByDescending(e => e.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return new ObservableCollection<ExpenseViewModel>(expenses.Select(e => new ExpenseViewModel(e)));
    }

    /// <summary>
    /// Load from the Database the Todo View Models Apply Skip + limit if necessary
    /// </summary>
    /// <param name="context"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private async Task<ObservableCollection<TodoViewModel>> GetTodoVMs(QuincalcContext context, int skip = 0, int limit = 15)
    {
      var todos = await context.Todos
                .OrderByDescending(t => t.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return new ObservableCollection<TodoViewModel>(todos.Select(t => new TodoViewModel(t)));
    }

    /// <summary>
    /// Navigate To Add Expense View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddExpense_Click(object sender, RoutedEventArgs e)
    {
      Frame.Navigate(typeof(ExpenseForm));
    }

    /// <summary>
    /// Navigate To Add Todo View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddTodo_Click(object sender, RoutedEventArgs e)
    {
      Frame.Navigate(typeof(TodoForm));
    }

    /// <summary>
    /// Handles Selection From expenses
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExpensesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (ExpensesList.SelectedItems.Count > 0)
      {
        DeleteBtn.Visibility = Visibility.Visible;
      }
      else
      {
        DeleteBtn.Visibility = Visibility.Collapsed;
      }

      if (TodosList.SelectedItems.Count == 0 && ExpensesList.SelectedItems.Count == 1)
      {
        EditBtn.Visibility = Visibility.Visible;
      }
      else
      {
        EditBtn.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Handles Selection From Todos
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TodosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (TodosList.SelectedItems.Count > 0)
      {
        DeleteBtn.Visibility = Visibility.Visible;
      }
      else
      {
        DeleteBtn.Visibility = Visibility.Collapsed;
      }

      if (TodosList.SelectedItems.Count == 1 && ExpensesList.SelectedItems.Count == 0)
      {
        EditBtn.Visibility = Visibility.Visible;
      }
      else
      {
        EditBtn.Visibility = Visibility.Collapsed;
      }
    }

    /// <summary>
    /// Handles Delete button action
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
      var selectedExpenses = ExpensesList.SelectedItems.ToList();
      var selectedTodos = TodosList.SelectedItems.ToList();
      await RemoveExpensesFromList(selectedExpenses);
      await RemoveTodosFromList(selectedTodos);
      UpNext = GetUpnext(Todos);
      UpNextExpense = GetUpNextExpense(Expenses);
      UpdateLayout();
    }

    /// <summary>
    /// Remove Expenses From List
    /// </summary>
    /// <param name="selectedExpenses"></param>
    /// <returns></returns>
    private async Task RemoveExpensesFromList(List<object> selectedExpenses)
    {
      if (selectedExpenses.Count > 0)
      {
        using (var context = new QuincalcContext())
        {
          foreach (ExpenseViewModel item in selectedExpenses)
          {
            context.Expenses.Remove(item);
            Expenses.Remove(item);
          }
          await context.SaveChangesAsync();
          NextQuin = await GetNextQuin(context);
        }
      }
    }

    /// <summary>
    /// Remove Todos From List
    /// </summary>
    /// <param name="selectedTodos"></param>
    /// <returns></returns>
    private async Task RemoveTodosFromList(List<object> selectedTodos)
    {
      if (selectedTodos.Count > 0)
      {
        using (var context = new QuincalcContext())
        {
          foreach (TodoViewModel item in selectedTodos)
          {
            context.Todos.Remove(item);
            Todos.Remove(item);
          }
          await context.SaveChangesAsync();
        }
      }
    }

    /// <summary>
    /// Handles Edit button action
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditBtn_Click(object sender, RoutedEventArgs e)
    {
      var selected = ExpensesList.SelectedItem != null ? ExpensesList.SelectedItem : TodosList.SelectedItem;
      if(selected.GetType().Name == "ExpenseViewModel")
      {
        Frame.Navigate(typeof(ExpenseForm), selected);
      } 
      else
      {
        Frame.Navigate(typeof(TodoForm), selected);
      }
    }
  }
}
