using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuinCalc.ViewModels
{
  public class ViewModelBase : Screen
  {
    protected readonly INavigationService PageNavigationService;

    protected ViewModelBase(INavigationService pageNavigationService)
    {
      PageNavigationService = pageNavigationService;
    }

    public bool CanGoBack
    {
      get { return PageNavigationService.CanGoBack; }
    }

    protected void NavigateTo<T>()
    {
      PageNavigationService.Navigate<T>();
    }

    public void GoBack()
    {
      PageNavigationService.GoBack();
    }
  }
}
