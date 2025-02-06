using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed class FillLayer : LayerBase, ILayer
    {

        readonly ColorSourceEffect ColorEffect;
        readonly CropEffect CropEffect;

        public LayerType Type => LayerType.Fill;
        public ICanvasImage this[BitmapType type] => this.CropEffect;


        //@Construct
        public FillLayer(ICanvasResourceCreator resourceCreator, Color color, int width, int height) : base(null, null, resourceCreator, width, height)
        {
            this.ColorEffect = new ColorSourceEffect
            {
                Color = color
            };
            this.CropEffect = new CropEffect
            {
                Source = this.ColorEffect,
                SourceRectangle = new Rect(0, 0, width, height)
            };
            this.RenderThumbnail(this.CropEffect);
        }

        public FillLayer(string id, XElement element, ICanvasResourceCreator resourceCreator, int width, int height) : base(id, element, resourceCreator, width, height)
        {
            if (element is null is false && element.Element("Color") is XElement color)
            {
                this.ColorEffect = new ColorSourceEffect
                {
                    Color = XML.LoadColor(color)
                };
            }
            else
            {
                this.ColorEffect = new ColorSourceEffect
                {
                    Color = Colors.White
                };
            }

            this.CropEffect = new CropEffect
            {
                Source = this.ColorEffect,
                SourceRectangle = new Rect(0, 0, width, height)
            };
            this.RenderThumbnail(this.CropEffect);
        }

        public XElement Save()
        {
            XElement element = base.Save(this.Type);
            element.Add(XML.SaveColor("Color", this.ColorEffect.Color));
            return element;
        }


        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.CropEffect);
        public ICanvasImage ReplaceRender(ICanvasImage background, string id, ICanvasImage mezzanine) => base.Render(background,
            (base.Id == id) ? mezzanine : this.CropEffect);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.CropEffect
        });
        public ICanvasImage ReplaceRender(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.CropEffect
        });


        public bool FillContainsPoint(Vector2 point) => true;


        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new FillLayer(resourceCreator, this.ColorEffect.Color, base.Width, base.Height);

        protected override ILayer OffsetSelf(ICanvasResourceCreator resourceCreator, Vector2 offset) => new FillLayer(resourceCreator, this.ColorEffect.Color, base.Width, base.Height);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new FillLayer(resourceCreator, this.ColorEffect.Color, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new FillLayer(resourceCreator, this.ColorEffect.Color, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new FillLayer(resourceCreator, this.ColorEffect.Color, width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => new FillLayer(resourceCreator, this.ColorEffect.Color, width, height);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip) => new FillLayer(resourceCreator, this.ColorEffect.Color, base.Width, base.Height);
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return new FillLayer(resourceCreator, this.ColorEffect.Color, base.Height, base.Width);
                default:
                    return new FillLayer(resourceCreator, this.ColorEffect.Color, base.Width, base.Height);
            }
        }
    }
}