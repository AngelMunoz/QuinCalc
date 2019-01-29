using System;
using QuinCalc.Helpers;

namespace QuinCalc.ViewModels
{
  public class Home
  {
    public TodoVm UpNextTodo { get; set; }
    public ExpenseVm UpNextExpense { get; set; }
    public ExpenseVm UpNextBiweek { get; set; }
  }

  public class HomeVm : NotificationBase<Home>
  {
    public HomeVm(Home home = null) : base(home) { }
    public HomeVm(TodoVm todo, ExpenseVm expense, ExpenseVm biweek, Home home = null) : base(home)
    {
      UpNextTodo = todo;
      UpNextExpense = expense;
      UpNextBiweek = biweek;
    }

    public TodoVm UpNextTodo
    {
      get { return This.UpNextTodo; }
      set { SetProperty(This.UpNextTodo, value, () => This.UpNextTodo = value); }
    }

    public ExpenseVm UpNextExpense
    {
      get { return This.UpNextExpense; }
      set { SetProperty(This.UpNextExpense, value, () => This.UpNextExpense = value); }
    }

    public ExpenseVm UpNextBiweek
    {
      get { return This.UpNextBiweek; }
      set { SetProperty(This.UpNextBiweek, value, () => This.UpNextBiweek = value); }
    }
  }
}
