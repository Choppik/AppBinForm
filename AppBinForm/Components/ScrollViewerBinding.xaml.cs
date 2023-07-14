using System.Windows;
using System.Windows.Controls;

namespace AppBinForm.Components
{
    public partial class ScrollViewerBinding : UserControl
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double),
            typeof(ScrollViewerBinding), new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnVerticalOffsetPropertyChanged));
        
        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.RegisterAttached("ScrollableHeight", typeof(double),
            typeof(ScrollViewerBinding), new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnScrollableHeightPropertyChanged)));

        private static readonly DependencyProperty VerticalScrollBindingProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBinding", typeof(bool?), typeof(ScrollViewerBinding));
        
        private static readonly DependencyProperty ScrollableHeightBindingProperty =
            DependencyProperty.RegisterAttached("ScrollableHeightBinding", typeof(bool?), typeof(ScrollViewerBinding));

        public static double GetVerticalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(VerticalOffsetProperty, value);
        }
        public static double GetScrollableHeight(DependencyObject depObj)
        {
            return (double)depObj.GetValue(ScrollableHeightProperty);
        }

        public static void SetScrollableHeight(DependencyObject depObj, double value)
        {
            depObj.SetValue(ScrollableHeightProperty, value);
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            BindVerticalOffset(scrollViewer);
            //if(scrollViewer.ScrollableHeight != scrollViewer.VerticalOffset)
            scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            //else scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset / 2);
            //BindScrollableHeight(scrollViewer);
        }
        private static void OnScrollableHeightPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer == null) return;
            BindScrollableHeight(scrollViewer);
        }

        public static void BindVerticalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(VerticalScrollBindingProperty) != null)
                return;

            scrollViewer.SetValue(VerticalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.VerticalChange == 0)
                    return;
                SetVerticalOffset(scrollViewer, se.VerticalOffset);
            };
        }
        public static void BindScrollableHeight(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(ScrollableHeightBindingProperty) != null)
                return;

            scrollViewer.SetValue(ScrollableHeightBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.Handled) return;
                SetScrollableHeight(scrollViewer, scrollViewer.ScrollableHeight);
            };
        }
    }
}