using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace running_social.Resources
{
    class UIHelper
    {
        public static bool ScrollToElement(ScrollViewer sv, UIElement element)
        {
            GeneralTransform transform = element.TransformToVisual(sv);
            Point position = transform.TransformPoint(new Point(0, 0));
            return sv.ChangeView(sv.HorizontalOffset, position.Y, sv.ZoomFactor);
        }
    }
}
