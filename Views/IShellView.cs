using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace QuinCalc.Views
{
  public interface IShellView
  {
    INavigationService CreateNavigationService(WinRTContainer container);

    WinUI.NavigationView GetNavigationView();

    Frame GetFrame();
  }
}
