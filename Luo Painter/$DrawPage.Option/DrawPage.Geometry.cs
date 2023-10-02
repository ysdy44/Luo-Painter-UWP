using FanKit.Transformers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructGeometry()
        {
            // RoundRect
            this.RoundRectCornerSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.RoundRectCornerSlider.Click += (s, e) => this.NumberShowAt(this.RoundRectCornerSlider);

            // Triangle
            this.TriangleCenterSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.TriangleCenterSlider.Click += (s, e) => this.NumberShowAt(this.TriangleCenterSlider);

            // Diamond
            this.DiamondMidSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.DiamondMidSlider.Click += (s, e) => this.NumberShowAt(this.DiamondMidSlider);

            // Pentagon
            this.PentagonPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.PentagonPointsSlider.Click += (s, e) => this.NumberShowAt(this.PentagonPointsSlider);

            // Star
            this.StarPointsSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.StarPointsSlider.Click += (s, e) => this.NumberShowAt(this.StarPointsSlider, NumberPickerMode.Case0);
            this.StarInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.StarInnerRadiusSlider.Click += (s, e) => this.NumberShowAt(this.StarInnerRadiusSlider, NumberPickerMode.Case1);

            // Cog
            this.CogCountSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogCountSlider.Click += (s, e) => this.NumberShowAt(this.CogCountSlider, NumberPickerMode.Case0);
            this.CogInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogInnerRadiusSlider.Click += (s, e) => this.NumberShowAt(this.CogInnerRadiusSlider, NumberPickerMode.Case1);
            this.CogToothSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogToothSlider.Click += (s, e) => this.NumberShowAt(this.CogToothSlider, NumberPickerMode.Case2);
            this.CogNotchSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CogNotchSlider.Click += (s, e) => this.NumberShowAt(this.CogNotchSlider, NumberPickerMode.Case3);

            // Donut
            this.DonutHoleRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.DonutHoleRadiusSlider.Click += (s, e) => this.NumberShowAt(this.DonutHoleRadiusSlider);

            // Pie
            this.PieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.PieSweepAngleSlider.Click += (s, e) => this.NumberShowAt(this.PieSweepAngleSlider);

            // Cookie
            this.CookieInnerRadiusSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CookieInnerRadiusSlider.Click += (s, e) => this.NumberShowAt(this.CookieInnerRadiusSlider, NumberPickerMode.Case0);
            this.CookieSweepAngleSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.CookieSweepAngleSlider.Click += (s, e) => this.NumberShowAt(this.CookieSweepAngleSlider, NumberPickerMode.Case1);

            // Arrow
            this.ArrowWidthSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.ArrowWidthSlider.Click += (s, e) => this.NumberShowAt(this.ArrowWidthSlider);
            this.ArrowLeftTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate();
            this.ArrowRightTailComboBox.SelectionChanged += (s, e) => this.GeometryInvalidate();

            // Heart
            this.HeartSpreadSlider.ValueChanged += (s, e) => this.GeometryInvalidate();
            this.HeartSpreadSlider.Click += (s, e) => this.NumberShowAt(this.HeartSpreadSlider);
        }

        private void GeometryInvalidate()
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.CreateTransform.Transformer), this.Color);
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
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.CreateTransform.Transformer), this.Color);
            }
            this.BitmapLayer.Hit(this.CreateTransform.Transformer);

            // History
            IHistory history = this.BitmapLayer.GetBitmapHistory();
            history.Title = App.Resource.GetString(this.OptionType.ToString());
            int removes = this.History.Push(history);

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

            if (this.CreateTransform.IsMove)
                this.CreateTransform.Transformer = this.CreateTransform.StartingTransformer + (this.Position - this.StartingPosition);
            else if (this.CreateTransform.Mode == default)
                this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);
            else
                this.CreateTransform.Transformer = FanKit.Transformers.Transformer.Controller(this.CreateTransform.Mode, this.StartingPosition, this.Position, this.CreateTransform.StartingTransformer, this.IsShift, this.IsCtrl);

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.CreateGeometry(this.CanvasDevice, this.OptionType, this.CreateTransform.Transformer), this.Color);
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

            this.CreateTransform.IsMove = false;
            this.CreateTransform.Mode = default;
            this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);

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
                case OptionType.GeometryDonut:
                case OptionType.GeometryDonutTransform:
                    return TransformerGeometry.CreateDount(resourceCreator, transformerLTRB,
                        (float)(this.DonutHoleRadiusSlider.Value / 100));
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