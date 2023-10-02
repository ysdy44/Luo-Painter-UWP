using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        int StrawMode => this.StrawComboBox.SelectedIndex;


        private void Straw_Start()
        {
            this.Straw();
            this.StrawCanvasControl.Invalidate();

            this.StrawViewer.Move(this.Point.X, this.Point.Y);
            this.StrawViewer.Show();
        }
        private void Straw_Delta()
        {
            this.StrawViewer.Move(this.Point.X, this.Point.Y);

            this.Straw();
            this.StrawCanvasControl.Invalidate();

            switch (this.StrawMode)
            {
                case 0:
                    Color color1 = this.StrawViewer.GetStraw();
                    this.ColorButton.OnColorChanged(color1, ColorChangedMode.WithPrimaryBrush);
                    break;
            }
        }
        private void Straw_Complete()
        {
            this.StrawViewer.Hide();
            switch (this.StrawMode)
            {
                case 0:
                    this.Straw();

                    Color color1 = this.StrawViewer.GetStraw();
                    this.ColorButton.Recolor(color1);
                    this.ColorButton.OnColorChanged(color1);
                    break;
                default:
                    if (this.LayerSelectedItem is null)
                    {
                        this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                        break;
                    }

                    if (this.LayerSelectedItem is BitmapLayer bitmapLayer is false)
                    {
                        this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                        break;
                    }

                    int x = (int)this.Position.X;
                    int y = (int)this.Position.Y;

                    if (bitmapLayer.Contains(x, y) is false) break;

                    Color color2 = bitmapLayer.GetPixelColor(x, y, BitmapType.Source);
                    if (color2 == default) break;

                    this.ColorButton.Recolor(color2);
                    this.ColorButton.OnColorChanged(color2);
                    break;
            }
        }

    }
}