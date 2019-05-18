using Caliburn.Micro;
using QuinCalcData.Models;

namespace QuinCalc.ViewModels
{
  public class TodosDetailViewModel : Screen
  {
    public TodosDetailViewModel(Todo item)
    {
      Item = item;
    }

    public Todo Item { get; }
  }
}
