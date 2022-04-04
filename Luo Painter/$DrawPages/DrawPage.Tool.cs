using Luo_Painter.Blends;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        ToolType ToolType = ToolType.PaintBrush;
        ToolGroupType ToolGroupType = ToolGroupType.Paint;

        BitmapLayer BitmapLayer;
        float InkSize = 22f;
        float InkOpacity = 1;
        BlendEffectMode BlendType = BlendExtensions.None;


        private void SetToolType(ToolType type)
        {
            // ToolIcon
            this.ToolResource.Source = new Uri(type.GetResource());
            this.ToolIcon.Template = type.GetTemplate(this.ToolResource);
        }
        private void SetToolGroupType(ToolGroupType groupType)
        {
            // Brush
            Color color = groupType.GetColor();
            foreach (SolidColorBrush item in this.ApplicationBrushes)
            {
                item.Color = color;
            }

            // FootGrid
            this.PaintBrushPanel.Visibility = groupType == ToolGroupType.Paint ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetBlendType(BlendEffectMode type)
        {
            // BlendIcon
            this.BlendResource.Source = new Uri(type.GetResource());
            this.BlendIcon.Template = type.GetTemplate(this.BlendResource);
        }


        private void ConstructTools()
        {
            this.SetToolType(this.ToolType);
            this.SetToolGroupType(this.ToolGroupType);
            this.ToolListView.SelectedIndex = this.ToolGroupingList.GetIndex(ToolType.PaintBrush);
            this.ToolListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ToolType item)
                {
                    if (this.ToolType == item) return;
                    this.ToolType = item;
                    this.SetToolType(item);

                    ToolGroupType groupType = this.ToolGroupingList.GetGroupType(item);

                    if (this.ToolGroupType == groupType) return;
                    this.ToolGroupType = groupType;
                    this.SetToolGroupType(groupType);
                }
            };
        }

        private void ConstructBlends()
        {
            this.SetBlendType(this.BlendType);
            this.BlendListView.SelectedIndex = this.BlendGroupingList.GetIndex(this.BlendType);
            this.BlendListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    if (this.BlendType == item) return;
                    this.BlendType = item;
                    this.SetBlendType(item);
                }
            };
        }


        private void Tool_Start(Vector2 position)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                    {
                        this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
                        if (this.BitmapLayer == null)
                        {
                            this.Tip("No Layer", "Create a new Layer?");
                            return;
                        }

                        this.BitmapLayer.InkMode = this.GetInkMode(this.ToolType == ToolType.PaintEraseBrush);
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;
                default:
                    break;
            }
        }

        private void Tool_Delta(Vector2 staringPosition, Vector2 position, float pressure)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                    if (this.BitmapLayer == null) return;
                    this.Paint_Delta(this.BitmapLayer, staringPosition, position, pressure, this.ColorPicker.Color);
                    break;
                default:
                    break;
            }
        }

        private void Tool_Complete(Vector2 position)
        {
            switch (this.ToolType)
            {
                case ToolType.PaintBrush:
                case ToolType.PaintWatercolorPen:
                case ToolType.PaintPencil:
                case ToolType.PaintEraseBrush:
                    {
                        if (this.BitmapLayer == null) return;

                        bool result = this.Paint_Complete(this.BitmapLayer);
                        if (result == false) return;

                        this.BitmapLayer = null;
                        this.CanvasControl.Invalidate(); // Invalidate

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                default:
                    break;
            }
        }

    }
}