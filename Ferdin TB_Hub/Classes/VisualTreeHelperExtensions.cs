using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Ferdin_TB_Hub.Classes
{
    public static class VisualTreeHelperExtensions
    {
        public static T GetFirstDescendantOfType<T>(DependencyObject startNode) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);

            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(startNode, i);

                if (current is T typedValue)
                {
                    return typedValue;
                }

                T result = GetFirstDescendantOfType<T>(current);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
