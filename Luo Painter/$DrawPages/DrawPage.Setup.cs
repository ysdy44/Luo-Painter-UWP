using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System.Linq;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void ConstructSetup()
        {
            this.CropCanvasSlider.ValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * System.Math.PI;
                this.Transformer.Radian = (float)radian;
                this.Transformer.ReloadMatrix();

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.StretchDialog.PrimaryButtonClick += (s, e) =>
            {
                int width2 = this.Transformer.Width;
                int height2 = this.Transformer.Height;

                uint width = (uint)width2;
                uint height = (uint)height2;

                uint w = this.StretchDialog.Size.Width;
                uint h = this.StretchDialog.Size.Height;
                if (w == width && h == height) return;

                int w2 = (int)w;
                int h2 = (int)h;

                switch (this.StretchDialog.SelectedIndex)
                {
                    case 0:
                        {
                            CanvasImageInterpolation interpolation = this.StretchDialog.Interpolation;
                            this.Setup(h2, w2);

                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.Skretch(this.CanvasDevice, w2, h2, interpolation)).ToArray(), new SetupSizes
                            {
                                UndoParameter = new BitmapSize { Width = width, Height = height },
                                RedoParameter = new BitmapSize { Width = w, Height = h }
                            }));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case 1:
                        {
                            IndicatorMode indicator = this.StretchDialog.Indicator;

                            Vector2 vect = this.Transformer.GetIndicatorVector(indicator);
                            this.Setup(h2, w2);
                            Vector2 vect2 = this.Transformer.GetIndicatorVector(indicator);

                            Vector2 offset = vect2 - vect;
                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.Crop(this.CanvasDevice, w2, h2, offset)).ToArray(), new SetupSizes
                            {
                                UndoParameter = new BitmapSize { Width = width, Height = height },
                                RedoParameter = new BitmapSize { Width = w, Height = h }
                            }));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        break;
                }
            };

            this.OtherMenu.ItemClick += async (s, type) =>
            {
                switch (type)
                {
                    case OptionType.CropCanvas:
                    case OptionType.Stretch:
                    case OptionType.FlipHorizontal:
                    case OptionType.FlipVertical:
                    case OptionType.LeftTurn:
                    case OptionType.RightTurn:
                    case OptionType.OverTurn:
                        break;
                    default:
                        return;
                }

                int width2 = this.Transformer.Width;
                int height2 = this.Transformer.Height;

                uint width = (uint)width2;
                uint height = (uint)height2;

                switch (type)
                {
                    case OptionType.CropCanvas:
                        this.ExpanderLightDismissOverlay.Hide();

                        this.SetCropCanvas(width2, height2);

                        this.OptionType = OptionType.CropCanvas;
                        this.SetFootType(OptionType.CropCanvas);
                        this.SetCanvasState(true);
                        break;
                    case OptionType.Stretch:
                        this.ExpanderLightDismissOverlay.Hide();

                        await this.StretchDialog.ShowInstance();
                        break;
                    case OptionType.FlipHorizontal:
                        {
                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.FlipHorizontal(this.CanvasDevice)).ToArray(), null));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case OptionType.FlipVertical:
                        {
                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.FlipVertical(this.CanvasDevice)).ToArray(), null));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case OptionType.LeftTurn:
                        {
                            this.Setup(height2, width2);

                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.LeftTurn(this.CanvasDevice)).ToArray(), width2 == height2 ? null : new SetupSizes
                            {
                                UndoParameter = new BitmapSize { Width = width, Height = height },
                                RedoParameter = new BitmapSize { Width = height, Height = width }
                            }));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case OptionType.RightTurn:
                        {
                            this.Setup(height2, width2);

                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.RightTurn(this.CanvasDevice)).ToArray(), width2 == height2 ? null : new SetupSizes
                            {
                                UndoParameter = new BitmapSize { Width = width, Height = height },
                                RedoParameter = new BitmapSize { Width = height, Height = width }
                            }));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case OptionType.OverTurn:
                        {
                            int removes = this.History.Push(this.Setup(this.Nodes.Select(c => c.OverTurn(this.CanvasDevice)).ToArray(), null));

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    default:
                        break;
                }
            };
        }

        public void Setup(BitmapSize size) => this.Setup((int)size.Width, (int)size.Height);
        public void Setup(int w, int h)
        {
            this.Transformer.Width = w;
            this.Transformer.Height = h;
            this.Transformer.ReloadMatrix();

            this.Mesh = new Mesh(this.CanvasDevice, this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(25), w, h);
        }

    }
}