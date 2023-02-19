using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public class IndexGrid : Grid
    {
        #region DependencyProperty

        /// <summary>
        /// Gets or sets the index indicating the all Children.
        /// </summary>     
        public int Index
        {
            get => (int)base.GetValue(IndexProperty);
            set => base.SetValue(IndexProperty, value);
        }
        /// <summary> Identifies the <see cref="Index"/> property. </summary>
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Type), typeof(int), typeof(IndexGrid), new PropertyMetadata(-1, (sender, e) =>
        {
            IndexGrid control = (IndexGrid)sender;

            if (e.NewValue is int value && value >= 0)
            {
                for (int i = 0; i < control.Children.Count; i++)
                {
                    control.Children[i].Visibility = i == value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else
            {
                foreach (UIElement item in control.Children)
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }));

        #endregion
    }
}