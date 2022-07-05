using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    public static class FrameworkElementExtensions
    {

        public static void Toggle(this SelectorItem selectorItem) => selectorItem.IsSelected = !selectorItem.IsSelected;

        public static T FindAncestor<T>(this DependencyObject reference)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            if (parent is null) return default;
            else if (parent is T result) return result;
            else return parent.FindAncestor<T>();
        }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject element)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);

                if (child is null) continue;
                else if (child is T result) yield return result;
                else
                {
                    foreach (T childOfChild in child.FindDescendants<T>())
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        //@Attached
        public static int GetDepth(DependencyObject dp) => (int)dp.GetValue(DepthProperty);
        public static void SetDepth(DependencyObject dp, int value) => dp.SetValue(DepthProperty, value);
        public static readonly DependencyProperty DepthProperty = DependencyProperty.RegisterAttached("Depth", typeof(int), typeof(ColumnDefinition), new PropertyMetadata(0, (sender, e) =>
        {
            ColumnDefinition control = (ColumnDefinition)sender;

            if (e.NewValue is int value)
            {
                control.Width = new GridLength(value * 20);
            }
        }));

        //@Attached
        public static bool GetIsExpand(DependencyObject dp) => (bool)dp.GetValue(IsExpandProperty);
        public static void SetIsExpand(DependencyObject dp, bool value) => dp.SetValue(IsExpandProperty, value);
        public static readonly DependencyProperty IsExpandProperty = DependencyProperty.RegisterAttached("IsExpand", typeof(bool), typeof(ColumnDefinition), new PropertyMetadata(false, (sender, e) =>
        {
            RotateTransform control = (RotateTransform)sender;

            if (e.NewValue is bool value)
            {
                control.Angle = value ? 90 : 0;
            }
        }));

        //@Attached
        public static Visibility GetVisible(DependencyObject dp) => (Visibility)dp.GetValue(VisibleProperty);
        public static void SetVisible(DependencyObject dp, Visibility value) => dp.SetValue(VisibleProperty, value);
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.RegisterAttached("Visible", typeof(Visibility), typeof(UIElement), new PropertyMetadata(Visibility.Visible, (sender, e) =>
        {
            UIElement control = (UIElement)sender;

            if (e.NewValue is Visibility value)
            {
                control.Opacity = value == Visibility.Visible ? 1 : 0.4;
            }
        }));

    }
}