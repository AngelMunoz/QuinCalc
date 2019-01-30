using System;
using System.Diagnostics;
using System.Threading.Tasks;
using QuinCalc.Services;
using Windows.ApplicationModel.Background;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace QuinCalc.Tasks
{
  public sealed class NotifyUpNextExpenseTask
  {
    public static readonly string TaskName = "NotifyUpNextExpenseTask";
    public static readonly string TaskGroupName = "Quincalc:Expenses";
    private readonly ToastService ToastService;

    public NotifyUpNextExpenseTask()
    {
      ToastService = new ToastService();
    }

    public BackgroundTaskRegistration RegisterTask()
    {
      var builder = new BackgroundTaskBuilder()
      {
        TaskGroup = new BackgroundTaskRegistrationGroup(TaskGroupName, TaskGroupName)
      };
      builder.Name = TaskName;
      builder.IsNetworkRequested = false;
      builder.SetTrigger(new TimeTrigger(15, false));
      var task = builder.Register();
      task.Completed += Task_Completed;
      task.Progress += Task_Progress;
      return task;
    }

    private void Task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
    {
      Debug.WriteLine($"{sender.TaskGroup}, {sender.Name}, {args.Progress}", "Quincalc:Tasks");
    }

    private async void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
    {
      using (var exser = new ExpenseService())
      {
        var expense = await exser.FindClosest();
        var diff = expense.DueDate.DateTime - DateTimeOffset.Now;
        if (diff.Days < 2 && diff.Days > -2)
        {
          var content = ToastService.GenerateExpenseToast(expense);
          var toast = new ToastNotification(content.GetXml());
          toast.Activated += Toast_Activated;
          ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
      }
      Debug.WriteLine($"{sender.TaskGroup}, {sender.Name}, {args.InstanceId}", "Quincalc:Tasks");
    }

    private void Toast_Activated(ToastNotification sender, object args)
    {
      Debug.WriteLine(args);
    }

    public static void RemoveTasks()
    {
      var tasks = BackgroundTaskRegistration.GetTaskGroup(TaskGroupName);
      if (tasks != null)
      {
        foreach (var tsk in tasks.AllTasks)
        {
          tsk.Value.Unregister(true);
        }
      }
    }
  }
}
