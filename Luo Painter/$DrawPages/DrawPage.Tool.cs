﻿using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructTools()
        {
            this.ToolListView.ItemClick += (groupType, type) =>
            {
                this.ToolType = type;

                this.ToolResource.Source = new Uri(type.GetResource());
                this.ToolIcon.Template = type.GetTemplate(this.ToolResource);

                this.ToolGroupSwitchPresenter.EvaluateCases(groupType, type);
                this.ToolGroupSwitchPresenter2.EvaluateCases(groupType, type);
            };
            this.ToolListView.Construct(this.ToolType);
        }


        private void Tool_Start(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.None:
                    switch (this.ToolType)
                    {
                        case ToolType.PaintBrush:
                        case ToolType.PaintWatercolorPen:
                        case ToolType.PaintPencil:
                        case ToolType.PaintEraseBrush:
                        case ToolType.PaintLiquefaction:
                            this.Paint_Start(point, pressure);
                            break;
                        case ToolType.MarqueeRectangular:
                        case ToolType.MarqueeElliptical:
                        case ToolType.MarqueePolygon:
                        case ToolType.MarqueeFreeHand:
                            this.Marquee_Start(point);
                            break;
                        case ToolType.SelectionBrush:
                            this.Position = this.ToPosition(point);
                            break;
                        case ToolType.View:
                            this.View_Start(point);
                            break;
                        default:
                            break;
                    }
                    break;
                case OptionType.Transform:
                    this.Transform_Start(point);
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Start(point);
                    break;
                default:
                    break;
            }
        }

        private void Tool_Delta(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.None:
                    switch (this.ToolType)
                    {
                        case ToolType.PaintBrush:
                        case ToolType.PaintWatercolorPen:
                        case ToolType.PaintPencil:
                        case ToolType.PaintEraseBrush:
                        case ToolType.PaintLiquefaction:
                            this.Paint_Delta(point, pressure);
                            break;
                        case ToolType.MarqueeRectangular:
                        case ToolType.MarqueeElliptical:
                        case ToolType.MarqueePolygon:
                        case ToolType.MarqueeFreeHand:
                            this.Marquee_Delta(point);
                            break;
                        case ToolType.SelectionBrush:
                            Vector2 position = this.ToPosition(point);
                            this.Marquee.Marquee(this.Position, position, 32, false);
                            this.Marquee.Hit(RectExtensions.GetRect(this.Position, position, 32));
                            this.Position = position;
                            break;
                        case ToolType.View:
                            this.View_Delta(point);
                            break;
                        default:
                            break;
                    }
                    break;
                case OptionType.Transform:
                    this.Transform_Delta(point);
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Delta(point);
                    break;
                default:
                    break;
            }
        }

        private void Tool_Complete(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.None:
                    switch (this.ToolType)
                    {
                        case ToolType.PaintBrush:
                        case ToolType.PaintWatercolorPen:
                        case ToolType.PaintPencil:
                        case ToolType.PaintEraseBrush:
                        case ToolType.PaintLiquefaction:
                            this.Paint_Complete(point, pressure);
                            break;
                        case ToolType.MarqueeRectangular:
                        case ToolType.MarqueeElliptical:
                        case ToolType.MarqueePolygon:
                        case ToolType.MarqueeFreeHand:
                            this.Marquee_Complete(point);
                            break;
                        case ToolType.SelectionFlood:
                            if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                            {
                                this.SelectionFlood(point, bitmapLayer);
                            }
                            break;
                        case ToolType.SelectionBrush:
                            {
                                // History
                                int removes = this.History.Push(this.Marquee.GetBitmapHistory());
                                this.Marquee.Flush();
                                this.Marquee.RenderThumbnail();

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                            }
                            break;
                        case ToolType.Cursor:
                            this.Position = this.ToPosition(point);
                            this.LayerListView.SelectedIndex = this.FillContainsPoint(this.Position);
                            break;
                        case ToolType.View:
                            this.View_Complete(point);
                            break;
                        default:
                            break;
                    }
                    break;
                case OptionType.Transform:
                    this.Transform_Complete(point);
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Complete(point);
                    break;
                default:
                    break;
            }
        }

    }
}