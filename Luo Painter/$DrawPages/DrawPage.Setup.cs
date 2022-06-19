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
    public sealed partial class DrawPage : Page
    {

        private void ConstructSetup()
        {
            this.StretchDialog.PrimaryButtonClick += (s, e) =>
            {
                uint width = (uint)this.Transformer.Width;
                uint height = (uint)this.Transformer.Height;

                uint w = this.StretchDialog.Size.Width;
                uint h = this.StretchDialog.Size.Height;
                if (w == width && h == height) return;

                int w2 = (int)w;
                int h2 = (int)h;

                switch (this.StretchDialog.SelectedIndex)
                {
                    case 0:
                        CanvasImageInterpolation interpolation = this.StretchDialog.Interpolation;
                        this.Setup(h2, w2);

                        this.Setup(this.ObservableCollection.Select(c => c.Skretch(this.CanvasDevice, w2, h2, interpolation)).ToArray(), new SetupSizes
                        {
                            UndoParameter = new BitmapSize { Width = width, Height = width },
                            RedoParameter = new BitmapSize { Width = w, Height = h }
                        });
                        break;
                    case 1:
                        IndicatorMode indicator = this.StretchDialog.Indicator;

                        Vector2 vect = this.Transformer.GetIndicatorVector(indicator);
                        this.Setup(h2, w2);
                        Vector2 vect2 = this.Transformer.GetIndicatorVector(indicator);

                        Vector2 offset = vect2 - vect;
                        this.Setup(this.ObservableCollection.Select(c => c.Crop(this.CanvasDevice, w2, h2, offset)).ToArray(), new SetupSizes
                        {
                            UndoParameter = new BitmapSize { Width = width, Height = width },
                            RedoParameter = new BitmapSize { Width = w, Height = h }
                        });
                        break;
                    default:
                        break;
                }
            };

            this.SetupMenu.ItemClick += async (s, type) =>
            {
                if (this.ObservableCollection.Count is 0)
                {
                    this.Tip("No Layer", "Create a new Layer?");
                    return;
                }

                uint w = this.StretchDialog.Size.Width;
                uint h = this.StretchDialog.Size.Height;

                int w2 = (int)w;
                int h2 = (int)h;

                switch (type)
                {
                    case OptionType.CropCanvas: break;
                    case OptionType.Stretch:
                        this.SetupMenu.Hide();
                        await this.StretchDialog.ShowInstance();
                        break;
                    case OptionType.FlipHorizontal:
                        this.Setup(this.ObservableCollection.Select(c => c.FlipHorizontal(this.CanvasDevice)).ToArray(), null);
                        break;
                    case OptionType.FlipVertical:
                        this.Setup(this.ObservableCollection.Select(c => c.FlipVertical(this.CanvasDevice)).ToArray(), null);
                        break;
                    case OptionType.LeftTurn:
                        this.Setup(h2, w2);
                   
                        this.Setup(this.ObservableCollection.Select(c => c.LeftTurn(this.CanvasDevice)).ToArray(), w2 == h2 ? null : new SetupSizes
                        {
                            UndoParameter = new BitmapSize { Width = w, Height = h },
                            RedoParameter = new BitmapSize { Width = h, Height = w }
                        }); break;
                    case OptionType.RightTurn:
                        this.Setup(h2, w2);
                  
                        this.Setup(this.ObservableCollection.Select(c => c.RightTurn(this.CanvasDevice)).ToArray(), w2 == h2 ? null : new SetupSizes
                        {
                            UndoParameter = new BitmapSize { Width = w, Height = h },
                            RedoParameter = new BitmapSize { Width = h, Height = w }
                        }); break;
                    case OptionType.OverTurn: 
                        this.Setup(this.ObservableCollection.Select(c => c.OverTurn(this.CanvasDevice)).ToArray(), null); 
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

        public void Setup(ILayer[] setups, SetupSizes sizes)
        {
            int index = this.LayerListView.SelectedIndex;

            // History
            string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();
            string[] redo = setups.Select(c => c.Id).ToArray();

            this.ObservableCollection.Clear();
            foreach (ILayer item in setups)
            {
                this.Layers.Add(item.Id, item);
                this.ObservableCollection.Add(item);
            }
            this.LayerListView.SelectedIndex = index;

            // History
            int removes = this.History.Push(new ArrangeHistory(undo, redo, sizes));

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

    }
}