using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Ferdin_TB_Hub.Classes
{
    public static class VisualTreeHelperExtensions
    {
        public static T GetFirstDescendantOfType<T>(DependencyObject startNode) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(startNode);

            for (var i = 0; i < count; i++)
            {
                var current = VisualTreeHelper.GetChild(startNode, i);

                if (current is T typedValue)
                {
                    return typedValue;
                }

                var result = GetFirstDescendantOfType<T>(current);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
