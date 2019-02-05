using Windows.ApplicationModel.Background;

namespace QuinCalcTasks
{
  public sealed class BackgroundTask : IBackgroundTask
  {
    public async void Run(IBackgroundTaskInstance taskInstance)
    {
      var _deferral = taskInstance.GetDeferral();
      //
      // TODO: Insert code to start one or more asynchronous methods using the
      //       await keyword, for example:
      //
      // await ExampleMethodAsync();
      //

      _deferral.Complete();
    }
  }
}
