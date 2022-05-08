using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public static class TIconExtensions
    {

        //@Static
        public static Grid GetGrid(UIElement icon, string text) => new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                },
                new ColumnDefinition
                {
                    Width =new GridLength(12)
                },
                new ColumnDefinition
                {
                    Width =new GridLength(1, GridUnitType.Star)
                },
            },
            Children =
            {
                icon,
                TIconExtensions.GetTextBlock(text, 2)
            }
        };

        public static StackPanel GetStackPanel(UIElement icon, string text) => new StackPanel
        {
            Spacing = 12,
            Orientation = Orientation.Horizontal,
            Children =
            {
                icon,
                TIconExtensions.GetTextBlock(text)
            }
        };

        public static FrameworkElement GetTextBlock(string text, int column = 0)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            if (column != 0) Grid.SetColumn(textBlock, column);
            return textBlock;
        }

    }


    public abstract class TIcon<T> : ContentControl
        where T : Enum
    {
        //@Abstract
        protected abstract void OnTypeChanged(T value);

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="TIcon{T}"/>. </summary>
        public T Type
        {
            get => (T)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(T), typeof(TIcon<T>), new PropertyMetadata(default(T), (sender, e) =>
        {
            TIcon<T> control = (TIcon<T>)sender;

            if (e.NewValue is T value)
            {
                control.OnTypeChanged(value);
            }
        }));


        #endregion
    }

    public abstract class TButton<T> : Button
        where T : Enum
    {
        //@Abstract
        protected abstract void OnTypeChanged(T value);

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="TButton{T}"/>. </summary>
        public T Type
        {
            get => (T)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TButton.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(T), typeof(TButton<T>), new PropertyMetadata(default(T), (sender, e) =>
        {
            TButton<T> control = (TButton<T>)sender;

            if (e.NewValue is T value)
            {
                control.OnTypeChanged(value);
            }
        }));


        #endregion
    }
}