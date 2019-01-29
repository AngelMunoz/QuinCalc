using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace QuinCalc.Helpers
{
  public static class NodeFinder
  {
    public static DependencyObject FindChildByName(DependencyObject from, string name)
    {
      int count = VisualTreeHelper.GetChildrenCount(from);

      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(from, i);
        if (child is FrameworkElement && ((FrameworkElement)child).Name == name)
          return child;

        var result = FindChildByName(child, name);
        if (result != null)
          return result;
      }

      return null;
    }
  }
}
