using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers.Models
{
    public sealed class ImageLayer : LayerBase, ILayer, ICacheTransform
    {
        //@Static
        public static readonly IDictionary<string, CanvasBitmap> Instance = new Dictionary<string, CanvasBitmap>();

        float SourceWidth;
        float SourceHeight;
        Transformer StartingDestination;
        Transformer Destination;
        public Matrix3x2 Matrix { get; private set; }
        public Rect Rect { get; private set; }
        public string Photo { get; private set; }

        public LayerType Type => LayerType.Image;
        public ICanvasImage this[BitmapType type] => ImageLayer.Instance[this.Photo];


        //@Construct
        private ImageLayer(ICanvasResourceCreator resourceCreator, ImageLayer imageLayer, int width, int height) : this(resourceCreator, imageLayer, imageLayer.Destination, width, height) { }
        private ImageLayer(ICanvasResourceCreator resourceCreator, ImageLayer imageLayer, Transformer transformer, int width, int height) : base(null, null, resourceCreator, width, height)
        {
            this.SourceWidth = imageLayer.SourceWidth;
            this.SourceHeight = imageLayer.SourceHeight;
            this.Destination = transformer;
            this.Matrix = Transformer.FindHomography(this.SourceWidth, this.SourceHeight, transformer);

            this.Rect = new Rect(0, 0, width, height);

            this.Photo = imageLayer.Photo;
            CanvasBitmap bitmap = ImageLayer.Instance[imageLayer.Photo];
            this.RenderThumbnail(bitmap);
        }

        public ImageLayer(ICanvasResourceCreator resourceCreator, string photo, int width, int height) : this(null, null, resourceCreator, photo, width, height) { }
        public ImageLayer(string id, XElement element, ICanvasResourceCreator resourceCreator, string photo, int width, int height) : base(id, element, resourceCreator, width, height)
        {
            CanvasBitmap bitmap = ImageLayer.Instance[photo];
            float w = bitmap.SizeInPixels.Width;
            float h = bitmap.SizeInPixels.Height;

            if (element is null is false && element.Element("Transformer") is XElement transformer)
            {
                this.SourceWidth = w;
                this.SourceHeight = h;
                this.Destination = XML.LoadTransformer(transformer);
                this.Matrix = Transformer.FindHomography(this.SourceWidth, this.SourceHeight, this.Destination);
            }
            else
            {
                this.SourceWidth = w;
                this.SourceHeight = h;
                this.Destination = new Transformer(w, h, Vector2.Zero);
                this.Matrix = Matrix3x2.Identity;
            }

            this.Rect = new Rect(0, 0, width, height);

            this.Photo = photo;
            this.RenderThumbnail(bitmap);
        }

        public XElement Save()
        {
            XElement element = base.Save(this.Type);
            element.Add(new XElement("Photo", this.Photo));
            element.Add(XML.SaveTransformer("Transformer", this.Destination));
            return element;
        }


        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.GetRender());
        public ICanvasImage ReplaceRender(ICanvasImage background, string id, ICanvasImage mezzanine) => base.Render(background,
            (base.Id == id) ? mezzanine : this.GetRender());
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.GetRender()
        });
        public ICanvasImage ReplaceRender(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.GetRender()
        });
        private ICanvasImage GetRender() => new CropEffect
        {
            SourceRectangle = this.Rect,
            Source = new Transform2DEffect
            {
                TransformMatrix = this.Matrix,
                Source = ImageLayer.Instance[this.Photo]
            }
        };


        public bool FillContainsPoint(Vector2 point) => this.Destination.FillContainsPoint(point);


        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new ImageLayer(resourceCreator, this, base.Width, base.Height);

        protected override ILayer OffsetSelf(ICanvasResourceCreator resourceCreator, Vector2 offset) => new ImageLayer(resourceCreator, this, this.Destination + offset, base.Width, base.Height);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new ImageLayer(resourceCreator, this, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new ImageLayer(resourceCreator, this, this.Destination + offset, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new ImageLayer(resourceCreator, this, this.Destination * matrix, width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => this.CropSelf(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)base.Width, height / (float)base.Height), interpolation);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, 1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapFlip.Vertical:
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return this.CropSelf(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(base.Height, 0), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return this.CropSelf(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, base.Width), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

        public void CacheTransform()
        {
            this.StartingDestination = this.Destination;
        }
        public void TransformMultiplies(Matrix3x2 matrix)
        {
            this.Destination = this.StartingDestination * matrix;
            this.Matrix = Transformer.FindHomography(this.SourceWidth, this.SourceHeight, this.Destination);
        }
        public void TransformAdd(Vector2 vector)
        {
            this.Destination = this.StartingDestination + vector;
            this.Matrix = Transformer.FindHomography(this.SourceWidth, this.SourceHeight, this.Destination);
        }
    }
}