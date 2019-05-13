using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalc.Enums;
using QuinCalc.Services;
using QuinCalc.ViewModels;
using QuinCalcData.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class ExpensesPage : Page
  {
    public ExpensesPage()
    {
      InitializeComponent();
    }

    private ExpensesViewModel ViewModel
    {
      get { return DataContext as ExpensesViewModel; }
    }

    private void MasterDetailsViewControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (MasterDetailsViewControl.ViewState == MasterDetailsViewState.Both)
      {
        ViewModel.ActiveItem = ViewModel.Items.FirstOrDefault();
      }
    }
  }
}
