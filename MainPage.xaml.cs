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
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuinCalc
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public TodoViewModel UpNext { get; set; }
    public ObservableCollection<TodoViewModel> Todos { get; set; }
    public ObservableCollection<ExpenseViewModel> Expenses { get; set; }
    private readonly DbContext db;
    public MainPage()
    {
      this.InitializeComponent();
      this.LoadCollections();
    }

    private async Task LoadCollections()
    {
      using (var context = new QuincalcContext())
      {
        var todos = await context.Todos.ToListAsync();
        var todovms = todos.Select(t => new TodoViewModel(t));
        Todos = new ObservableCollection<TodoViewModel>(todovms);

        var expenses = await context.Expenses.ToListAsync();
        var expensevms = expenses.Select(e => new ExpenseViewModel(e));
        Expenses = new ObservableCollection<ExpenseViewModel>(expensevms);
      }

      UpNext = Todos
        .OrderBy(t => Math.Abs((t.DueDate - DateTime.Now).Ticks))
        .FirstOrDefault();
    }

    private void AddExpense_Click(object sender, RoutedEventArgs e)
    {
    }

    private void AddTodo_Click(object sender, RoutedEventArgs e)
    {
      Frame.Navigate(typeof(TodoForm));
    }
  }
}
