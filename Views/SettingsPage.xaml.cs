using QuinCalc.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class SettingsPage : Page
  {
    public SettingsPage()
    {
      InitializeComponent();
    }

    private SettingsViewModel ViewModel
    {
      get { return DataContext as SettingsViewModel; }
    }
  }
}
