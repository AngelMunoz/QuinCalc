﻿using QuinCalc.ViewModels;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views
{
  public sealed partial class ShareTargetPage : Page
  {
    public ShareTargetViewModel ViewModel { get; } = new ShareTargetViewModel();

    public ShareTargetPage()
    {
      DataContext = ViewModel;
      InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      await ViewModel.LoadAsync(e.Parameter as ShareOperation);
    }
  }
}
