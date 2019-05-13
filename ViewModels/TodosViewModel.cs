using Caliburn.Micro;
using QuinCalc.Services;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class TodosViewModel : Conductor<TodosDetailViewModel>.Collection.OneActive
  {
    private int pageNum = 1;
    private int totalTodoCount = 0;

    public int PageNum { get => pageNum; private set => Set(ref pageNum, value); }
    public int TotalTodoCount { get => totalTodoCount; private set => Set(ref totalTodoCount, value); }

    public bool NextEnabled { get; set; }
    public bool BackEnabled { get; set; }
    public bool SaveEnabled { get; set; }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      base.OnInitializeAsync(cancellationToken);
      return LoadTodos();
    }

    private async Task LoadTodos(int page = 1, int limit = 5)
    {
      var skip = (page - 1) * limit;
      using (var todservice = new TodoService())
      {
        var (count, todos) = await todservice.Find(skip, limit);
        TotalTodoCount = count;
        Items.Clear();
        Items.AddRange(todos.Select(t => new TodosDetailViewModel(t)));
      }
      NextEnabled = skip <= TotalTodoCount;
      BackEnabled = skip >= limit;
      PageNum = page;
    }

    private async void CreateTodoBtn_Click()
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
      await LoadTodos();
    }

    private async void SaveBtn_Click()
    {
      SaveEnabled = false;
      //TodoVm current = SelectedItem as TodoVm;
      //using (var todservice = new TodoService())
      //{
      //  var success = await todservice.Update(current);
      //  if (!success)
      //  {
      //    // TODO: add unsuccessful code
      //    return;
      //  }
      //}
      await LoadTodos(PageNum);
      SaveEnabled = true;
    }

    private async void DeleteBtn_Click()
    {
      SaveEnabled = false;
      //TodoVm current = SelectedItem as TodoVm;
      //using (var todservice = new TodoService())
      //{
      //  var success = await todservice.Destroy(current);
      //  if (!success)
      //  {
      //    // TODO: add unsuccessful code
      //    return;
      //  }
      //}
      await LoadTodos(PageNum);
      SaveEnabled = true;
    }

    private async void BackBtn_Click()
    {
      await LoadTodos(PageNum - 1);
    }

    private async void NextBtn_Click()
    {
      await LoadTodos(PageNum + 1);
    }
  }
}
