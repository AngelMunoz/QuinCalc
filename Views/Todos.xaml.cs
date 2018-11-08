using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Todos : Page
  {
    public ObservableCollection<TodoViewModel> TodosList = new ObservableCollection<TodoViewModel>();
    public int skipTodos = 0;
    public int limitTodos = 15;

    public Todos()
    {
      InitializeComponent();
      LoadTodos();
    }

    private async void LoadTodos()
    {
      using (var context = new QuincalcContext())
      {
        TodosList = await GetTodoVMs(context);
      }
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
                .OrderBy(t => t.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return new ObservableCollection<TodoViewModel>(todos.Select(t => new TodoViewModel(t)));
    }

    /// <summary>
    /// Remove Todos From List
    /// </summary>
    /// <param name="selectedTodos"></param>
    /// <returns></returns>
    private async Task RemoveTodo(List<object> selectedTodos)
    {
      throw new NotImplementedException();
    }
  }
}
