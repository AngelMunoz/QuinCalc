using System;
using QuinCalc.ViewModels;
using QuinCalc.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QuinCalc.Components
{
  public sealed partial class ExpenseForm : UserControl
  {
    public event EventHandler<(ExpenseVm, ExpenseUpdateType)> OnExpenseUpdate;

    public ExpenseForm()
    {
      InitializeComponent();
    }
    
    private void UpdateExpense_Click(object sender, RoutedEventArgs e)
    {
      var btn = sender as AppBarButton;
      switch (btn.Tag)
      {
        case "Save":
          OnExpenseUpdate?.Invoke(this, ((DataContext as ExpenseVm), ExpenseUpdateType.Save));
          break;
        case "Delete":
          OnExpenseUpdate?.Invoke(this, ((DataContext as ExpenseVm), ExpenseUpdateType.Delete));
          break;
        case "MarkAsDone":
          OnExpenseUpdate?.Invoke(this, ((DataContext as ExpenseVm), ExpenseUpdateType.MarkAsDone));
          break;
        case "MarkAsNotDone":
          OnExpenseUpdate?.Invoke(this, ((DataContext as ExpenseVm), ExpenseUpdateType.MarkAsNotDone));
          break;
      }
    }
  }
}
