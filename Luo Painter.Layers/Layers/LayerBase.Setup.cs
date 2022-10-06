using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase : IRender, IDisposable
    {

        protected abstract ILayer CloneSelf(ICanvasResourceCreator resourceCreator);

        protected abstract ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height);
        protected abstract ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset);
        protected abstract ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation);

        protected abstract ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation);
        protected abstract ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip);
        protected abstract ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation);


        public ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            ILayer layer = this.CloneSelf(resourceCreator);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Clone(resourceCreator));
            return layer;
        }

        public ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            ILayer layer = this.CropSelf(resourceCreator, width, height);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Crop(resourceCreator, width, height));
            return layer;
        }
        public ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset)
        {
            ILayer layer = this.CropSelf(resourceCreator, width, height, offset);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Crop(resourceCreator, width, height, offset));
            return layer;
        }
        public ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation)
        {
            ILayer layer = this.CropSelf(resourceCreator, width, height, matrix, interpolation);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Crop(resourceCreator, width, height, matrix, interpolation));
            return layer;
        }

        public ILayer Skretch(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation)
        {
            ILayer layer = this.SkretchSelf(resourceCreator, width, height, interpolation);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Skretch(resourceCreator, width, height, interpolation));
            return layer;
        }
        public ILayer Flip(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            ILayer layer = this.FlipSelf(resourceCreator, flip);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Flip(resourceCreator, flip));
            return layer;
        }
        public ILayer Rotation(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            ILayer layer = this.RotationSelf(resourceCreator, rotation);
            layer.CopyWith(this);
            foreach (ILayer item in this.Children) layer.Children.Add(item.Rotation(resourceCreator, rotation));
            return layer;
        }

    }
}