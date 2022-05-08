using Luo_Painter.Blends;
using Luo_Painter.Edits;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Tools
{
    internal sealed class InkRender
    {
        readonly BitmapLayer PaintLayer;
        public ICanvasImage Souce => this.PaintLayer.Source;
        public InkRender(CanvasControl sender) => this.PaintLayer = new BitmapLayer(sender, (int)sender.ActualWidth, (int)sender.ActualHeight);
        public void Render(float size, Color color)
        {
            this.PaintLayer.Clear(Colors.Transparent, BitmapType.Origin);
            this.PaintLayer.Clear(Colors.Transparent, BitmapType.Source);

            float width = this.PaintLayer.Width;
            float height = this.PaintLayer.Height;
            float space = System.Math.Max(2, size / height * 2);

            Vector2 position = new Vector2(10, height / 2 + 3.90181f);
            float pressure = 0.001f;

            for (float x = 10; x < width - 10; x += space)
            {
                // 0 ~ Π
                float radian = x / width * FanKit.Math.Pi;

                // Sin 0 ~ Π ︵
                float targetPressure = (float)System.Math.Sin(radian);
                // Sin 0 ~ 2Π ~
                float offsetY = 20 * (float)System.Math.Sin(radian + radian);
                Vector2 targetPosition = new Vector2(x, height / 2 + offsetY);

                this.PaintLayer.FillCircleDry(position, targetPosition, pressure, targetPressure, space, color);
                position = targetPosition;
                pressure = targetPressure;
            }
        }
    }

    internal sealed class BlendIcon : TIcon<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource, out string title),
                Resources = resource,
            }, title);
        }
    }

    internal sealed class BlendList : List<BlendEffectMode> { }

    public sealed partial class PaintTool : UserControl
    {

        public float InkSize { get; private set; } = 22f;
        public float InkOpacity { get; private set; } = 1;
        public BlendEffectMode? InkBlendMode { get; private set; } = null;

        InkRender InkRender;

        //@Construct
        public PaintTool()
        {
            this.InitializeComponent();

            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new InkRender(sender);
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.InkRender.Render(this.InkSize, Colors.Black);
                        break;
                    case ElementTheme.Dark:
                        this.InkRender.Render(this.InkSize, Colors.White);
                        break;
                }
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.InkRender == null) return;
                args.DrawingSession.DrawImage(this.InkRender.Souce);
            };

            this.SizeSlider.ValueChanged += (s, e) =>
            {
                this.InkSize = (float)e.NewValue;

                if (this.InkRender == null) return;

                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.InkRender.Render(this.InkSize, Colors.Black);
                        break;
                    case ElementTheme.Dark:
                        this.InkRender.Render(this.InkSize, Colors.White);
                        break;
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)(e.NewValue / 100);
                this.CanvasControl.Opacity = this.InkOpacity;
            };
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    bool isNone = item.IsNone();

                    if (isNone) this.InkBlendMode = null;
                    else this.InkBlendMode = item;
                }
            };

            this.BlendModeListView.SelectedIndex = this.InkBlendMode.HasValue ? this.BlendCollection.IndexOf(this.InkBlendMode.Value) : 0;
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    if (item.IsNone())
                    {
                        if (this.InkBlendMode == null) return;
                        this.InkBlendMode = null;
                    }
                    else
                    {
                        if (this.InkBlendMode == item) return;
                        this.InkBlendMode = item;
                    }
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public ICanvasImage GetInk(BitmapLayer bitmapLayer)
        {
            switch (bitmapLayer.InkMode)
            {
                case InkMode.WetWithOpacity:
                    return bitmapLayer.GetWeting(this.InkOpacity);
                case InkMode.WetWithBlendMode:
                    return bitmapLayer.GetWeting(this.InkBlendMode.Value);
                case InkMode.WetWithOpacityAndBlendMode:
                    return bitmapLayer.GetWeting(this.InkOpacity, this.InkBlendMode.Value);

                case InkMode.EraseWetWithOpacity:
                    return bitmapLayer.GetEraseWeting(this.InkOpacity);

                default:
                    return bitmapLayer.Source;
            }
        }

        public InkMode GetInkMode(bool isErase, bool isLiquefaction)
        {
            if (isLiquefaction) return InkMode.Liquefy;

            if (isErase)
            {
                if (this.InkOpacity == 1f) return InkMode.EraseDry;
                else return InkMode.EraseWetWithOpacity;
            }

            if (this.InkBlendMode.HasValue == false)
            {
                if (this.InkOpacity == 1f) return InkMode.Dry;
                else return InkMode.WetWithOpacity;
            }

            if (this.InkOpacity == 1f) return InkMode.WetWithBlendMode;
            else return InkMode.WetWithOpacityAndBlendMode;
        }

    }
}