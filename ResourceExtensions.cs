using Windows.ApplicationModel.Resources;

namespace QuinCalc
{
  internal static class ResourceExtensions
  {
    private static ResourceLoader _resLoader = new ResourceLoader();

    public static string GetLocalized(this string resourceKey)
    {
      return _resLoader.GetString(resourceKey);
    }
  }
}
