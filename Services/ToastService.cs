using System.Globalization;
using Microsoft.Toolkit.Uwp.Notifications;
using QuinCalcData.Models;

namespace QuinCalc.Services
{
  public class ToastService
  {
    public static ToastContent GenerateExpenseToast(Expense expense)
    {
      return new ToastContent()
      {
        Launch = "action=viewExpenses",
        Scenario = ToastScenario.Reminder,
        Visual = new ToastVisual()
        {
          BindingGeneric = new ToastBindingGeneric()
          {
            Children =
            {
              new AdaptiveText()
              {
                Text=$"You have an expense soon: {expense.Name}"
              },
              new AdaptiveText()
              {
                Text=$"For an amount of: {expense.Amount.ToString("C", CultureInfo.CurrentCulture)}"
              }
            }
          }
        },
        Actions = new ToastActionsCustom()
        {
          Inputs =
          {
            new ToastSelectionBox("snoozeTime")
            {
              DefaultSelectionBoxItemId = "60",
              Items =
              {
                new ToastSelectionBoxItem("1", "1 minute"),
                new ToastSelectionBoxItem("15", "15 minutes"),
                new ToastSelectionBoxItem("60", "1 hour"),
                new ToastSelectionBoxItem("240", "4 hours"),
                new ToastSelectionBoxItem("1440", "1 day")
              }
            }
          },
          Buttons =
          {
            new ToastButtonSnooze()
            {
              SelectionBoxId = "snoozeTime",
            },
            new ToastButtonDismiss()
          }
        }
      };
    }
  }
}
