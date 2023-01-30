using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    /// <summary>
    /// double t = 0 ~ 1;
    /// 
    /// double x = (1 + 3 * Point1.X - 3 * Point2.X) * t * t * t + 3 * (Point2.X - 2 * Point1.X) * t * t + 3 * Point1.X * t;
    /// 
    /// double y = (1 + 3 * Point1.Y - 3 * Point2.Y) * t * t * t + 3 * (Point2.Y - 2 * Point1.Y) * t * t + 3 * Point1.Y * t;
    /// </summary>
    public sealed partial class SplinePicker : UserControl
    {
        //@Delegate
        public event System.EventHandler<object> Invalidate;

        //@Static
        readonly static float Round = (float)((System.Math.Sqrt(2) - 1) * 4 / 3); // 0.552284749831 // new Vector2(Round, 0) // new Vector2(1, 1 - Round)

        //@Converter
        private double X(Vector2 value) => value.X * 240;
        private double Y(Vector2 value) => 240 - value.Y * 240;

        private double Left(Vector2 value) => value.X * 240 - 16;
        private double Top(Vector2 value) => 240 - value.Y * 240 - 16;

        private Point Point(Vector2 value) => new Point(value.X * 240, 240 - value.Y * 240);

        Vector2 Statring;

        #region DependencyProperty

        /// <summary> Gets or sets the first point of<see cref = "SplinePicker" />. </summary>
        public Vector2 Point1
        {
            get => (Vector2)base.GetValue(Point1Property);
            set => base.SetValue(Point1Property, value);
        }
        /// <summary> Identifies the <see cref = "SplinePicker.Point1" /> dependency property. </summary>
        public static readonly DependencyProperty Point1Property = DependencyProperty.Register(nameof(Point1), typeof(Vector2), typeof(SplinePicker), new PropertyMetadata(Vector2.Zero));

        /// <summary> Gets or sets the second point of<see cref = "SplinePicker" />. </summary>
        public Vector2 Point2
        {
            get => (Vector2)base.GetValue(Point2Property);
            set => base.SetValue(Point2Property, value);
        }
        /// <summary> Identifies the <see cref = "SplinePicker.Point2" /> dependency property. </summary>
        public static readonly DependencyProperty Point2Property = DependencyProperty.Register(nameof(Point2), typeof(Vector2), typeof(SplinePicker), new PropertyMetadata(Vector2.One));

        #endregion

        //@Construct
        public SplinePicker()
        {
            this.InitializeComponent();

            this.Thumb1.DragStarted += (s, e) =>
            {
                this.Statring = this.Point1 * 240;
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.Thumb1.DragDelta += (s, e) =>
            {
                this.Statring.X += (float)e.HorizontalChange;
                this.Statring.Y -= (float)e.VerticalChange;
                this.Point1 = Vector2.Clamp(this.Statring / 240, Vector2.Zero, Vector2.One);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.Thumb1.DragCompleted += (s, e) =>
            {
                this.Invalidate?.Invoke(this, null); // Delegate
            };

            this.Thumb2.DragStarted += (s, e) =>
            {
                this.Statring = this.Point2 * 240;
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.Thumb2.DragDelta += (s, e) =>
            {
                this.Statring.X += (float)e.HorizontalChange;
                this.Statring.Y -= (float)e.VerticalChange;
                this.Point2 = Vector2.Clamp(this.Statring / 240, Vector2.Zero, Vector2.One);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.Thumb2.DragCompleted += (s, e) =>
            {
                this.Invalidate?.Invoke(this, null); // Delegate
            };
        }
        public void Linear(float value)
        {
            this.Point1 = new Vector2(System.Math.Clamp(1 - value, 0, 1), 0);
            this.Point2 = new Vector2(System.Math.Clamp(2 - value, 0, 1), 1);
            this.Invalidate?.Invoke(this, null); // Delegate
        }
    }
}