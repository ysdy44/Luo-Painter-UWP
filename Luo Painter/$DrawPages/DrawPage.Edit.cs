using Luo_Painter.Edits;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public class EditsCanvas : Canvas
    {

        readonly BitmapSize HSize = new BitmapSize { Width = 400, Height = 327 };
        readonly BitmapSize HItemSize = new BitmapSize { Width = 128, Height = 40 };
        readonly BitmapSize[] HCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 140, Height = 157 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 140, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 276, Height = 4 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 244 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Merge"/>

            new BitmapSize{ Width = 136, Height = 181 }, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 136, Height = 223 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 136, Height = 271 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 136, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 136, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 136, Height = 112 }, /// <see cref="EditType.Invert"/>

            new BitmapSize{ Width = 272, Height = 28 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 272, Height = 70 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 272, Height = 112 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 272, Height = 154 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 272, Height = 202 }, /// <see cref="EditType.ExpandStroke"/>
        };

        readonly BitmapSize VEditSize = new BitmapSize { Width = 320, Height = 486 };
        readonly BitmapSize VEditItemSize = new BitmapSize { Width = 156, Height = 40 };
        readonly BitmapSize[] VEditCoordinates = new BitmapSize[]
        {
            new BitmapSize{ Width = 4, Height = 4 }, /// <see cref="EditGroupType.Edit"/>
            new BitmapSize{ Width = 4, Height = 331 }, /// <see cref="EditGroupType.Group"/>
            new BitmapSize{ Width = 168, Height = 4 }, /// <see cref="EditGroupType.Select"/>
            new BitmapSize{ Width = 168, Height = 157 }, /// <see cref="EditGroupType.Combine"/>

            new BitmapSize{ Width = 0, Height = 28 }, /// <see cref="EditType.Cut"/>
            new BitmapSize{ Width = 0, Height = 70 }, /// <see cref="EditType.Duplicate"/>
            new BitmapSize{ Width = 0, Height = 112 }, /// <see cref="EditType.Copy"/>
            new BitmapSize{ Width = 0, Height = 154 }, /// <see cref="EditType.Paste"/>
            new BitmapSize{ Width = 0, Height = 196 }, /// <see cref="EditType.Clear"/>
            new BitmapSize{ Width = 0, Height = 244 }, /// <see cref="EditType.Extract"/>
            new BitmapSize{ Width = 0, Height = 286 }, /// <see cref="EditType.Merge"/>

            new BitmapSize{ Width = 0, Height = 355 }, /// <see cref="EditType.Group"/>
            new BitmapSize{ Width = 0, Height = 397 }, /// <see cref="EditType.Ungroup"/>
            new BitmapSize{ Width = 0, Height = 445 }, /// <see cref="EditType.Release"/>

            new BitmapSize{ Width = 164, Height = 28 }, /// <see cref="EditType.All"/>
            new BitmapSize{ Width = 164, Height = 70 }, /// <see cref="EditType.Deselect"/>
            new BitmapSize{ Width = 164, Height = 112 }, /// <see cref="EditType.Invert"/>
       
            new BitmapSize{ Width = 164, Height = 181 }, /// <see cref="EditType.Union"/>
            new BitmapSize{ Width = 164, Height = 223 }, /// <see cref="EditType.Exclude"/>
            new BitmapSize{ Width = 164, Height = 265 }, /// <see cref="EditType.Xor"/>
            new BitmapSize{ Width = 164, Height = 307 }, /// <see cref="EditType.Intersect"/>
            new BitmapSize{ Width = 164, Height = 355 }, /// <see cref="EditType.ExpandStroke"/>
        };

        #region DependencyProperty


        /// <summary> Gets or set the orientation for <see cref="EditsCanvas"/>. </summary>
        public Orientation Orientation
        {
            get => (Orientation)base.GetValue(OrientationProperty);
            set => base.SetValue(OrientationProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditsCanvas.Type" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(EditsCanvas), new PropertyMetadata(Orientation.Vertical, (sender, e) =>
        {
            EditsCanvas control = (EditsCanvas)sender;

            if (e.NewValue is Orientation value)
            {
                control.Resizing(value);
            }
        }));


        #endregion

        public void Resizing() => this.Resizing(this.Orientation);

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
}