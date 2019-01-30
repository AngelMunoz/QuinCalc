using System;
using QuinCalc.Helpers;
using QuinCalc.Models;

namespace QuinCalc.ViewModels
{
  public class TodoVm : NotificationBase<Todo>
  {
    public TodoVm(Todo todo = null) : base(todo)
    {
    }

    public string Name
    {
      get { return This.Name; }
      set { SetProperty(This.Name, value, () => This.Name = value); }
    }

    public bool IsDone
    {
      get { return This.IsDone; }
      set { SetProperty(This.IsDone, value, () => This.IsDone = value); }
    }

    public string Description
    {
      get { return This.Description; }
      set { SetProperty(This.Description, value, () => This.Description = value); }
    }

    public DateTimeOffset DueDate
    {
      get { return This.DueDate; }
      set { SetProperty(This.DueDate, value, () => This.DueDate = value); }
    }
  }
}
