using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Brushes;
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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        public void ConstructGeometry()
        {
            this.RoundRectCornerSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.TriangleCenterSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.DiamondMidSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.PentagonPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.StarPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.StarInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogCountSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogToothSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogNotchSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.DountHoleRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.PieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CookieInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CookieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.ArrowWidthSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.ArrowLeftTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate();
            this.ArrowRightTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate();
            this.HeartSpreadSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
        }

        private void GeometryInvalidate()
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void CancelGeometryTransform()
        {
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }
        private void PrimaryGeometryTransform()
        {
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.Color);
            }
            this.BitmapLayer.Hit(this.BoundsTransformer);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();


            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }


        private void GeometryTransform_Delta()
        {
            if (this.BitmapLayer is null) return;

            if (this.IsBoundsMove)
            {
                this.BoundsTransformer = this.StartingBoundsTransformer + (this.Position - this.StartingPosition);
            }
            else if (this.BoundsMode == default)
            {
                this.BoundsTransformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);
            }
            else
            {
                this.BoundsTransformer = FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, this.Position, this.StartingBoundsTransformer, this.IsShift, this.IsCtrl);
            }

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private void Geometry_Start()
        {
            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            if (this.OptionType.HasPreview())
            {
                // GeometryTransform_Start
                this.Transform_Start();
                return;
            }

            this.IsBoundsMove = false;
            this.BoundsMode = default;
            this.BoundsTransformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);


            this.OptionType = this.OptionType.ToGeometryTransform();
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(true); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        public CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, OptionType type, ITransformerLTRB transformerLTRB)
        {
            switch (type)
            {
                case OptionType.GeometryRectangle:
                case OptionType.GeometryRectangleTransform:
                    return TransformerGeometry.CreateRectangle(resourceCreator, transformerLTRB);
                case OptionType.GeometryEllipse:
                case OptionType.GeometryEllipseTransform:
                    return TransformerGeometry.CreateEllipse(resourceCreator, transformerLTRB);
                case OptionType.GeometryRoundRect:
                case OptionType.GeometryRoundRectTransform:
                    return TransformerGeometry.CreateRoundRect(resourceCreator, transformerLTRB,
                        (float)(this.RoundRectCornerSlider.Value / 100));
                case OptionType.GeometryTriangle:
                case OptionType.GeometryTriangleTransform:
                    return TransformerGeometry.CreateTriangle(resourceCreator, transformerLTRB,
                        (float)(this.TriangleCenterSlider.Value / 100));
                case OptionType.GeometryDiamond:
                case OptionType.GeometryDiamondTransform:
                    return TransformerGeometry.CreateDiamond(resourceCreator, transformerLTRB,
                        (float)(this.DiamondMidSlider.Value / 100));
                case OptionType.GeometryPentagon:
                case OptionType.GeometryPentagonTransform:
                    return TransformerGeometry.CreatePentagon(resourceCreator, transformerLTRB,
                        (int)this.PentagonPointsSlider.Value);
                case OptionType.GeometryStar:
                case OptionType.GeometryStarTransform:
                    return TransformerGeometry.CreateStar(resourceCreator, transformerLTRB,
                        (int)this.StarPointsSlider.Value,
                        (float)(this.StarInnerRadiusSlider.Value / 100));
                case OptionType.GeometryCog:
                case OptionType.GeometryCogTransform:
                    return TransformerGeometry.CreateCog(resourceCreator, transformerLTRB,
                        (int)this.CogCountSlider.Value,
                        (float)(this.CogInnerRadiusSlider.Value / 100),
                        (float)(this.CogToothSlider.Value / 100),
                        (float)(this.CogNotchSlider.Value / 100));
                case OptionType.GeometryDount:
                case OptionType.GeometryDountTransform:
                    return TransformerGeometry.CreateDount(resourceCreator, transformerLTRB,
                        (float)(this.DountHoleRadiusSlider.Value / 100));
                case OptionType.GeometryPie:
                case OptionType.GeometryPieTransform:
                    return TransformerGeometry.CreatePie(resourceCreator, transformerLTRB,
                        (float)this.PieSweepAngleSlider.Value * FanKit.Math.Pi / 180);
                case OptionType.GeometryCookie:
                case OptionType.GeometryCookieTransform:
                    return TransformerGeometry.CreateCookie(resourceCreator, transformerLTRB,
                        (float)this.CookieInnerRadiusSlider.Value / 100,
                        (float)this.CookieSweepAngleSlider.Value * FanKit.Math.Pi / 180);
                case OptionType.GeometryArrow:
                case OptionType.GeometryArrowTransform:
                    return TransformerGeometry.CreateArrow(resourceCreator, transformerLTRB,
                        false, 10,
                        (float)(this.ArrowWidthSlider.Value / 100),
                        this.ArrowLeftTailComboBox.SelectedIndex is 0 ?
                            GeometryArrowTailType.None :
                            GeometryArrowTailType.Arrow,
                        this.ArrowRightTailComboBox.SelectedIndex is 0 ?
                            GeometryArrowTailType.None :
                            GeometryArrowTailType.Arrow);
                case OptionType.GeometryCapsule:
                case OptionType.GeometryCapsuleTransform:
                    return TransformerGeometry.CreateCapsule(resourceCreator, transformerLTRB);
                case OptionType.GeometryHeart:
                case OptionType.GeometryHeartTransform:
                    return TransformerGeometry.CreateHeart(resourceCreator, transformerLTRB,
                        (float)(this.HeartSpreadSlider.Value / 100));
                default:
                    return TransformerGeometry.CreateRectangle(resourceCreator, transformerLTRB);
            }
        }

    }
}