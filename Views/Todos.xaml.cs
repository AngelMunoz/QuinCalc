using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Collections.ObjectModel;
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
  public sealed partial class Todos : Page
  {
    public ObservableCollection<TodoVm> TodosList = new ObservableCollection<TodoVm>();
    public int skipTodos = 0;
    public int limitTodos = 15;

    public Todos()
    {
      InitializeComponent();
      LoadTodos();
      CheckViewState();
    }

    private void CheckViewState()
    {
      switch (MDView.ViewState)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          break;
      }
    }

    private async void LoadTodos()
    {
      TodosList.Clear();
      using (var context = new QuincalcContext())
      {
        var todos = await GetTodoVMs(context);
        foreach (var todo in todos)
        {
          TodosList.Add(todo);
        }
      }

      if (TodosList.Count < limitTodos)
      {
        NextBtn.IsEnabled = false;
      }
      else
      {
        NextBtn.IsEnabled = true;
      }

      if (skipTodos < 15)
      {
        BackBtn.IsEnabled = false;
      }
      else
      {
        BackBtn.IsEnabled = true;
      }
    }

    /// <summary>
    /// Load from the Database the Todo View Models Apply Skip + limit if necessary
    /// </summary>
    /// <param name="context"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private async Task<ObservableCollection<TodoVm>> GetTodoVMs(QuincalcContext context, int skip = 0, int limit = 15)
    {
      var todos = await context.Todos
                .OrderBy(t => t.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return new ObservableCollection<TodoVm>(todos.Select(t => new TodoVm(t)));
    }

    private async void CreateTodoBtn_Click(object sender, RoutedEventArgs e)
    {
      using (var context = new QuincalcContext())
      {
        await context.Todos.AddAsync(new Todo());
        await context.SaveChangesAsync();
      }
      LoadTodos();
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      TodoVm current = MDView.SelectedItem as TodoVm;
      using (var context = new QuincalcContext())
      {
        context.Todos.Update(current);
        context.SaveChangesAsync();
      }
      SaveBtn.IsEnabled = true;
    }

    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      TodoVm current = MDView.SelectedItem as TodoVm;
      using (var context = new QuincalcContext())
      {
        context.Todos.Remove(current);
        context.SaveChangesAsync();
      }
      SaveBtn.IsEnabled = true;
      LoadTodos();
    }

    private void MDView_ViewStateChanged(object sender, MasterDetailsViewState e)
    {
      switch (e)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
          break;
      }
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BackBtn_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
