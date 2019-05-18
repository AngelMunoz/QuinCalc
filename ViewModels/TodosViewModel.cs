using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using QuinCalc.Core.Enums;
using QuinCalc.Core.Services;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class TodosViewModel : Conductor<TodosDetailViewModel>.Collection.OneActive
  {
    private int pageNum = 1;
    private int totalTodoCount = 0;
    private int pageLimit = 5;
    private bool nextEnabled;
    private bool backEnabled;
    private bool saveEnabled;
    private LoadExpenseType currentLoadType = LoadExpenseType.All;

    public int PageNum { get => pageNum; private set => Set(ref pageNum, value); }
    public int TotalTodoCount { get => totalTodoCount; private set => Set(ref totalTodoCount, value); }
    public int PageLimit { get => pageLimit; set => Set(ref pageLimit, value); }

    public bool NextEnabled { get => nextEnabled; set => Set(ref nextEnabled, value); }
    public bool BackEnabled { get => backEnabled; set => Set(ref backEnabled, value); }
    public bool SaveEnabled { get => saveEnabled; set => Set(ref saveEnabled, value); }

    public LoadExpenseType CurrentLoadType { get => currentLoadType; set => Set(ref currentLoadType, value); }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      LoadTodos();
      return base.OnInitializeAsync(cancellationToken);
    }

    private void LoadTodos(int page = 1, int limit = 5, LoadExpenseType loadType = LoadExpenseType.All)
    {
      var skip = (page - 1) * limit;
      using (var todservice = new TodoService())
      {
        int count = 0;
        IEnumerable<Todo> todos = null;
        switch (loadType)
        {
          case LoadExpenseType.All:
            (count, todos) = todservice.Find(skip, limit);
            break;
          case LoadExpenseType.Done:
            (count, todos) = todservice.FindByIsDone(true, skip, limit);
            break;
          case LoadExpenseType.NotDone:
            (count, todos) = todservice.FindByIsDone(false, skip, limit);
            break;
        }
        TotalTodoCount = count;
        Items.Clear();
        Items.AddRange(todos.Select(t => new TodosDetailViewModel(t)));
      }
      NextEnabled = skip <= TotalTodoCount;
      BackEnabled = skip >= limit;
      PageNum = page;
    }

    private async Task SaveTodoAsync(TodosDetailViewModel args, TodoUpdateType type)
    {
      Analytics.TrackEvent("Saved Todo", new Dictionary<string, string> { { "Save Type", Enum.GetName(typeof(TodoUpdateType), type) } });
      var expense = args.Item;
      var result = false;
      using (var exservice = new TodoService())
      {
        switch (type)
        {
          case TodoUpdateType.Delete:
            result = await exservice.DestroyAsync(expense);
            break;
          case TodoUpdateType.MarkAsDone:
            expense.IsDone = true;
            result = await exservice.UpdateAsync(expense);
            break;
          case TodoUpdateType.MarkAsNotDone:
            expense.IsDone = false;
            result = await exservice.UpdateAsync(expense);
            break;
          case TodoUpdateType.Save:
            result = await exservice.UpdateAsync(expense);
            break;
        }
      }
      if(!result)
      {
        Debug.WriteLine("Failed to Update Todo");
      }
      await Task.Run(() => LoadTodos(PageNum, PageLimit, CurrentLoadType));
    }

    private async void CreateTodoAsync()
    {
      using (var todservice = new TodoService())
      {
        var success = await todservice.CreateAsync(new Todo { DueDate = DateService.GetNextQuin() });
        if (!success)
        {
          Debug.WriteLine("Failed to Add Todo");
          return;
        }
      }
      Analytics.TrackEvent("Created Todo");
      await Task.Run(() => LoadTodos(PageNum, PageLimit));
    }

    private async void BackBtn_Click()
    {
      Analytics.TrackEvent("Navigated Todo", new Dictionary<string, string> { { "Direction", "Back" } });
      await Task.Run(() => LoadTodos(PageNum - 1, PageLimit, CurrentLoadType));
    }

    private async void NextBtn_Click()
    {
      Analytics.TrackEvent("Navigated Todo", new Dictionary<string, string> { { "Direction", "Forwards" } });
      await Task.Run(() => LoadTodos(PageNum + 1, PageLimit, CurrentLoadType));
    }

    private async void HideDoneCheck_Checked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "Not Done" } });
      CurrentLoadType = LoadExpenseType.NotDone;
      await Task.Run(() => LoadTodos(PageNum, PageLimit, CurrentLoadType));
    }

    private async void HideDoneCheck_Unchecked()
    {
      Analytics.TrackEvent("Filtered Expenses", new Dictionary<string, string> { { "Load Type", "All" } });
      CurrentLoadType = LoadExpenseType.All;
      await Task.Run(() => LoadTodos(PageNum, PageLimit, CurrentLoadType));
    }
  }
}
