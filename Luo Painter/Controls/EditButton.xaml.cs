using Luo_Painter.Edits;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal class EditTypeCommand : RelayCommand<EditType> { }

    internal class EditsCanvas : Canvas
    {

        readonly BitmapSize HSize = new BitmapSize { Width = 400, Height = 412 };
        readonly BitmapSize HItemSize = new BitmapSize { Width = 128, Height = 40 };
        readonly BitmapSize[] HCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 276, Height = 248 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 140, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 276, Height = 4 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 238 }, /// <see cref="EditType.Remove"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 328 }, /// <see cref="EditType.Merge"/>
            new BitmapSize{ Width = 0, Height = 370 }, /// <see cref="EditType.Flatten"/>

            new BitmapSize{ Width = 272, Height = 271}, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 272, Height = 313 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 272, Height = 361 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 136, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 136, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 136, Height = 112 }, /// <see cref="EditType.Invert"/>
            new BitmapSize{ Width = 136, Height = 154 }, /// <see cref="EditType.Pixel"/>
            new BitmapSize{ Width = 136, Height = 202 }, /// <see cref="EditType.Feather"/>
            new BitmapSize{ Width = 136, Height = 244 }, /// <see cref="EditType.Transform"/>
            new BitmapSize{ Width = 136, Height = 286 }, /// <see cref="EditType.Grow"/>
            new BitmapSize{ Width = 136, Height = 328 }, /// <see cref="EditType.Shrink"/>

            new BitmapSize{ Width = 272, Height = 28 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 272, Height = 70 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 272, Height = 112 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 272, Height = 154 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 272, Height = 202 }, /// <see cref="EditType.ExpandStroke"/>
        };

        readonly BitmapSize VEditSize = new BitmapSize { Width = 320, Height = 612 };
        readonly BitmapSize VEditItemSize = new BitmapSize { Width = 156, Height = 40 };
        readonly BitmapSize[] VEditCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 4, Height = 415 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 168, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 168, Height = 373 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 244 }, /// <see cref="EditType.Remove"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 328 }, /// <see cref="EditType.Merge"/>
            new BitmapSize{ Width = 0, Height = 370 }, /// <see cref="EditType.Flatten"/>

            new BitmapSize{ Width = 0, Height = 439 }, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 0, Height = 481 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 0, Height = 529 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 164, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 164, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 164, Height = 112 }, /// <see cref="EditType.Invert"/>
            new BitmapSize{ Width = 164, Height = 154 }, /// <see cref="EditType.Pixel"/>
            new BitmapSize{ Width = 164, Height = 202 }, /// <see cref="EditType.Feather"/>
            new BitmapSize{ Width = 164, Height = 244 }, /// <see cref="EditType.Transform"/>
            new BitmapSize{ Width = 164, Height = 286 }, /// <see cref="EditType.Grow"/>
            new BitmapSize{ Width = 164, Height = 328 }, /// <see cref="EditType.Shrink"/>
       
            new BitmapSize{ Width = 164, Height = 397 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 164, Height = 439 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 164, Height = 481 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 164, Height = 523 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 164, Height = 571 }, /// <see cref="EditType.ExpandStroke"/>
        };

        public void Resizing(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Vertical: this.Resizing(this.VEditSize, this.VEditItemSize, this.VEditCoordinates); break;
                case Orientation.Horizontal: this.Resizing(this.HSize, this.HItemSize, this.HCoordinates); break;
                default: break;
            }
        }

        public void Resizing(BitmapSize size, BitmapSize itemSize, BitmapSize[] coordinates)
        {
            base.Width = size.Width;
            base.Height = size.Height;
            for (int i = 0; i < base.Children.Count; i++)
            {
                FrameworkElement item = base.Children[i] as FrameworkElement;
                item.Width = itemSize.Width;
                item.Height = itemSize.Height;

                BitmapSize p = coordinates[i];
                Canvas.SetLeft(item, p.Width);
                Canvas.SetTop(item, p.Height);
            }
        }
    }

    internal sealed class EditItem : Button
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="EditItem"/>. </summary>
        public EditType Type
        {
            get => (EditType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditItem.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(EditType), typeof(EditItem), new PropertyMetadata(EditType.None, (sender, e) =>
        {
            EditItem control = (EditItem)sender;

            if (e.NewValue is EditType value)
            {
                control.CommandParameter = value;
                control.Icon = new ContentControl
                {
                    Content = value,
                    Template = value.GetTemplate(out ResourceDictionary resource),
                    Resources = resource,
                };
                control.Icon.GoToState(control.IsEnabled);
                control.Content = IconExtensions.GetGrid(control.Icon, value.ToString());
            }
        }));


        #endregion

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
    }

    public sealed partial class EditButton : Button
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
        /// <summary> Identifies the <see cref = "EditButton.Type" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(EditButton), new PropertyMetadata(Orientation.Horizontal, (sender, e) =>
        {
            EditButton control = (EditButton)sender;

            if (e.NewValue is Orientation value)
            {
                control.Canvas.Resizing(value);
            }
        }));


        #endregion

        //@Construct
        public EditButton()
        {
            this.InitializeComponent();
            this.Canvas.Resizing(this.Orientation);
            this.ItemClick += (s, e) => this.EditFlyout.Hide();
            base.Click += (s, e) =>
           {
               switch (this.Orientation)
               {
                   case Orientation.Vertical:
                       if (base.Parent is FrameworkElement element)
                       {
                           this.EditFlyout.ShowAt(element);
                       }
                       break;
                   case Orientation.Horizontal:
                       this.EditFlyout.ShowAt(this);
                       break;
                   default:
                       break;
               }
           };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}