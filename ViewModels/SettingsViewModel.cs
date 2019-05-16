using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using QuinCalc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace QuinCalc.ViewModels
{
  public class SettingsViewModel : Screen
  {
    public Visibility FeedbackLinkVisibility => Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported() ? Visibility.Visible : Visibility.Collapsed;

    public async void LaunchFeedbackHub()
    {
      Analytics.TrackEvent("Launched Feedback");
      // This launcher is part of the Store Services SDK https://docs.microsoft.com/en-us/windows/uwp/monetize/microsoft-store-services-sdk
      var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
      await launcher.LaunchAsync();
    }

    private ElementTheme _elementTheme = ThemeSelectorService.Theme;

    public ElementTheme ElementTheme
    {
      get { return _elementTheme; }

      set { Set(ref _elementTheme, value); }
    }

    private string _versionDescription;

    public string VersionDescription
    {
      get { return _versionDescription; }

      set { Set(ref _versionDescription, value); }
    }

    public async void SwitchTheme(ElementTheme theme)
    {
      Analytics.TrackEvent("Switched Theme", new Dictionary<string, string> { { "Theme", Enum.GetName(typeof(ElementTheme), theme) } });
      await ThemeSelectorService.SetThemeAsync(theme);
    }

    public SettingsViewModel()
    {
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
      VersionDescription = GetVersionDescription();
      return base.OnInitializeAsync(cancellationToken);
    }

    private string GetVersionDescription()
    {
      var appName = "AppDisplayName".GetLocalized();
      var package = Package.Current;
      var packageId = package.Id;
      var version = packageId.Version;

      return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
  }
}
