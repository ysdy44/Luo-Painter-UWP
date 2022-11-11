using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Placement of <see cref="Marble"/>.
    /// </summary>
    public enum MarblePlacement
    {
        Default,
        Minimum,
        Maximum,
    }

    /// <summary>
    /// Direction of <see cref="Marble"/>.
    /// </summary>
    public enum MarbleDirection
    {
        None,
        Positive,
        Negative,
    }

    /// <summary>
    /// Represents a computational class,
    /// that manipulates <see cref="UIElement.RenderTransform"/> 
    /// by <see cref="UIElement.ManipulationDelta"/> events, 
    /// like the Space Marbles.
    /// </summary>
    /// <code>
    /// 
    ///    readonly int Radius = 100;
    ///    readonly Marble MarbleX = new Marble();
    ///    readonly Marble MarbleY = new Marble();
    ///    readonly TranslateTransform TranslateTransform = new TranslateTransform();
    ///    
    ///    this.UIElement.Width = 100 + 100;
    ///    this.UIElement.Height = 100 + 100;
    ///    this.UIElement.RenderTransform = this.TranslateTransform;
    ///    this.UIElement.ManipulationMode = ManipulationModes.All;
    ///    this.UIElement.ManipulationDelta += (s, e) =>
    ///    {
    ///       this.TranslateTransform.X = this.MarbleX.Move(this.TranslateTransform.X, e.Delta.Translation.X, e.IsInertial, 100, base.ActualWidth - 100);
    ///       this.TranslateTransform.Y = this.MarbleY.Move(this.TranslateTransform.Y, e.Delta.Translation.Y, e.IsInertial, 100, base.ActualHeight - 100);
    ///    };
    ///    
    /// </code>
    public sealed class Marble
    {

        /// <summary>
        /// Placement of <see cref="Marble"/>.
        /// </summary>
        public MarblePlacement Placement { get; private set; }
        /// <summary>
        /// Direction of <see cref="Marble"/>.
        /// </summary>
        public MarbleDirection Direction { get; private set; }

        /// <summary>
        /// Move the transform.
        /// </summary>
        /// <param name="transform"> The transform. </param>
        /// <param name="delta"> <see cref="ManipulationDeltaRoutedEventArgs.Delta"/> </param>
        /// <param name="isInertial"> <see cref="ManipulationDeltaRoutedEventArgs.IsInertial"/> </param>
        /// <param name="minimum"> The minimum region. </param>
        /// <param name="maximum"> The maximum region. </param>
        /// <returns> The product transform. </returns>
        public double Move(double transform, double delta, bool isInertial, double minimum, double maximum)
        {
            // NoInertial
            if (isInertial is false) return System.Math.Clamp(transform + delta, minimum, maximum);

            // Direction
            switch (this.Direction)
            {
                case MarbleDirection.None: transform += delta; break;
                case MarbleDirection.Positive: transform += System.Math.Abs(delta); break;
                case MarbleDirection.Negative: transform -= System.Math.Abs(delta); break;
                default: break;
            }

            // Placement
            MarblePlacement placement = this.GetPlacement(transform, minimum, maximum);
            if (this.Placement == placement) return transform;

            this.Direction = this.GetDirection(this.Placement, placement);
            this.Placement = placement;
            return transform;
        }

        private MarblePlacement GetPlacement(double value, double minimum, double maximum)
        {
            if (value < minimum) return MarblePlacement.Minimum;
            else if (value > maximum) return MarblePlacement.Maximum;
            else return MarblePlacement.Default;
        }

        private MarbleDirection GetDirection(MarblePlacement startingPlacement, MarblePlacement placement)
        {
            switch (startingPlacement)
            {
                case MarblePlacement.Minimum:
                    switch (placement)
                    {
                        case MarblePlacement.Minimum: return MarbleDirection.Positive;
                        case MarblePlacement.Default: return MarbleDirection.Positive;
                        case MarblePlacement.Maximum: return MarbleDirection.Negative;
                        default: return MarbleDirection.None;
                    }
                case MarblePlacement.Default:
                    switch (placement)
                    {
                        case MarblePlacement.Minimum: return MarbleDirection.Positive;
                        case MarblePlacement.Default: return MarbleDirection.None;
                        case MarblePlacement.Maximum: return MarbleDirection.Negative;
                        default: return MarbleDirection.None;
                    }
                case MarblePlacement.Maximum:
                    switch (placement)
                    {
                        case MarblePlacement.Minimum: return MarbleDirection.Positive;
                        case MarblePlacement.Default: return MarbleDirection.Negative;
                        case MarblePlacement.Maximum: return MarbleDirection.Negative;
                        default: return MarbleDirection.None;
                    }
                default: return MarbleDirection.None;
            }
        }

    }
}