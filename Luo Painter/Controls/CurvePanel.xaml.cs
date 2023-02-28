using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Controls
{
    public sealed partial class CurvePanel : UserControl
    {
        //@Delegate
        public event EventHandler<object> Invalidate;

        //@Converter
        private Visibility AlphaVisibilityConverter(EffectChannelSelect value) => value is EffectChannelSelect.Alpha ? Visibility.Visible : Visibility.Collapsed;
        private Visibility RedVisibilityConverter(EffectChannelSelect value) => value is EffectChannelSelect.Red ? Visibility.Visible : Visibility.Collapsed;
        private Visibility GreenVisibilityConverter(EffectChannelSelect value) => value is EffectChannelSelect.Green ? Visibility.Visible : Visibility.Collapsed;
        private Visibility BlueVisibilityConverter(EffectChannelSelect value) => value is EffectChannelSelect.Blue ? Visibility.Visible : Visibility.Collapsed;
        private bool? AlphaBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Alpha;
        private bool? RedBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Red;
        private bool? GreenBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Green;
        private bool? BlueBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Blue;
        private bool ReverseAlphaBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Alpha is false;
        private bool ReverseRedBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Red is false;
        private bool ReverseGreenBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Green is false;
        private bool ReverseBlueBooleanConverter(EffectChannelSelect value) => value is EffectChannelSelect.Blue is false;

        readonly IList<float> Curves = new List<float>
        {
            0,
            0.5f,
            1
        };

        //@Content
        public float[] RedTable => this.RedSelector.Data;
        public float[] GreenTable => this.GreenSelector.Data;
        public float[] BlueTable => this.BlueSelector.Data;

        #region DependencyProperty


        /// <summary> Gets or set the mode for <see cref="CurvePanel"/>. </summary>
        public EffectChannelSelect Mode
        {
            get => (EffectChannelSelect)base.GetValue(ModeProperty);
            set => base.SetValue(ModeProperty, value);
        }
        /// <summary> Identifies the <see cref = "CurvePanel.Mode" /> dependency property. </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(EffectChannelSelect), typeof(CurvePanel), new PropertyMetadata(EffectChannelSelect.Red));


        /// <summary> Gets or set the orientation for <see cref="CurvePanel"/>. </summary>
        public Orientation Orientation
        {
            get => (Orientation)base.GetValue(OrientationProperty);
            set => base.SetValue(OrientationProperty, value);
        }
        /// <summary> Identifies the <see cref = "CurvePanel.Orientation" /> dependency property. </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(CurvePanel), new PropertyMetadata(Orientation.Horizontal, (sender, e) =>
        {
            CurvePanel control = (CurvePanel)sender;

            if (e.NewValue is Orientation value)
            {
                switch (value)
                {
                    case Orientation.Vertical:
                        control.Vertical(control.ActualWidth);
                        break;
                    case Orientation.Horizontal:
                        control.Horizontal(control.ActualWidth);
                        break;
                    default:
                        break;
                }

            }
        }));


        #endregion

        //@Construct
        public CurvePanel()
        {
            this.InitializeComponent();
            this.ConstructRandom();
            base.SizeChanged += (s, e) =>
            {
                switch (this.Orientation)
                {
                    case Orientation.Vertical:
                        this.Vertical(e.NewSize.Width);
                        break;
                    case Orientation.Horizontal:
                        this.Horizontal(e.NewSize.Width);
                        break;
                    default:
                        break;
                }
            };

            this.RedButton.Click += (s, e) => this.Mode = EffectChannelSelect.Red;
            this.GreenButton.Click += (s, e) => this.Mode = EffectChannelSelect.Green;
            this.BlueButton.Click += (s, e) => this.Mode = EffectChannelSelect.Blue;

            this.RedSelector.Reset(this.Curves);
            this.RedSelector.ItemRemoved += (s, e) =>
            {
                this.ChangePolyline(this.RedPolyline, this.RedSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.RedSelector.Invalidate += (s, e) =>
            {
                this.ChangePolyline(this.RedPolyline, this.RedSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };

            this.GreenSelector.Reset(this.Curves);
            this.GreenSelector.ItemRemoved += (s, e) =>
            {
                this.ChangePolyline(this.GreenPolyline, this.GreenSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.GreenSelector.Invalidate += (s, e) =>
            {
                this.ChangePolyline(this.GreenPolyline, this.GreenSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };

            this.BlueSelector.Reset(this.Curves);
            this.BlueSelector.ItemRemoved += (s, e) =>
            {
                this.ChangePolyline(this.BluePolyline, this.BlueSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            this.BlueSelector.Invalidate += (s, e) =>
            {
                this.ChangePolyline(this.BluePolyline, this.BlueSelector.Data);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
        }

        private void Vertical(double width)
        {
            width = Math.Max(200, width);

            double height = 178;
            base.Height = 178 + 50;


            double w = (width - 10 - 10 - 10) / 4;
            this.RandomButton.Width =
            this.RedButton.Width =
            this.GreenButton.Width =
            this.BlueButton.Width = w;

            Canvas.SetLeft(this.RandomButton, 0);
            Canvas.SetTop(this.RandomButton, height + 10);

            Canvas.SetLeft(this.RedButton, w + 10);
            Canvas.SetTop(this.RedButton, height + 10);

            Canvas.SetLeft(this.GreenButton, w + 10 + w + 10);
            Canvas.SetTop(this.GreenButton, height + 10);

            Canvas.SetLeft(this.BlueButton, w + 10 + w + 10 + w + 10);
            Canvas.SetTop(this.BlueButton, height + 10);


            Canvas.SetLeft(this.Canvas, 0);

            this.Canvas.Width =
            this.Rectangle.Width =
            this.Grid.Width = width;

            this.Canvas.Height =
            this.Rectangle.Height =
            this.Grid.Height = height;

            this.ChangeLines(width, height);

            this.ChangePolyline(this.RedPolyline, this.RedSelector.Data, width, height);
            this.ChangePolyline(this.GreenPolyline, this.GreenSelector.Data, width, height);
            this.ChangePolyline(this.BluePolyline, this.BlueSelector.Data, width, height);
        }
        private void Horizontal(double width)
        {
            width -= 100;
            width = Math.Max(200, width);
            double height = 178;
            base.Height = 178;


            this.RandomButton.Width =
            this.RedButton.Width =
            this.GreenButton.Width =
            this.BlueButton.Width = 100 - 10;

            Canvas.SetLeft(this.RandomButton, 0);
            Canvas.SetLeft(this.RedButton, 0);
            Canvas.SetLeft(this.GreenButton, 0);
            Canvas.SetLeft(this.BlueButton, 0);

            Canvas.SetTop(this.RandomButton, 0);
            Canvas.SetTop(this.RedButton, 40 + 6);
            Canvas.SetTop(this.GreenButton, 40 + 6 + 40 + 6);
            Canvas.SetTop(this.BlueButton, 40 + 6 + 40 + 6 + 40 + 6);


            Canvas.SetLeft(this.Canvas, 100);

            this.Canvas.Width =
            this.Rectangle.Width =
            this.Grid.Width = width;

            this.Canvas.Height =
            this.Rectangle.Height =
            this.Grid.Height = height;

            this.ChangeLines(width, height);

            this.ChangePolyline(this.RedPolyline, this.RedSelector.Data, width, height);
            this.ChangePolyline(this.GreenPolyline, this.GreenSelector.Data, width, height);
            this.ChangePolyline(this.BluePolyline, this.BlueSelector.Data, width, height);
        }

        public void ChangeLines(double width, double height)
        {
            this.H1Line.X1 = 0;
            this.H1Line.X2 = width;
            this.H1Line.Y1 =
            this.H1Line.Y2 = height / 4;

            this.H2Line.X1 = 0;
            this.H2Line.X2 = width;
            this.H2Line.Y1 =
            this.H2Line.Y2 = height / 2;

            this.H3Line.X1 = 0;
            this.H3Line.X2 = width;
            this.H3Line.Y1 =
            this.H3Line.Y2 = height / 4 * 3;


            this.V1Line.Y1 = 0;
            this.V1Line.Y2 = height;
            this.V1Line.X1 =
            this.V1Line.X2 = width / 4;

            this.V2Line.Y1 = 0;
            this.V2Line.Y2 = height;
            this.V2Line.X1 =
            this.V2Line.X2 = width / 2;

            this.V3Line.Y1 = 0;
            this.V3Line.Y2 = height;
            this.V3Line.X1 =
            this.V3Line.X2 = width / 4 * 3;
        }

        public void ChangePolyline(Polyline polyline, float[] array) => this.ChangePolyline(polyline, array, this.Grid.ActualWidth, this.Grid.ActualHeight);
        public void ChangePolyline(Polyline polyline, float[] array, double width, double height)
        {
            int count = array.Length;

            if (count == polyline.Points.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    polyline.Points[i] = new Point
                    {
                        X = width / (count - 1) * i,
                        Y = array[i] * height
                    };
                }
            }
            else
            {
                polyline.Points.Clear();
                for (int i = 0; i < count; i++)
                {
                    polyline.Points.Add(new Point
                    {
                        X = width / (count - 1) * i,
                        Y = array[i] * height
                    });
                }
            }
        }

    }
}