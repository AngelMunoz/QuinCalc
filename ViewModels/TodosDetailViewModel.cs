using Caliburn.Micro;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
