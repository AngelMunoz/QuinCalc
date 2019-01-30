using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;
using QuinCalc.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace QuinCalc
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public sealed partial class App : Application
  {
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      InitializeComponent();
      Suspending += OnSuspending;
      UnhandledException += App_UnhandledException;
      using (var db = new QuincalcContext())
      {
        db.Database.Migrate();
      }

    }

    private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
    {
      Debug.WriteLine(e.Message, "Quincalc:Exceptions");
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      NotifyUpNextExpenseTask.RemoveTasks();
      var rootFrame = CreateRootFrame();
      if (e.PrelaunchActivated == false)
      {
        if (rootFrame.Content == null)
        {
          // When the navigation stack isn't restored navigate to the first page,
          // configuring the new page by passing required information as a navigation
          // parameter
          rootFrame.Navigate(typeof(MainPage), e.Arguments);
        }
        // Ensure the current window is active
        Window.Current.Activate();
      }

      if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
      {
        //TODO: Load state from previously suspended application
      }

      if (e.Kind == ActivationKind.Protocol)
      {
        rootFrame.Navigate(typeof(MainPage), e.Arguments);
      }

      var expenseNotifier = new NotifyUpNextExpenseTask();
      var tasks = BackgroundTaskRegistration.GetTaskGroup(NotifyUpNextExpenseTask.TaskGroupName);
      if (tasks == null || tasks?.AllTasks.Count == 0)
      {
        var task = expenseNotifier.RegisterTask();
        Debug.WriteLine(task.TaskGroup.Name);
      }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();
      //TODO: Save application state and stop any background activity
      deferral.Complete();
    }

    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
      base.OnBackgroundActivated(args);
      args.TaskInstance.Task.Unregister(true);
    }

    protected override void OnActivated(IActivatedEventArgs args)
    {
      base.OnActivated(args);
      NotifyUpNextExpenseTask.RemoveTasks();
      var rootFrame = CreateRootFrame();
      rootFrame.Navigate(typeof(MainPage), args);
      var expenseNotifier = new NotifyUpNextExpenseTask();
      var tasks = BackgroundTaskRegistration.GetTaskGroup(NotifyUpNextExpenseTask.TaskGroupName);
      if (tasks == null || tasks?.AllTasks.Count == 0)
      {
        var task = expenseNotifier.RegisterTask();
        Debug.WriteLine(task.TaskGroup.Name);
      }
      Window.Current.Activate();
    }

    private Frame CreateRootFrame()
    {
      Frame rootFrame = Window.Current.Content as Frame;

      // Do not repeat app initialization when the Window already has content,
      // just ensure that the window is active
      if (rootFrame == null)
      {
        // Create a Frame to act as the navigation context and navigate to the first page
        rootFrame = new Frame();

        rootFrame.NavigationFailed += OnNavigationFailed;

        // Place the frame in the current Window
        Window.Current.Content = rootFrame;
      }

      return rootFrame;
    }
  }
}
