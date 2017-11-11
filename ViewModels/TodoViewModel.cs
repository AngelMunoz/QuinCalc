using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuinCalc.Models;

namespace QuinCalc.ViewModels
{
  public class TodoViewModel : NotificationBase<Todo>
  {
    public TodoViewModel(Todo todo = null) : base(todo) { }

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

    public DateTime DueDate
    {
      get { return This.DueDate; }
      set { SetProperty(This.DueDate, value, () => This.DueDate = value); }
    }
  }
}
