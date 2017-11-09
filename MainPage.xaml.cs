using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using QuinCalc.Models;
using System.Collections.ObjectModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuinCalc
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public ObservableCollection<Todo> Todos = new ObservableCollection<Todo>(){
      new Todo() { Id = 1, Name = "Some Todo 1", IsDone = true },
      new Todo() { Id = 2, Name = "Some Todo 2", IsDone = false },
      new Todo() { Id = 3, Name = "Some Todo 3", IsDone = true },
      new Todo() { Id = 4, Name = "Some Todo 4", IsDone = false },
      new Todo() { Id = 5, Name = "Some Todo 5", IsDone = true },
      new Todo() { Id = 6, Name = "Some Todo 6", IsDone = false },
    };

    public MainPage()
    {
      this.InitializeComponent();
    }
  }
}
