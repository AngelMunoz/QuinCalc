using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalcData.Models;
using QuinCalc.Services;
using QuinCalc.ViewModels;
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
    public int PageNum { get; private set; } = 1;
    public int TotalTodoCount { get; private set; } = 0;

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
          AddMobileBtn.Visibility = Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Visibility.Collapsed;
          break;
      }
    }

    private async void LoadTodos(int page = 1, int limit = 5)
    {
      var skip = (page - 1) * limit;
      using (var todservice = new TodoService())
      {
        var (count, todos) = await todservice.Find(skip, limit);
        TotalTodoCount = count;
        TodosList.Clear();
        foreach (var todo in todos)
        {
          TodosList.Add(new TodoVm(todo));
        }
      }
      NextBtn.IsEnabled = skip <= TotalTodoCount;
      BackBtn.IsEnabled = skip >= limit;
      PageNum = page;
    }

    private async void CreateTodoBtn_Click(object sender, RoutedEventArgs e)
    {
      using (var todservice = new TodoService())
      {
        var success = await todservice.Create(new Todo { DueDate = DateService.GetNextQuin() });
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      LoadTodos();
    }

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      TodoVm current = MDView.SelectedItem as TodoVm;
      using (var todservice = new TodoService())
      {
        var success = await todservice.Update(current);
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      LoadTodos(PageNum);
      SaveBtn.IsEnabled = true;
    }

    private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
      SaveBtn.IsEnabled = false;
      TodoVm current = MDView.SelectedItem as TodoVm;
      using (var todservice = new TodoService())
      {
        var success = await todservice.Destroy(current);
        if (!success)
        {
          // TODO: add unsuccessful code
          return;
        }
      }
      LoadTodos(PageNum);
      SaveBtn.IsEnabled = true;
    }

    private void MDView_ViewStateChanged(object sender, MasterDetailsViewState e)
    {
      switch (e)
      {
        case MasterDetailsViewState.Master:
          AddMobileBtn.Visibility = Visibility.Visible;
          break;
        default:
          AddMobileBtn.Visibility = Visibility.Collapsed;
          break;
      }
    }

    private void BackBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadTodos(PageNum - 1);
    }

    private void NextBtn_Click(object sender, RoutedEventArgs e)
    {
      LoadTodos(PageNum + 1);
    }
  }
}
