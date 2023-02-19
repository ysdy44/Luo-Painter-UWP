using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Shape of <see cref="ScrollerOrigami"/>.
    /// </summary>
    public enum ScrollerShape
    {
        /// <summary> "Page Down" state </summary>
        PageDown,
        /// <summary> "Page Up" state </summary>
        PageUp,

        /// <summary> Origami is like a Rectangle. </summary>
        Rectangle,
        /// <summary> Origami is like a Triangle. </summary>
        Triangle,

        /// <summary> Origami resembles an Inside Trapezium. </summary>
        InsideTrapezium,
        /// <summary> Origami resembles an Outside Trapezium. </summary>
        OutsideTrapezium,
    }

    /// <summary>
    /// Origami of <see cref="Scroller"/>.
    /// </summary>
    public struct ScrollerOrigami
    {
        public ScrollerShape Shape;
        public Point LeftTop;
        public Point RightTop;
        public Point RightBottom;
        public Point LeftBottom;

        public ScrollerOrigami(Point value, double width, double height)
        {
            if (value.X == 1)
            {
                if (value.Y >= 1)
                {
                    this.Shape = ScrollerShape.PageDown;
                    this.LeftTop =
                    this.RightTop =
                    this.RightBottom =
                    this.LeftBottom = new Point(width, height);
                }
                else if (value.Y <= -1)
                {
                    this.Shape = ScrollerShape.PageUp;
                    this.LeftTop = new Point(0, -height);
                    this.RightTop = new Point(width, -height);
                    this.RightBottom = new Point(width, 0);
                    this.LeftBottom = new Point(0, 0);
                }
                else
                {
                    double y1 = value.Y * height;
                    double y2 = (height + y1) / 2;

                    this.Shape = ScrollerShape.Rectangle;
                    this.LeftTop = new Point(0, y1);
                    this.RightTop = new Point(width, y1);
                    this.RightBottom = new Point(width, y2);
                    this.LeftBottom = new Point(0, y2);
                }
                return;
            }

            /////////////////////////////////////////////////////////

            Point point = new Point(width * value.X, Math.Min(height - 1, height * value.Y));
            if (value.X < 1)
            {
                // RightTop & Point
                double maxLength = height;
                Point vector = ScrollerOrigami.Subtract(new Point(width, 0), point);
                double length = ScrollerOrigami.Length(vector);
                if (length > maxLength)
                {
                    point = ScrollerOrigami.Subtract(new Point(width, 0), ScrollerOrigami.Multipe(vector, 1 / length * maxLength));
                }
            }
            else // if (value.X > 1)
            {
                // LeftTop & Point
                double maxLength = ScrollerOrigami.Length(new Point(width, height));
                Point vector = point;
                double length = ScrollerOrigami.Length(vector);
                if (length > maxLength)
                {
                    point = ScrollerOrigami.Multipe(vector, 1 / length * maxLength);
                }
            }

            /////////////////////////////////////////////////////////

            Point center = ScrollerOrigami.Divide(ScrollerOrigami.Add(point, new Point(width, height)), 2);
            double diagonal = ScrollerOrigami.Angle(point, new Point(width, height));

            double angleLeft = diagonal + Math.PI / 2;
            Point vectorLeft = new Point(Math.Cos(angleLeft), Math.Sin(angleLeft));
            Point intersectionLeft = ScrollerOrigami.Intersection(center, ScrollerOrigami.Add(center, ScrollerOrigami.Multipe(vectorLeft, 9999)), new Point(0, -9999), new Point(0, 9999));

            double angleRight = diagonal - Math.PI / 2;
            Point vectorRight = new Point(Math.Cos(angleRight), Math.Sin(angleRight));
            Point intersectionRight = ScrollerOrigami.Intersection(center, ScrollerOrigami.Add(center, ScrollerOrigami.Multipe(vectorRight, 9999)), new Point(width, -9999), new Point(width, 9999));

            Point intersectionBottom = ScrollerOrigami.Intersection(center, ScrollerOrigami.Add(center, ScrollerOrigami.Multipe(vectorLeft, 9999)), new Point(-9999, height), new Point(9999, height));

            /////////////////////////////////////////////////////////

            if (value.X < 1)
            {
                if (intersectionLeft.Y > height)
                {
                    this.Shape = ScrollerShape.Triangle;
                    this.LeftTop = intersectionBottom;
                    this.RightTop = point;
                    this.RightBottom = intersectionRight;
                    this.LeftBottom = intersectionBottom;
                }
                else
                {
                    Point vector = ScrollerOrigami.Subtract(intersectionBottom, point);
                    double length = ScrollerOrigami.Length(vector);
                    Point leftTop = ScrollerOrigami.Add(point, ScrollerOrigami.Multipe(vector, 1 / length * width));
                    this.Shape = ScrollerShape.InsideTrapezium;
                    this.LeftTop = leftTop;
                    this.RightTop = point;
                    this.RightBottom = intersectionRight;
                    this.LeftBottom = intersectionLeft;
                }
            }
            else // if (value.X > 1)
            {
                Point vector = ScrollerOrigami.Subtract(point, intersectionBottom);
                double length = ScrollerOrigami.Length(vector);
                Point leftTop = ScrollerOrigami.Add(point, ScrollerOrigami.Multipe(vector, 1 / length * width));
                this.Shape = ScrollerShape.OutsideTrapezium;
                this.LeftTop = leftTop;
                this.RightTop = point;
                this.RightBottom = intersectionRight;
                this.LeftBottom = intersectionLeft;
            }
        }


        public double MinX() => Math.Min(Math.Min(this.LeftTop.X, this.RightTop.X), Math.Min(this.LeftBottom.X, this.RightBottom.X));
        public double MinY() => Math.Min(Math.Min(this.LeftTop.Y, this.RightTop.Y), Math.Min(this.LeftBottom.Y, this.RightBottom.Y));
        public Point EndPoint() => ScrollerOrigami.FootOfPerpendicular(this.StartPoint(), this.LeftBottom, this.RightBottom);
        public Point StartPoint()
        {
            switch (this.Shape)
            {
                case ScrollerShape.OutsideTrapezium: return this.LeftTop;
                default: return this.RightTop;
            }
        }


        //@Static
        public static Point Add(Point origin, Point other) => new Point(origin.X + other.X, origin.Y + other.Y);
        public static Point Subtract(Point origin, Point other) => new Point(origin.X - other.X, origin.Y - other.Y);
        public static Point Multipe(Point origin, double length) => new Point(origin.X * length, origin.Y * length);
        public static Point Divide(Point origin, double length) => new Point(origin.X / length, origin.Y / length);


        public static double Angle(Point origin, Point point) => Math.Atan2(point.Y - origin.Y, point.X - origin.X);
        public static double Length(Point origin) => Math.Sqrt(origin.X * origin.X + origin.Y * origin.Y);


        public static Point Intersection(Point lineAStart, Point lineAEnd, Point lineBStart, Point lineBEnd)
        {
            double x1 = lineAStart.X, y1 = lineAStart.Y;
            double x2 = lineAEnd.X, y2 = lineAEnd.Y;

            double x3 = lineBStart.X, y3 = lineBStart.Y;
            double x4 = lineBEnd.X, y4 = lineBEnd.Y;


            //equations of the form x=c (two vertical lines)
            if (x1 == x2 && x3 == x4 && x1 == x3) return new Point();
            //equations of the form y=c (two horizontal lines)
            if (y1 == y2 && y3 == y4 && y1 == y3) return new Point();
            //equations of the form x=c (two vertical lines)
            if (x1 == x2 && x3 == x4) return new Point();
            //equations of the form y=c (two horizontal lines)
            if (y1 == y2 && y3 == y4) return new Point();


            double x, y;

            if (x1 == x2)
            {
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                x = x1;
                y = c2 + m2 * x1;
            }
            else if (x3 == x4)
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                x = x3;
                y = c1 + m1 * x3;
            }
            else
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                //if (!(-m1 * x + y == c1
                //    && -m2 * x + y == c2))
                //{
                //    return new Point();
                //}
            }

            return new Point(x, y);

            if (IsInsideLine(lineAStart, lineAEnd, x, y) && IsInsideLine(lineBStart, lineBEnd, x, y))
            {
                return new Point(x, y);
            }
            else
            {
                //return default null (no intersection)
                return new Point();
            }
        }
        private static bool IsInsideLine(Point start, Point end, double x, double y)
        {
            return ((x >= start.X && x <= end.X)
                || (x >= end.X && x <= start.X))
                && ((y >= start.Y && y <= end.Y)
                    || (y >= end.Y && y <= start.Y));
        }


        public static Point FootOfPerpendicular(Point origin, Point start, Point end)
        {
            double dx = start.X - end.X;
            double dy = start.Y - end.Y;

            // if (Math.Abs(dx) < 0.00000001 && Math.Abs(dy) < 0.00000001)
            // return begin;

            double u = (origin.X - start.X) * (start.X - end.X) +
                (origin.Y - start.Y) * (start.Y - end.Y);

            u = u / ((dx * dx) + (dy * dy));

            double x = start.X + u * dx;
            double y = start.Y + u * dy;
            return new Point(x, y);
        }
    }

    /// <summary>
    /// Represents a control that simulates origami animations.
    /// </summary>
    public sealed partial class Scroller : Canvas
    {

        //@Delegate
        /// <summary>
        /// Occurs when this page up timeline has completely finished playing.
        /// </summary>
        public event EventHandler<object> DragPageUpCompleted;
        /// <summary>
        /// Occurs when this page down timeline has completely finished playing.
        /// </summary>
        public event EventHandler<object> DragPageDownCompleted;

        /// <summary>
        /// Gets the state of <see cref="Scroller"/>.
        /// </summary>
        public bool IsPlaying => this.IsPageUpCompleted == false || this.IsPageDownCompleted == false;

        bool IsDragStarted;
        bool IsPageUpCompleted = true;
        bool IsPageDownCompleted = true;

        double StartingX = 100;
        double StartingY = 200;

        #region DependencyProperty


        /// <summary> Gets or sets the value of<see cref = "Scroller" />. </summary>
        public Point Value
        {
            get => (Point)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref = "Scroller.Value" /> dependency property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Point), typeof(Scroller), new PropertyMetadata(new Point(1, 1), (sender, e) =>
        {
            Scroller control = (Scroller)sender;

            if (e.NewValue is Point value)
            {
                control.SetValue(value);
            }
        }));


        #endregion

        readonly Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap RenderTargetBitmap = new Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap();

        readonly LineSegment RightTopSegment = new LineSegment();
        readonly LineSegment RightLineSegment = new LineSegment();
        readonly LineSegment BottomLineSegment = new LineSegment();
        readonly LineSegment LeftLineSegment = new LineSegment();
        readonly PointCollection Points = new PointCollection
        {
            new Point(),
            new Point(),
            new Point(),
            new Point(),
        };
        readonly Line Line = new Line
        {
            StrokeThickness = 1,
            Stroke = new SolidColorBrush(Colors.Gray)
        };
        readonly LinearGradientBrush LinearGradientBrush = new LinearGradientBrush
        {
            MappingMode = BrushMappingMode.Absolute,
            GradientStops =
            {
                new GradientStop
                {
                    Color = Colors.LightGray,
                    Offset = 0,
                },
                new GradientStop
                {
                    Color = Colors.LightGray,
                    Offset = 0.7,
                },
                new GradientStop
                {
                    Color = Colors.Black,
                    Offset = 1,
                }
            }
        };
        readonly Storyboard PageDownStoryboard = new Storyboard
        {
            RepeatBehavior = new RepeatBehavior(1),
            Children =
            {
                new PointAnimation
                {
                    From = null,
                    To = new Point(1, 1),
                    Duration = TimeSpan.FromMilliseconds(400),
                    EnableDependentAnimation = true,
                }
            }
        };
        readonly Storyboard PageUpStoryboard = new Storyboard
        {
            RepeatBehavior = new RepeatBehavior(1),
            Children =
            {
                new PointAnimation
                {
                    From = null,
                    To = new Point(1, -1),
                    Duration = TimeSpan.FromMilliseconds(400),
                    EnableDependentAnimation = true,
                }
            }
        };

        //@Construct
        /// <summary>
        /// Initializes a Scroller. 
        /// </summary> 
        public Scroller()
        {
            this.InitializeComponent();
            {
                Timeline animation = this.PageDownStoryboard.Children.First();
                Storyboard.SetTarget(animation, this);
                Storyboard.SetTargetProperty(animation, nameof(Value));
            }
            {
                Timeline animation = this.PageUpStoryboard.Children.First();
                Storyboard.SetTarget(animation, this);
                Storyboard.SetTargetProperty(animation, nameof(Value));
            }
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.SetValue(this.Value, e.NewSize.Width, e.NewSize.Height);
            };

            this.PageUpStoryboard.Completed += (s, e) =>
            {
                this.IsPageUpCompleted = true;

                if (this.IsDragStarted == false) return;
                this.IsDragStarted = false;
                this.DragPageUpCompleted?.Invoke(this, e); // Delegate
            };
            this.PageDownStoryboard.Completed += (s, e) =>
            {
                this.IsPageDownCompleted = true;

                if (this.IsDragStarted == false) return;
                this.IsDragStarted = false;
                this.DragPageDownCompleted?.Invoke(this, e); // Delegate
            };
        }


        /// <summary> <see cref="Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap.RenderAsync"/> </summary>
        public IAsyncAction RenderAsync(UIElement element) => this.RenderTargetBitmap.RenderAsync(element);


        /// <summary> <see cref="Thumb.DragStarted"/> </summary>
        public void DragStarted(double offsetX = 50, double offsetY = 50)
        {
            this.Reset();

            this.StartingX = base.ActualWidth - offsetX;
            this.StartingY = base.ActualHeight - offsetY;
            this.Value = new Point(this.StartingX / base.ActualWidth, this.StartingY / base.ActualHeight);

            this.IsDragStarted = true;
        }
        /// <summary> <see cref="Thumb.DragDelta"/> </summary>
        public void DragDelta(double horizontalChange, double verticalChange)
        {
            if (this.IsDragStarted == false) return;

            this.StartingX += horizontalChange;
            this.StartingY += verticalChange;
            this.Value = new Point(this.StartingX / base.ActualWidth, this.StartingY / base.ActualHeight);
        }
        /// <summary> <see cref="Thumb.DragCompleted"/> </summary>
        public void DragCompleted()
        {
            if (this.IsDragStarted == false) return;

            if (this.Value.Y > 0.5)
                this.PageDown();
            else
                this.PageUp();
        }


        /// <summary> Page Up </summary>
        public void PageUp()
        {
            if (this.PageUpStoryboard.Children.First() is PointAnimation asnimation)
            {
                asnimation.From = this.Value;
                this.PageUpStoryboard.Begin(); // Storyboard
                this.IsPageUpCompleted = false;
            }
        }
        /// <summary> Page Down </summary>
        public void PageDown()
        {
            if (this.PageDownStoryboard.Children.First() is PointAnimation asnimation)
            {
                asnimation.From = this.Value;
                this.PageDownStoryboard.Begin(); // Storyboard
                this.IsPageDownCompleted = false;
            }
        }


        private void InitializeComponent()
        {
            base.Children.Add(new Path
            {
                Fill = new ImageBrush
                {
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    Stretch = Stretch.None,
                    ImageSource = this.RenderTargetBitmap
                },
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(0, 0),
                            Segments = new PathSegmentCollection
                            {
                                this.RightTopSegment,
                                this.RightLineSegment,
                                this.BottomLineSegment,
                                this.LeftLineSegment,
                                new LineSegment
                                {
                                    Point = new Point(0, 0)
                                }
                            }
                        }
                    }
                }
            });

            base.Children.Add(new Polyline
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = this.LinearGradientBrush,
                Points = this.Points
            });

            base.Children.Add(this.Line);
        }


        private void SetValue(Point value) => this.SetValue(value, base.ActualWidth, base.ActualHeight);
        private void SetValue(Point value, double width, double height)
        {
            ScrollerOrigami origami = new ScrollerOrigami(value, width, height);

            // Path
            switch (origami.Shape)
            {
                case ScrollerShape.PageDown:
                    this.Reset(width, height);
                    return;
                case ScrollerShape.Triangle:
                    this.RightLineSegment.Point = origami.RightBottom;
                    this.BottomLineSegment.Point = origami.LeftBottom;
                    this.LeftLineSegment.Point = new Point(0, height);
                    break;
                case ScrollerShape.PageUp:
                case ScrollerShape.Rectangle:
                case ScrollerShape.InsideTrapezium:
                case ScrollerShape.OutsideTrapezium:
                    this.RightLineSegment.Point = origami.RightBottom;
                    this.BottomLineSegment.Point =
                    this.LeftLineSegment.Point = origami.LeftBottom;
                    break;
                default:
                    break;
            }

            // Polyline
            this.Points[0] = origami.LeftBottom;
            this.Points[1] = origami.LeftTop;
            this.Points[2] = origami.RightTop;
            this.Points[3] = origami.RightBottom;

            // Line
            this.Line.X1 = origami.LeftBottom.X;
            this.Line.Y1 = origami.LeftBottom.Y;
            this.Line.X2 = origami.RightBottom.X;
            this.Line.Y2 = origami.RightBottom.Y;

            // Brush
            Point start = origami.StartPoint();
            Point end = origami.EndPoint();
            double minX = origami.MinX();
            double minY = origami.MinY();
            this.LinearGradientBrush.StartPoint = new Point(start.X - minX, start.Y - minY);
            this.LinearGradientBrush.EndPoint = new Point(end.X - minX, end.Y - minY);
        }


        private void Reset() => this.Reset(base.ActualWidth, base.ActualHeight);
        private void Reset(double width, double height)
        {
            // Path
            this.RightTopSegment.Point = new Point(width, 0);
            this.RightLineSegment.Point = new Point(width, height);
            this.BottomLineSegment.Point = new Point(width, height);
            this.LeftLineSegment.Point = new Point(0, height);

            // Polyline
            this.Points[0] =
            this.Points[1] =
            this.Points[2] =
            this.Points[3] = new Point(width, height);

            // Brush
            this.LinearGradientBrush.StartPoint =
            this.LinearGradientBrush.EndPoint = new Point(0, 0);
        }

    }
}