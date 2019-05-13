using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuinCalcData.Models;
using QuinCalc.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using QuinCalc.Services;
using System.Collections.Generic;
using QuinCalc.Helpers;
using QuinCalc.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace QuinCalc
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  [Windows.UI.Xaml.Data.Bindable]
  public sealed partial class App
  {
    private WinRTContainer _container;
    private Lazy<ActivationService> _activationService;

    private ActivationService ActivationService
    {
      get { return _activationService.Value; }
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      
      InitializeComponent();
      EnteredBackground += App_EnteredBackground;
      Resuming += App_Resuming;
      UnhandledException += App_UnhandledException1;
      AppCenter.Start("6a3a0926-ddb0-47b8-afc3-b27dc0b61683", typeof(Analytics), typeof(Crashes));
      Initialize();
      _activationService = new Lazy<ActivationService>(CreateActivationService);
      using (var db = new QuincalcContext())
      {
        db.Database.Migrate();
      }
    }

    private void App_UnhandledException1(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
    {
      Debug.WriteLine(e.Message, "Quincalc:Exceptions");
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
      if (!args.PrelaunchActivated)
      {
        await ActivationService.ActivateAsync(args);
      }
    }

    protected override async void OnActivated(IActivatedEventArgs args)
    {
      await ActivationService.ActivateAsync(args);
    }

    private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
    {
      Debug.WriteLine(e.Message, "Quincalc:Exceptions");
    }

    protected override void Configure()
    {
      // This configures the framework to map between MainViewModel and MainPage
      // Normally it would map between MainPageViewModel and MainPage
      var config = new TypeMappingConfiguration
      {
        IncludeViewSuffixInViewModelNames = false
      };

      ViewLocator.ConfigureTypeMappings(config);
      ViewModelLocator.ConfigureTypeMappings(config);

      _container = new WinRTContainer();
      _container.RegisterWinRTServices();

      _container.PerRequest<ShellViewModel>();
      _container.PerRequest<HomeViewModel>();
      _container.PerRequest<ExpensesViewModel>();
      _container.PerRequest<TodosViewModel>();
      //_container.PerRequest<QuickNotesViewModel>();
      //_container.PerRequest<MyListsViewModel>();
      //_container.PerRequest<ExpensesViewModel>();
      _container.PerRequest<SettingsViewModel>();
      _container.PerRequest<ShareTargetViewModel>();
    }

    protected override object GetInstance(Type service, string key)
    {
      return _container.GetInstance(service, key);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
      return _container.GetAllInstances(service);
    }

    protected override void BuildUp(object instance)
    {
      _container.BuildUp(instance);
    }

    private ActivationService CreateActivationService()
    {
      return new ActivationService(_container, typeof(ViewModels.HomeViewModel), new Lazy<UIElement>(CreateShell));
    }

    private UIElement CreateShell()
    {
      var shellPage = new Views.ShellPage();
      return shellPage;
    }

    private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
    {
      var deferral = e.GetDeferral();
      await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
      deferral.Complete();
    }

    private void App_Resuming(object sender, object e)
    {
      Singleton<SuspendAndResumeService>.Instance.ResumeApp();
    }

    protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
    {
      await ActivationService.ActivateFromShareTargetAsync(args);
    }

    //protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    //{
    //  await ActivationService.ActivateAsync(args);
    //}
  }
}
