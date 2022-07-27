using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed class GroupLayer : LayerBase, ILayer
    {

        public LayerType Type => LayerType.Group;

        public ICanvasImage this[BitmapType type] => null;


        //@Construct
        public GroupLayer(ICanvasResourceCreator resourceCreator, int width, int height) : this(null, resourceCreator, width, height) { }
        public GroupLayer(string id, ICanvasResourceCreator resourceCreator, int width, int height) : base(id, resourceCreator, width, height)
        {
        }

        public ICanvasImage Render(ICanvasImage background) => base.Children.Render(background);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Children.Render(background, matrix, interpolationMode);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Children.Render(background, matrix, interpolationMode, id, mezzanine);

        public ICanvasImage Merge(ILayerRender previousRender, ICanvasImage previousImage)
        {
            if (base.Children.Count is 0) return null;

            if (base.Opacity == 0.0) return null;
            else if (base.Opacity == 1.0) return previousRender.Render(previousImage, this.Render(new ColorSourceEffect { Color = Colors.Transparent }));
            return previousRender.Render(previousImage, new OpacityEffect
            {
                Opacity = base.Opacity,
                Source = this.Render(new ColorSourceEffect { Color = Colors.Transparent })
            });
        }

        public bool FillContainsPoint(Vector2 point)
        {
            foreach (ILayer item in base.Children)
            {
                if (item.FillContainsPoint(point)) return true;
            }
            return false;
        }

        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new GroupLayer(resourceCreator, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new GroupLayer(resourceCreator, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new GroupLayer(resourceCreator, width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => new GroupLayer(resourceCreator, width, height);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip) => new GroupLayer(resourceCreator, base.Width, base.Height);
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees:
                case BitmapRotation.Clockwise270Degrees:
                    return new GroupLayer(resourceCreator, base.Height, base.Width);
                default:
                    return new GroupLayer(resourceCreator, base.Width, base.Height);
            }
        }

    }
}