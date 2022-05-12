using Luo_Painter.Edits;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal class EditTypeCommand : RelayCommand<EditType> { }

    public struct EditPosition
    {
        public uint Index;
        public uint X;
        public uint Y;
    }

    public sealed partial class EditButton : Canvas
    {

        readonly uint HWidth = 400;
        readonly uint HHeight = 412;
        readonly EditPosition[] HCoordinates = new EditPosition[]
        {
            new EditPosition{ Index = 0, X = 4 , Y = 4 }, /// <see cref="EditGroupType.Edit"/>
            new EditPosition{ Index = 2, X = 4 , Y = 248 }, /// <see cref="EditGroupType.Group"/>
            new EditPosition{ Index = 1, X = 4 , Y = 4 }, /// <see cref="EditGroupType.Select"/>
            new EditPosition{ Index = 2, X = 4 , Y = 4 }, /// <see cref="EditGroupType.Combine"/>

            new EditPosition{ Index = 0, Y = 28 }, /// <see cref="EditType.Cut"/>
            new EditPosition{ Index = 0, Y = 70 }, /// <see cref="EditType.Duplicate"/>
            new EditPosition{ Index = 0, Y = 112 }, /// <see cref="EditType.Copy"/>
            new EditPosition{ Index = 0, Y = 154 }, /// <see cref="EditType.Paste"/>
            new EditPosition{ Index = 0, Y = 196 }, /// <see cref="EditType.Clear"/>
            new EditPosition{ Index = 0, Y = 238 }, /// <see cref="EditType.Remove"/>
            new EditPosition{ Index = 0, Y = 286 }, /// <see cref="EditType.Extract"/>
            new EditPosition{ Index = 0, Y = 328 }, /// <see cref="EditType.Merge"/>
            new EditPosition{ Index = 0, Y = 370 }, /// <see cref="EditType.Flatten"/>

            new EditPosition{ Index = 2, Y = 271}, /// <see cref="EditType.Group"/>
            new EditPosition{ Index = 2, Y = 313 }, /// <see cref="EditType.Ungroup"/>
            new EditPosition{ Index = 2, Y = 361 }, /// <see cref="EditType.Release"/>

            new EditPosition{ Index = 1, Y = 28 }, /// <see cref="EditType.All"/>
            new EditPosition{ Index = 1, Y = 70 }, /// <see cref="EditType.Deselect"/>
            new EditPosition{ Index = 1, Y = 112 }, /// <see cref="EditType.Invert"/>
            new EditPosition{ Index = 1, Y = 154 }, /// <see cref="EditType.Pixel"/>
            new EditPosition{ Index = 1, Y = 202 }, /// <see cref="EditType.Feather"/>
            new EditPosition{ Index = 1, Y = 244 }, /// <see cref="EditType.Transform"/>
            new EditPosition{ Index = 1, Y = 286 }, /// <see cref="EditType.Grow"/>
            new EditPosition{ Index = 1, Y = 328 }, /// <see cref="EditType.Shrink"/>

            new EditPosition{ Index = 2, Y = 28 }, /// <see cref="EditType.Union"/>
            new EditPosition{ Index = 2, Y = 70 }, /// <see cref="EditType.Exclude"/>
            new EditPosition{ Index = 2, Y = 112 }, /// <see cref="EditType.Xor"/>
            new EditPosition{ Index = 2, Y = 154 }, /// <see cref="EditType.Intersect"/>
            new EditPosition{ Index = 2, Y = 202 }, /// <see cref="EditType.ExpandStroke"/>
        };

        readonly uint VWidth = 320;
        readonly uint VHeight = 612;
        readonly EditPosition[] VEditCoordinates = new EditPosition[]
        {
            new EditPosition{ Index = 0, X = 4 , Y = 4 }, /// <see cref="EditGroupType.Edit"/>
            new EditPosition{ Index = 0, X = 4 , Y = 415 }, /// <see cref="EditGroupType.Group"/>
            new EditPosition{ Index = 1, X = 4 , Y = 4 }, /// <see cref="EditGroupType.Select"/>
            new EditPosition{ Index = 1, X = 4 , Y = 373 }, /// <see cref="EditGroupType.Combine"/>

            new EditPosition{ Index = 0, Y = 28 }, /// <see cref="EditType.Cut"/>
            new EditPosition{ Index = 0, Y = 70 }, /// <see cref="EditType.Duplicate"/>
            new EditPosition{ Index = 0, Y = 112 }, /// <see cref="EditType.Copy"/>
            new EditPosition{ Index = 0, Y = 154 }, /// <see cref="EditType.Paste"/>
            new EditPosition{ Index = 0, Y = 196 }, /// <see cref="EditType.Clear"/>
            new EditPosition{ Index = 0, Y = 238 }, /// <see cref="EditType.Remove"/>
            new EditPosition{ Index = 0, Y = 286 }, /// <see cref="EditType.Extract"/>
            new EditPosition{ Index = 0, Y = 328 }, /// <see cref="EditType.Merge"/>
            new EditPosition{ Index = 0, Y = 370 }, /// <see cref="EditType.Flatten"/>

            new EditPosition{ Index = 0, Y = 439 }, /// <see cref="EditType.Group"/>
            new EditPosition{ Index = 0, Y = 481 }, /// <see cref="EditType.Ungroup"/>
            new EditPosition{ Index = 0, Y = 529 }, /// <see cref="EditType.Release"/>

            new EditPosition{ Index = 1, Y = 28 }, /// <see cref="EditType.All"/>
            new EditPosition{ Index = 1, Y = 70 }, /// <see cref="EditType.Deselect"/>
            new EditPosition{ Index = 1, Y = 112 }, /// <see cref="EditType.Invert"/>
            new EditPosition{ Index = 1, Y = 154 }, /// <see cref="EditType.Pixel"/>
            new EditPosition{ Index = 1, Y = 202 }, /// <see cref="EditType.Feather"/>
            new EditPosition{ Index = 1, Y = 244 }, /// <see cref="EditType.Transform"/>
            new EditPosition{ Index = 1, Y = 286 }, /// <see cref="EditType.Grow"/>
            new EditPosition{ Index = 1, Y = 328 }, /// <see cref="EditType.Shrink"/>
       
            new EditPosition{ Index = 1, Y = 397 }, /// <see cref="EditType.Union"/>
            new EditPosition{ Index = 1, Y = 439 }, /// <see cref="EditType.Exclude"/>
            new EditPosition{ Index = 1, Y = 481 }, /// <see cref="EditType.Xor"/>
            new EditPosition{ Index = 1, Y = 523 }, /// <see cref="EditType.Intersect"/>
            new EditPosition{ Index = 1, Y = 571 }, /// <see cref="EditType.ExpandStroke"/>
        };

        public void Resizing(Orientation orientation, bool fullScreen)
        {
            switch (orientation)
            {
                case Orientation.Vertical:
                    base.Width = fullScreen ? double.NaN : this.VWidth;
                    base.Height = this.VHeight;
                    this.Resizing(this.VEditCoordinates, ((fullScreen ? base.ActualWidth : this.VWidth) - 8) / 2);
                    break;
                case Orientation.Horizontal:
                    base.Width = fullScreen ? double.NaN : this.HWidth;
                    base.Height = this.HHeight;
                    this.Resizing(this.HCoordinates, ((fullScreen ? base.ActualWidth : this.HWidth) - 8 - 8) / 3);
                    break;
                default:
                    break;
            }
        }

        public void Resizing(EditPosition[] coordinates, double itemWidth)
        {
            double column = itemWidth + 8;

            for (int i = 0; i < base.Children.Count; i++)
            {
                FrameworkElement item = base.Children[i] as FrameworkElement;
                item.Width = itemWidth;

                EditPosition p = coordinates[i];
                Canvas.SetLeft(item, p.X + p.Index * column);
                Canvas.SetTop(item, p.Y);
            }
        }
    }

    internal sealed class EditItem : TButton<EditType>
    {

        Control Icon;
        public EditItem()
        {
            base.IsEnabledChanged += (s, e) =>
            {
                if (this.Icon is null) return;
                if (e.NewValue is bool value)
                {
                    this.Icon.GoToState(value);
                }
            };
        }

        protected override void OnTypeChanged(EditType value)
        {
            base.CommandParameter = value;
            this.Icon = new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            };
            this.Icon.GoToState(base.IsEnabled);
            base.Content = TIconExtensions.GetGrid(this.Icon, value.ToString());
        }
    }

    public sealed partial class EditButton : Canvas
    {
        //@Delegate
        public event EventHandler<EditType> ItemClick
        {
            remove => this.EditTypeCommand.Click -= value;
            add => this.EditTypeCommand.Click += value;
        }

        #region DependencyProperty


        /// <summary> Gets or set the orientation for <see cref="EditButton"/>. </summary>
        public Orientation Orientation
        {
            get => (Orientation)base.GetValue(OrientationProperty);
            set => base.SetValue(OrientationProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditButton.Orientation" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(EditButton), new PropertyMetadata(Orientation.Horizontal, (sender, e) =>
        {
            EditButton control = (EditButton)sender;
            if (control.IsLoaded is false) return;

            if (e.NewValue is Orientation value)
            {
                control.Resizing(value, control.FullScreen);
            }
        }));


        /// <summary> Gets a state that indicates whether the <see cref = "EditButton" />. </summary>
        public bool FullScreen
        {
            get => (bool)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditButton.FullScreen" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(FullScreen), typeof(bool), typeof(EditButton), new PropertyMetadata(false, (sender, e) =>
        {
            EditButton control = (EditButton)sender;

            if (e.NewValue is bool value)
            {
                if (control.IsLoaded) control.Resizing(control.Orientation, value);
                if (value)
                    control.SizeChanged += control.EditButton_SizeChanged;
                else
                    control.SizeChanged -= control.EditButton_SizeChanged;
            }
        }));


        #endregion

        //@Construct
        public EditButton()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => this.Resizing(this.Orientation, this.FullScreen);
        }

        private void EditButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize == Size.Empty) return;
            if (e.NewSize == e.PreviousSize) return;
            if (e.NewSize.Width == e.PreviousSize.Width) return;

            this.Resizing(this.Orientation, this.FullScreen);
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}