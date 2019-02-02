using System;
using System.Collections.ObjectModel;
using System.Linq;
using QuinCalc.Enums;
using QuinCalc.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuinCalc.Components
{
  public sealed partial class ExpenseDatagrid : UserControl
  {
    public ExpenseVm CurrentExpense { get; private set; }
    public event EventHandler<(ExpenseVm, ExpenseUpdateType)> OnExpenseUpdate;

    public ExpenseDatagrid()
    {
      InitializeComponent();
      Loaded += ExpenseDatagrid_Loaded;
      DataContextChanged += ExpenseDatagrid_DataContextChanged;
    }

    private void ExpenseDatagrid_Loaded(object sender, RoutedEventArgs e)
    {
      MainDataGrid.ItemsSource = DataContext as ObservableCollection<ExpenseVm>;
    }

    private void ExpenseDatagrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
      MainDataGrid.ItemsSource = DataContext as ObservableCollection<ExpenseVm>;
    }

    private void MainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      CurrentExpense = e.AddedItems.Count == 0 ? null : e.AddedItems.First() as ExpenseVm;
    }

    private void UpdateExpense_Click(object sender, RoutedEventArgs e)
    {
      var btn = sender as AppBarButton;
      switch (btn.Tag)
      {
        case "Save":
          OnExpenseUpdate?.Invoke(this, (CurrentExpense, ExpenseUpdateType.Save));
          break;
        case "Delete":
          OnExpenseUpdate?.Invoke(this, (CurrentExpense, ExpenseUpdateType.Delete));
          break;
        case "MarkAsDone":
          OnExpenseUpdate?.Invoke(this, (CurrentExpense, ExpenseUpdateType.MarkAsDone));
          break;
        case "MarkAsNotDone":
          OnExpenseUpdate?.Invoke(this, (CurrentExpense, ExpenseUpdateType.MarkAsNotDone));
          break;
      }
    }
  }
}

