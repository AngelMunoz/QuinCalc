using Caliburn.Micro;
using QuinCalc.ViewModels;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace QuinCalc.Views
{
  public sealed partial class ShellPage : IShellView
  {
    private ShellViewModel ViewModel => DataContext as ShellViewModel;

    public ShellPage()
    {
      InitializeComponent();
    }

    public INavigationService CreateNavigationService(WinRTContainer container)
    {
      var navigationService = container.RegisterNavigationService(shellFrame);
      return navigationService;
    }

    public WinUI.NavigationView GetNavigationView()
    {
      return navigationView;
    }

    public Frame GetFrame()
    {
      return shellFrame;
    }
  }
}
