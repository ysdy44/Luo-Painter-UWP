using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Luo_Painter.Layers.Models
{
    public sealed class GroupLayer : LayerBase, ILayer
    {

        public LayerType Type => LayerType.Group;

        public ICanvasImage this[BitmapType type] => null;


        //@Construct
        public GroupLayer(ICanvasResourceCreator resourceCreator, int width, int height) : base(resourceCreator, width, height)
        {
        }

        public ICanvasImage Render(ICanvasImage background) => base.Children.Render(background);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Children.Render(background, matrix, interpolationMode);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Children.Render(background, matrix, interpolationMode, id, mezzanine);

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

        protected override ILayer FlipHorizontalSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);
        protected override ILayer FlipVerticalSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);
        protected override ILayer LeftTurnSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);
        protected override ILayer RightTurnSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);
        protected override ILayer OverTurnSelf(ICanvasResourceCreator resourceCreator) => new GroupLayer(resourceCreator, base.Width, base.Height);

    }
}