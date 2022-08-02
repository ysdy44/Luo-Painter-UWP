using FanKit.Transformers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void Geometry_Start(Vector2 position, Vector2 point)
        {
            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.BoundsTransformer = new Transformer(this.StartingPosition, position, this.IsCtrl, this.IsShift);
           
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Geometry_Delta(Vector2 position, Vector2 point)
        {
            if (this.BitmapLayer is null) return;

            this.BoundsTransformer = new Transformer(this.StartingPosition, position, this.IsCtrl, this.IsShift);

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.CreateGeometry(this.OptionType, this.BoundsTransformer), this.ColorMenu.Color);
            }

            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Geometry_Complete(Vector2 position, Vector2 point)
        {
            if (this.BitmapLayer is null) return;

            this.BoundsTransformer = new Transformer(this.StartingPosition, position, this.IsCtrl, this.IsShift);

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                ds.FillGeometry(this.CreateGeometry(this.OptionType, this.BoundsTransformer), this.ColorMenu.Color);
            }
            this.BitmapLayer.Hit(this.BoundsTransformer);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.BitmapLayer = null;
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private CanvasGeometry CreateGeometry(OptionType type, ITransformerLTRB transformerLTRB)
        {
            switch (type)
            {
                case OptionType.GeometryRectangle:
                    return TransformerGeometry.CreateRectangle(this.CanvasDevice, transformerLTRB);
                case OptionType.GeometryEllipse:
                    return TransformerGeometry.CreateEllipse(this.CanvasDevice, transformerLTRB);
                case OptionType.GeometryRoundRect:
                    return TransformerGeometry.CreateRoundRect(this.CanvasDevice, transformerLTRB, 0.5f);
                case OptionType.GeometryTriangle:
                    return TransformerGeometry.CreateTriangle(this.CanvasDevice, transformerLTRB, 0.5f);
                case OptionType.GeometryDiamond:
                    return TransformerGeometry.CreateDiamond(this.CanvasDevice, transformerLTRB, 0.5f);
                case OptionType.GeometryPentagon:
                    return TransformerGeometry.CreatePentagon(this.CanvasDevice, transformerLTRB, 5);
                case OptionType.GeometryStar:
                    return TransformerGeometry.CreateStar(this.CanvasDevice, transformerLTRB, 5, 0.5f);
                case OptionType.GeometryCog:
                    return TransformerGeometry.CreateCog(this.CanvasDevice, transformerLTRB, 5, 0.5f, 0.5f, 0.5f);
                case OptionType.GeometryDount:
                    return TransformerGeometry.CreateDount(this.CanvasDevice, transformerLTRB, 0.5f);
                case OptionType.GeometryPie:
                    return TransformerGeometry.CreatePie(this.CanvasDevice, transformerLTRB, FanKit.Math.Pi);
                case OptionType.GeometryCookie:
                    return TransformerGeometry.CreateCookie(this.CanvasDevice, transformerLTRB, FanKit.Math.Pi, FanKit.Math.Pi);
                case OptionType.GeometryArrow:
                    return TransformerGeometry.CreateArrow(this.CanvasDevice, transformerLTRB);
                case OptionType.GeometryCapsule:
                    return TransformerGeometry.CreateCapsule(this.CanvasDevice, transformerLTRB);
                case OptionType.GeometryHeart:
                    return TransformerGeometry.CreateHeart(this.CanvasDevice, transformerLTRB, 0.5f);
                default:
                    return TransformerGeometry.CreateRectangle(this.CanvasDevice, transformerLTRB);
            }
        }

    }
}