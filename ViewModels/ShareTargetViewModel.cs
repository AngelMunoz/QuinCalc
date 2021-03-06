﻿using Caliburn.Micro;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

namespace QuinCalc.ViewModels
{
  public class ShareTargetViewModel : Screen
  {
    private ShareOperation _shareOperation;

    private SharedDataViewModelBase _sharedData;

    public SharedDataViewModelBase SharedData
    {
      get => _sharedData;
      set => Set(ref _sharedData, value);
    }

    public ShareTargetViewModel()
    {
    }

    public async Task LoadAsync(ShareOperation shareOperation)
    {
      // TODO WTS: Configure the Share Target Declaration for the formats you require.
      // Share Target declarations are defined in Package.appxmanifest.
      // Current declarations allow to share WebLink and image files with the app.
      // ShareTarget can be tested sharing the WebLink from Microsoft Edge or sharing images from Windows Photos.

      // TODO WTS: Customize SharedDataModelBase or derived classes adding properties for data that you need to extract from _shareOperation
      _shareOperation = shareOperation;
      if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
      {
        SharedData = new SharedDataStorageItemsViewModel();
      }

      if (shareOperation.Data.Contains(StandardDataFormats.WebLink))
      {
        SharedData = new SharedDataWebLinkViewModel();
      }

      await SharedData?.LoadDataAsync(_shareOperation);
    }

    public void Complete()
    {
      // TODO WTS: Implement any other logic or add a QuickLink before completing the share operation.
      // More details at https://docs.microsoft.com/en-us/windows/uwp/app-to-app/receive-data
      _shareOperation.ReportCompleted();
    }
  }
}
