﻿using Luo_Painter.Blends;
using Luo_Painter.Layers.Models;
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

        private void SetToolType(ToolType type)
        {
            // ToolIcon
            this.ToolResource.Source = new Uri(type.GetResource());
            this.ToolIcon.Template = type.GetTemplate(this.ToolResource);

            ToolGroupType groupType = this.ToolCollection[type];
            this.ToolGroupSwitchPresenter.EvaluateCases(groupType, type);
            this.ToolGroupSwitchPresenter2.EvaluateCases(groupType, type);
        }


        private void ConstructTools()
        {
            this.ToolButton.Click += (s, e) =>
            {
                this.SetPaint();
                this.ToolFlyout.ShowAt(this.ToolButton);
            };

            this.SetToolType(this.ToolType);
            this.ToolListView.SelectedIndex = this.ToolCollection.IndexOf(this.ToolType);
            this.ToolListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ToolType item)
                {
                    if (this.ToolType == item) return;
                    this.ToolType = item;
                    this.SetToolType(item);
                }
            };
        }

        private void ConstructBlends()
        {
            this.PaintBlendModeListView.SelectedIndex = this.InkBlendMode.HasValue ? this.BlendCollection.IndexOf(this.InkBlendMode.Value) : 0;
            this.PaintBlendModeListView.ItemClick += (s, e) =>
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


        private void Tool_Start(Vector2 point, PointerPointProperties properties)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                case ToolType.PaintLiquefaction:
                    this.Paint_Start(point, properties);
                    break;
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                case ToolType.MarqueeSelectionBrush:
                    // case ToolType.MarqueeFloodSelect:
                    this.Marquee_Start(point);
                    break;
                default:
                    break;
            }
        }

        private void Tool_Delta(Vector2 point, PointerPointProperties properties)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                case ToolType.PaintLiquefaction:
                    this.Paint_Delta(point, properties);
                    break;
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                case ToolType.MarqueeSelectionBrush:
                    // case ToolType.MarqueeFloodSelect:
                    this.Marquee_Delta(point);
                    break;
                default:
                    break;
            }
        }

        private void Tool_Complete(Vector2 point, PointerPointProperties properties)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                case ToolType.PaintLiquefaction:
                    this.Paint_Complete(point, properties);
                    break;
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                case ToolType.MarqueeSelectionBrush:
                case ToolType.MarqueeFloodSelect:
                    this.Marquee_Complete(point);
                    break;
                default:
                    break;
            }
        }

    }
}