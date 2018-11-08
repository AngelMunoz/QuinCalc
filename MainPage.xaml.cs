using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.ViewModels;
using QuinCalc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QuinCalc
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public TodoViewModel UpNext { get; set; }
    public ExpenseViewModel UpNextExpense { get; set; }
    public ExpenseViewModel NextQuin { get; set; }

    public int skipExpenses = 0;
    public int limitExpenses = 15;

    public MainPage()
    {
      InitializeComponent();
      LoadCollections();
    }

    private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
    {
      ("Home", typeof(Home)),
      ("Expenses", typeof(Expenses)),
      ("Todos", typeof(Todos)),
    };


    /// <summary>
    /// Loads useful Data from the Database when called
    /// </summary>
    /// <returns></returns>
    private async void LoadCollections()
    {
      using (var context = new QuincalcContext())
      {
        int dayToCheck = GetDayToCheck();
        decimal totalAmount = await GetTotalAmount(context, dayToCheck);
        NextQuin = new ExpenseViewModel
        {
          DueDate = GetNextQuin(dayToCheck),
          Amount = totalAmount
        };
        UpNext = await GetUpnext(context);
        UpNextExpense = await GetUpNextExpense(context);
      }
    }

    /// <summary>
    /// Get The closest Expense to *Today* (including Time)
    /// </summary>
    /// <param name="expenses"></param>
    /// <returns></returns>
    private async Task<ExpenseViewModel> GetUpNextExpense(QuincalcContext context)
    {
      var expense = await context.Expenses.OrderBy(e => Math.Abs((e.DueDate - DateTime.Now).Ticks)).FirstOrDefaultAsync();
      return new ExpenseViewModel(expense);
    }

    /// <summary>
    /// Get The Closest Todo according to the provided Date
    /// </summary>
    /// <param name="todos"></param>
    /// <returns></returns>
    private async Task<TodoViewModel> GetUpnext(QuincalcContext context)
    {
      var todo = await context.Todos
        .Where(t => !t.IsDone)
        .OrderBy(e => Math.Abs((e.DueDate - DateTime.Now).Ticks))
        .FirstOrDefaultAsync();
      return new TodoViewModel(todo);
    }

    /// <summary>
    /// Get the closest day to pay the expenses (either day 15 or last day of the month)
    /// </summary>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    private static DateTime GetNextQuin(int dayToCheck)
    {
      return new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayToCheck);
    }

    /// <summary>
    /// Get the Total Amount to pay on the NextQuin (day 15 or last day of the current month)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    private static async Task<decimal> GetTotalAmount(QuincalcContext context, int dayToCheck)
    {
      return await context.Expenses
        .Where(e => (e.DueDate.Day >= DateTime.Now.Day) && (e.DueDate.Day <= GetNextQuin(dayToCheck).Day))
        .SumAsync(e => e.Amount);
    }

    /// <summary>
    /// Gets the Next Day to Check (either 15 or last day of the month)
    /// </summary>
    /// <returns></returns>
    private static int GetDayToCheck()
    {
      return DateTime.Now.Day < 15 ? 15 : DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
    }

    private void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
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
          (Nav.DisplayMode == NavigationViewDisplayMode.Compact ||
           Nav.DisplayMode == NavigationViewDisplayMode.Minimal))
        return false;

      ContentFrame.GoBack();
      return true;
    }

    private void NavView_BackRequested(NavigationView sender,
                                   NavigationViewBackRequestedEventArgs args)
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
      if (!(_page is null) && !Type.Equals(preNavPageType, _page))
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
        Nav.SelectedItem = (NavigationViewItem)Nav.SettingsItem;
        Nav.Header = "Settings";
      }
      else if (ContentFrame.SourcePageType != null)
      {
        var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

        Nav.SelectedItem = Nav.MenuItems
            .OfType<NavigationViewItem>()
            .First(n => n.Tag.Equals(item.Tag));

        Nav.Header =
            ((NavigationViewItem)Nav.SelectedItem)?.Content?.ToString();
      }
    }
  }
}
