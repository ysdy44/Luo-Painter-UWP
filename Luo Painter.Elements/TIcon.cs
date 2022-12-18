using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
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

    public abstract class TItem<T> : TIcon<T>
        where T : Enum
    {
        protected readonly TextBlock TextBlock = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
        };
        protected readonly ContentControl Icon = new ContentControl
        {
            Width = 32,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };
        public TItem()
        {
            base.HorizontalAlignment = HorizontalAlignment.Stretch;
            base.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            base.Content = new Grid
            {
                Children =
                {
                    this.TextBlock,
                    this.Icon,
                }
            };
        }
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

    public abstract class TAppBarButton<T> : AppBarButton
        where T : Enum
    {
        //@Abstract
        protected abstract void OnTypeChanged(T value);

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="TAppBarButton{T}"/>. </summary>
        public T Type
        {
            get => (T)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TAppBarButton.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(T), typeof(TAppBarButton<T>), new PropertyMetadata(default(T), (sender, e) =>
        {
            TAppBarButton<T> control = (TAppBarButton<T>)sender;

            if (e.NewValue is T value)
            {
                control.OnTypeChanged(value);
            }
        }));


        #endregion
    }
}