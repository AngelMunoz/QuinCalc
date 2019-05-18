using Caliburn.Micro;
using QuinCalc.Activation;
using QuinCalc.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QuinCalc.Services
{
  internal class ActivationService
  {
    private readonly WinRTContainer _container;
    private readonly Type _defaultNavItem;
    private readonly Lazy<UIElement> _shell;

    public ActivationService(WinRTContainer container, Type defaultNavItem, Lazy<UIElement> shell = null)
    {
      _container = container;
      _shell = shell;
      _defaultNavItem = defaultNavItem;
    }

    public async Task ActivateAsync(object activationArgs)
    {
      if (IsInteractive(activationArgs))
      {
        // Initialize things like registering background task before the app is loaded
        await InitializeAsync();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (Window.Current.Content == null)
        {
          // Create a Frame to act as the navigation context and navigate to the first page
          if (_shell?.Value == null)
          {
            var frame = new Frame();
            NavigationService = _container.RegisterNavigationService(frame);
            Window.Current.Content = frame;
          }
          else
          {
            var viewModel = ViewModelLocator.LocateForView(_shell.Value);

            ViewModelBinder.Bind(viewModel, _shell.Value, null);

            await ScreenExtensions.TryActivateAsync(viewModel);

            NavigationService = _container.GetInstance<INavigationService>();
            Window.Current.Content = _shell?.Value;
          }
        }
      }

      var activationHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));

      if (activationHandler != null)
      {
        await activationHandler.HandleAsync(activationArgs);
      }

      if (IsInteractive(activationArgs))
      {
        var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem, NavigationService);
        if (defaultHandler.CanHandle(activationArgs))
        {
          await defaultHandler.HandleAsync(activationArgs);
        }

        // Ensure the current window is active
        Window.Current.Activate();

        // Tasks after activation
        await StartupAsync();
      }
    }

    private INavigationService NavigationService { get; set; }

    private async Task InitializeAsync()
    {
      await ThemeSelectorService.InitializeAsync();
    }

    private async Task StartupAsync()
    {
      await ThemeSelectorService.SetRequestedThemeAsync();

      // TODO WTS: This is a sample to demonstrate how to add a UserActivity. Please adapt and move this method call to where you consider convenient in your app.
      //await UserActivityService.AddSampleUserActivity();
      //await FirstRunDisplayService.ShowIfAppropriateAsync();
      //await WhatsNewDisplayService.ShowIfAppropriateAsync();
    }

    private IEnumerable<ActivationHandler> GetActivationHandlers()
    {
      yield return Singleton<ShareTargetActivationHandler>.Instance;
      yield return Singleton<SuspendAndResumeService>.Instance;
      yield return new SchemeActivationHandler(NavigationService);
    }

    private bool IsInteractive(object args)
    {
      return args is IActivatedEventArgs;
    }

    internal async Task ActivateFromShareTargetAsync(ShareTargetActivatedEventArgs activationArgs)
    {
      var shareTargetHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));
      if (shareTargetHandler != null)
      {
        await shareTargetHandler.HandleAsync(activationArgs);
      }
    }
  }
}
