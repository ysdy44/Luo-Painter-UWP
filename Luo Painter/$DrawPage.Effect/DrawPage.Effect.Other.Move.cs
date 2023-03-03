using System.Numerics;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructMove()
        {
            this.MovePicker.XClick += (s, e) => this.NumberShowAt(this.MovePicker.XNumber, NumberPickerMode.Case0);
            this.MovePicker.YClick += (s, e) => this.NumberShowAt(this.MovePicker.YNumber, NumberPickerMode.Case1);
        }

        private void ResetMove()
        {
            // Move
            this.Move = Vector2.Zero;

            this.MovePicker.Value = this.Move;
        }

        private void SetMove(NumberPickerMode mode, float e)
        {
            switch (mode)
            {
                case NumberPickerMode.Case0:
                    this.Move.X = e;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.MovePicker.X = e;
                    break;
                case NumberPickerMode.Case1:
                    this.Move.Y = e;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.MovePicker.Y = e;
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

            this.MovePicker.Value = this.Move;
        }

    }
}