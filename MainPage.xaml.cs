using System;
using System.Collections.Generic;
using System.Linq;
using QuinCalc.Views;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuinCalc
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {

    public MainPage()
    {
      InitializeComponent();
    }

    private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
    {
      ("Home", typeof(Home)),
      ("Expenses", typeof(Expenses)),
      ("Todos", typeof(Todos)),
    };

    private void Nav_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      ContentFrame.Navigated += On_Navigated;
      // NavView doesn't load any page by default, so load home page.
      Nav.SelectedItem = Nav.MenuItems[0];
      // If navigation occurs on SelectionChanged, this isn't needed.
      // Because we use ItemInvoked to navigate, we need to call Navigate
      // here to load the home page.
      NavView_Navigate("Home", new EntranceNavigationTransitionInfo());

      // Add keyboard accelerators for backwards navigation.
      var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
      goBack.Invoked += BackInvoked;
      KeyboardAccelerators.Add(goBack);

      // ALT routes here
      var altLeft = new KeyboardAccelerator
      {
        Key = VirtualKey.Left,
        Modifiers = VirtualKeyModifiers.Menu
      };
      altLeft.Invoked += BackInvoked;
      KeyboardAccelerators.Add(altLeft);

    }

    private void Nav_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
    {
      if (args.IsSettingsInvoked == true)
      {
        NavView_Navigate("settings", new SlideNavigationTransitionInfo());
      }
      else if (args.InvokedItem != null)
      {
        var navItemTag = args.InvokedItem;
        NavView_Navigate(navItemTag as string, new SlideNavigationTransitionInfo());
      }
    }

    private void BackInvoked(KeyboardAccelerator sender,
                         KeyboardAcceleratorInvokedEventArgs args)
    {
      On_BackRequested();
      args.Handled = true;
    }

    private bool On_BackRequested()
    {
      if (!ContentFrame.CanGoBack)
        return false;

      // Don't go back if the nav pane is overlayed.
      if (Nav.IsPaneOpen &&
       (Nav.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
        Nav.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
        return false;


      ContentFrame.GoBack();
      return true;
    }

    private void Nav_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
    {
      On_BackRequested();
    }


    private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
    {
      Type _page = null;
      if (navItemTag == "settings")
      {
        _page = typeof(Settings);
      }
      else
      {
        var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
        _page = item.Page;
      }
      // Get the page type before navigation so you can prevent duplicate
      // entries in the backstack.
      var preNavPageType = ContentFrame.CurrentSourcePageType;

      // Only navigate if the selected page isn't currently loaded.
      if (!(_page is null) && !Equals(preNavPageType, _page))
      {
        ContentFrame.Navigate(_page, null, transitionInfo);
      }
    }


    private void On_Navigated(object sender, NavigationEventArgs e)
    {
      Nav.IsBackEnabled = ContentFrame.CanGoBack;

      if (ContentFrame.SourcePageType == typeof(Settings))
      {
        // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
        Nav.SelectedItem = (muxc.NavigationViewItem)Nav.SettingsItem;
        Nav.Header = "Settings";
      }
      else if (ContentFrame.SourcePageType != null)
      {
        var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

        Nav.SelectedItem = Nav.MenuItems
            .OfType<muxc.NavigationViewItem>()
            .First(n => n.Tag.Equals(item.Tag));

        Nav.Header =
            ((muxc.NavigationViewItem)Nav.SelectedItem)?.Content?.ToString();
      }
    }
  }
}
