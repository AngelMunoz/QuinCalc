using Microsoft.Toolkit.Uwp.UI.Controls;
using QuinCalc.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class TodosPage : Page
  {
    public TodosPage()
    {
      InitializeComponent();
    }

    public TodosViewModel ViewModel { get => DataContext as TodosViewModel; }

    private void MasterDetailsViewControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (MasterDetailsViewControl.ViewState == MasterDetailsViewState.Both)
      {
        ViewModel.ActiveItem = ViewModel.Items.FirstOrDefault();
      }
    }

  }
}
