using Caliburn.Micro;
using QuinCalc.ViewModels;
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
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
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
