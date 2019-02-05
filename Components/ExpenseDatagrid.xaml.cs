using QuinCalc.Enums;
using QuinCalc.ViewModels;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuinCalc.Components
{
  public sealed partial class ExpenseDatagrid : UserControl
  {
    public event EventHandler<(ExpenseVm, ExpenseUpdateType)> OnExpenseUpdate;
    public event EventHandler<bool> OnPaneToggle;

    public ExpenseVm CurrentExpense
    {
      get { return (ExpenseVm)GetValue(CurrentExpenseProperty); }
      set { SetValue(CurrentExpenseProperty, value); }
    }


    public bool IsPaneOpen
    {
      get { return (bool)GetValue(IsPaneOpenProperty); }
      set { SetValue(IsPaneOpenProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsPaneOpen.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsPaneOpenProperty =
        DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(ExpenseDatagrid), new PropertyMetadata(true));

    // Using a DependencyProperty as the backing store for CurrentExpense.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentExpenseProperty =
        DependencyProperty.Register("CurrentExpense", typeof(ExpenseVm), typeof(ExpenseDatagrid), new PropertyMetadata(null));

    public ExpenseDatagrid()
    {
      InitializeComponent();
      Loaded += ExpenseDatagrid_Loaded;
      OnPaneToggle += ExpenseDatagrid_OnPaneToggle;
    }

    private void ExpenseDatagrid_OnPaneToggle(object sender, bool e)
    {
      MainSplitView.IsPaneOpen = e;
    }

    private void ExpenseDatagrid_Loaded(object sender, RoutedEventArgs e)
    {
      ExpensesList.ItemsSource = DataContext as ObservableCollection<ExpenseVm>;
    }

    private void ExpensesList_ItemClick(object sender, ItemClickEventArgs e)
    {
      CurrentExpense = e.ClickedItem as ExpenseVm;
      MainSplitView.IsPaneOpen = false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      MainSplitView.IsPaneOpen = true;
    }

    private void ExpenseForm_OnExpenseUpdate(object sender, (ExpenseVm, ExpenseUpdateType) e)
    {
      OnExpenseUpdate?.Invoke(sender, e);
      CurrentExpense = null;
      MainSplitView.IsPaneOpen = true;
    }
  }
}

