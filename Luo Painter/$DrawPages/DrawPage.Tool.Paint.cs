using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        Vector2 Point;
        Vector2 Position;
        float Pressure;


        private BitmapType GetBitmapType(InkType type)
        {
            if (type.HasFlag(InkType.Pattern))
                return BitmapType.Temp;
            else if (type.HasFlag(InkType.Opacity))
                return BitmapType.Temp;
            else if (type.HasFlag(InkType.BlendMode))
                return BitmapType.Temp;
            else
                return BitmapType.Source;
        }


        private void Paint_Start(Vector2 point, float pressure)
        {
            this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.Point = point;
            this.Position = this.ToPosition(point);
            this.Pressure = pressure;

            this.InkType = this.InkPresenter.GetType(this.InkToolType);
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Paint_Delta(Vector2 point, float pressure)
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            Vector2 position = this.ToPosition(point);

            Rect rect = this.Position.GetRect(this.InkPresenter.Size);
            this.BitmapLayer.Hit(rect);

            if (this.Paint(this.BitmapLayer, position, pressure) is false) return;

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.Point = point;
            this.Position = position;
            this.Pressure = pressure;
        }

        private void Paint_Complete(Vector2 point, float pressure)
        {
            this.Paint_Delta(point, pressure);
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            if (this.InkType.HasFlag(InkType.BlendMode))
                this.BitmapLayer.DrawCopy(this.InkPresenter.GetWetPreview(this.InkType, this.BitmapLayer.Temp, this.BitmapLayer.Origin));
            else if (this.InkType.HasFlag(InkType.Opacity) || this.InkType.HasFlag(InkType.Pattern))
                this.BitmapLayer.Draw(this.InkPresenter.GetWetPreview(this.InkType, this.BitmapLayer.Temp));

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.InkType = default;
            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private bool Paint(BitmapLayer bitmapLayer, Vector2 position, float pressure)
        {
            switch (this.InkToolType)
            {
                case InkType.BrushDry:
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardness(
                        this.BrushEdgeHardnessShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        this.GetBitmapType(this.InkType));
                case InkType.MaskBrushDry:
                    return bitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        this.ColorMenu.ColorHdr,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness,
                        this.GetBitmapType(this.InkType));
                case InkType.CircleDry:
                    return bitmapLayer.IsometricFillCircle(
                        this.ColorMenu.Color,
                        this.Position, position,
                        this.Pressure, pressure,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        this.GetBitmapType(this.InkType));
                case InkType.LineDry:
                    bitmapLayer.DrawLine(this.Position, position, this.ColorMenu.Color, this.InkPresenter.Size, this.GetBitmapType(this.InkType));
                    return true;
                case InkType.EraseDry:
                    if (this.InkType.HasFlag(InkType.Pattern) || this.InkType.HasFlag(InkType.Opacity))
                        return bitmapLayer.IsometricErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
                    else
                        return bitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
                case InkType.Liquefy:
                    bitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = bitmapLayer.Source,
                        Properties =
                        {
                            ["radius"] = bitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                            ["position"] = bitmapLayer .ConvertValueToOne(this.Position),
                            ["targetPosition"] = bitmapLayer.ConvertValueToOne(position),
                            ["pressure"] = pressure,
                        }
                    }, RectExtensions.GetRect(this.Position, position, this.InkPresenter.Size));
                    return true;
                default:
                    return false;
            }
        }

    }
}