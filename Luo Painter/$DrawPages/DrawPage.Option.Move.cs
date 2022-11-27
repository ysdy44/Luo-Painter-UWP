using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        public void ConstructMove()
        {
            this.MoveXButton.Click += (s, e) => this.NumberShowAt(this.MoveXButton, NumberPickerMode.Case0);
            this.MoveYButton.Click += (s, e) => this.NumberShowAt(this.MoveYButton, NumberPickerMode.Case1);
        }

        private void ResetMove()
        {
            // Move
            this.Move = Vector2.Zero;

            this.MoveXButton.Number = (int)this.Move.X;
            this.MoveYButton.Number = (int)this.Move.Y;
        }

        private void SetMove(NumberPickerMode mode, int e)
        {
            switch (mode)
            {
                case NumberPickerMode.Case0:
                    this.Move.X = e;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.MoveXButton.Number = e;
                    break;
                case NumberPickerMode.Case1:
                    this.Move.Y = e;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.MoveYButton.Number = e;
                    break;
                default:
                    break;
            }
        }

        private void Move_Start()
        {
            this.StartingMove = this.Move;

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Move_Delta()
        {
            this.Move = this.Position - this.StartingPosition + this.StartingMove;

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Move_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.MoveXButton.Number = (int)this.Move.X;
            this.MoveYButton.Number = (int)this.Move.Y;
        }

    }
}