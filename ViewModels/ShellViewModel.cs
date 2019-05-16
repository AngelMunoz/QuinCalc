using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using QuinCalc.Helpers;
using QuinCalc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;
namespace QuinCalc.ViewModels
{
  public class ShellViewModel : ViewModelBase
  {
    private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
    private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

    private readonly WinRTContainer _container;
    private static INavigationService _navigationService;
    private WinUI.NavigationView _navigationView;
    private bool _isBackEnabled;
    private WinUI.NavigationViewItem _selected;

    public ShellViewModel(WinRTContainer container, INavigationService pageNavigationService): base(pageNavigationService)
    {
      _container = container;
    }
    public bool IsBackEnabled
    {
      get { return _isBackEnabled; }
      set { Set(ref _isBackEnabled, value); }
    }

    public WinUI.NavigationViewItem Selected
    {
      get { return _selected; }
      set { Set(ref _selected, value); }
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      
      var view = GetView() as IShellView;

      _navigationService = view?.CreateNavigationService(_container);
      _navigationView = view?.GetNavigationView();

      if (_navigationService != null)
      {
        _navigationService.NavigationFailed += (sender, e) =>
        {
          throw e.Exception;
        };
        _navigationService.Navigated += NavigationService_Navigated;
        _navigationView.BackRequested += OnBackRequested;
      }
      return base.OnInitializeAsync(cancellationToken);
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      if (GetView() is UIElement page)
      {
        page.KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
        page.KeyboardAccelerators.Add(_backKeyboardAccelerator);
      }
    }

    private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
    {
      if (args.IsSettingsInvoked)
      {
        Analytics.TrackEvent("Navigated To", new Dictionary<string, string> { { "Page", typeof(SettingsPage).ToString() } });
        _navigationService.Navigate(typeof(SettingsPage));
        return;
      }

      var item = _navigationView.MenuItems
                      .OfType<WinUI.NavigationViewItem>()
                      .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
      var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
      Analytics.TrackEvent("Navigated To", new Dictionary<string, string> { { "Page", pageType.ToString() } });
      var viewModelType = ViewModelLocator.LocateTypeForViewType(pageType, false);
      _navigationService.NavigateToViewModel(viewModelType);
    }

    private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
    {
      Analytics.TrackEvent("Go Back");
      _navigationService.GoBack();
    }

    private void NavigationService_Navigated(object sender, NavigationEventArgs e)
    {
      IsBackEnabled = _navigationService.CanGoBack;
      if (e.SourcePageType == typeof(SettingsPage))
      {
        Selected = _navigationView.SettingsItem as WinUI.NavigationViewItem;
        return;
      }

      Selected = _navigationView.MenuItems
                      .OfType<WinUI.NavigationViewItem>()
                      .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
    }

    private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
    {
      var sourceViewModelType = ViewModelLocator.LocateTypeForViewType(sourcePageType, false);
      var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
      var viewModelType = ViewModelLocator.LocateTypeForViewType(pageType, false);
      return viewModelType == sourceViewModelType;
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
      var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
      if (modifiers.HasValue)
      {
        keyboardAccelerator.Modifiers = modifiers.Value;
      }

      keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
      return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
      if (_navigationService.CanGoBack)
      {
        _navigationService.GoBack();
        args.Handled = true;
      }
    }
  }
}
