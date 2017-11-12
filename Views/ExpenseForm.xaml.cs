using QuinCalc.Models;
using QuinCalc.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class ExpenseForm : Page
  {
    public ExpenseViewModel expensevm = new ExpenseViewModel();
    public bool EnableSave { get; set; }
    public string tempAmt = "0";
    public bool updating = false;

    public ExpenseForm()
    {
      this.InitializeComponent();
    }

    private async void SaveExpenseBtn_Click(object sender, RoutedEventArgs e)
    {
      expensevm.DueDate = DatePickerCtrl.Date.DateTime;
      Decimal.TryParse(tempAmt, out decimal val);
      expensevm.Amount = val;
      using (var context = new QuincalcContext())
      {
        if(!updating)
        {
          context.Expenses.Add(expensevm);
        }
        else
        {
          context.Expenses.Attach(expensevm);
          context.Expenses.Update(expensevm);
        }
        await context.SaveChangesAsync();
      }
      Frame.Navigate(typeof(MainPage));
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
      Frame.Navigate(typeof(MainPage));
    }
    
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      expensevm = (ExpenseViewModel)e.Parameter ?? expensevm;
      tempAmt = expensevm.Amount.ToString();
      updating = (ExpenseViewModel)e.Parameter != null ? true : false;
    }
  }
}
