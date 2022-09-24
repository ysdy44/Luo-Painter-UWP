using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal enum PenCurveTool
    {
        Curve,
        Line,
        RectChoose,
        Move,
        Hitter,
        Cutter,
    }

    public sealed partial class PenCurvePage : Page
    {

        //@Key
        private PenCurveTool Mode
        {
            get
            {
                switch (this.ListBox.SelectedIndex)
                {
                    case 0: return PenCurveTool.Curve;
                    case 1: return PenCurveTool.Line;
                    case 2: return PenCurveTool.RectChoose;
                    case 3: return PenCurveTool.Move;
                    case 4: return PenCurveTool.Hitter;
                    case 5: return PenCurveTool.Cutter;
                    default: return default;
                }
            }
        }

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));
        private CanvasGeometry ToPoint(CanvasGeometry geometry) => geometry.Transform(this.CanvasControl.Dpi.ConvertPixelsToDips((this.Transformer.GetMatrix())));
        private float ToPressure(float value)
        {
            if (value > 0) return 1 + value;
            if (value < 0) return 1 / (1 - value);
            return 1;
        }

        readonly ObservableCollection<CurveLayer> ObservableCollection = new ObservableCollection<CurveLayer>();
        CanvasBitmap CanvasBitmap;
        Transformer Border;

        TransformerRect TransformerRect;
        Vector2 StartingPosition;
        Vector2 Position;

        Vector2 StartingPreviousPosition;
        Vector2 PreviousPosition;

        PenHitter Hitter;
        PenCutter Cutter;

        public PenCurvePage()
        {
            this.InitializeComponent();
            this.ConstructPenCurve();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenCurve()
        {
            this.ListView.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.AddAnchorsButton.Click += (s, e) =>
            {
                int count = this.ObservableCollection.Count;

                this.ObservableCollection.Add(new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height));
                this.ListView.SelectedIndex = count;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RemoveAnchorsButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    this.ObservableCollection.RemoveAt(index);
                    this.ListView.SelectedIndex = index - 1;

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.ListBox.ItemsSource = System.Enum.GetValues(typeof(PenCurveTool));
            this.ListBox.SelectedIndex = 0;

            this.ResetPressureButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    foreach (Anchor item in layer.Anchors)
                    {
                        item.Pressure = 1;
                    }

                    layer.Anchors.Segment(this.CanvasControl);
                    layer.Anchors.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.PressureSlider.ValueChanged += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    float value = this.ToPressure((float)e.NewValue);
                    foreach (Anchor item in layer.Anchors)
                    {
                        if (item.IsChecked) item.Pressure = value;
                    }

                    layer.Anchors.Segment(this.CanvasControl);
                    layer.Anchors.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (e.NewColor == e.OldColor) return;

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    layer.Anchors.Color = e.NewColor;
                    layer.Anchors.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.StrokeWidthSlider.ValueChanged += (s, e) =>
            {
                if (e.NewValue == e.OldValue) return;

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    layer.Anchors.StrokeWidth = (float)e.NewValue;
                    layer.Anchors.Segment(this.CanvasControl);
                    layer.Anchors.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    Anchor[] uncheck = layer.Anchors.Where(c => c.IsChecked is false).ToArray();

                    layer.Anchors.Clear();
                    foreach (Anchor item in uncheck)
                    {
                        layer.Anchors.Add(item);
                    }

                    layer.Anchors.Segment(this.CanvasControl);
                    layer.Anchors.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Transformer.Fit();
                this.ObservableCollection.Add(new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height));
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CanvasBitmap,
                    TransformMatrix = this.Transformer.GetMatrix(),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                });

                foreach (CurveLayer items in this.ObservableCollection)
                {
                    if (items is null) continue;
                    args.DrawingSession.DrawImage(new Transform2DEffect
                    {
                        Source = items[BitmapType.Source],
                        TransformMatrix = this.Transformer.GetMatrix(),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    });
                }


                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    foreach (Anchor item in layer.Anchors)
                    {
                        if (item is null) continue;
                        if (item.Geometry is null) continue;
                        args.DrawingSession.DrawGeometry(this.ToPoint(item.Geometry), Colors.DodgerBlue, 1);
                    }
                    args.DrawingSession.DrawAnchorCollection(layer.Anchors, matrix);

                    //for (int i = 0; i < layer.Anchors.Count; i++)
                    //{
                    //    args.DrawingSession.DrawText(i.ToString(), Vector2.Transform(layer[i].Point, matrix) - new Vector2(40, 40), Colors.Red);
                    //}
                }

                switch (this.Mode)
                {
                    case PenCurveTool.RectChoose:
                        if (this.TransformerRect.Width == default) break;
                        if (this.TransformerRect.Height == default) break;
                        {
                            CanvasGeometry canvasGeometry = this.TransformerRect.ToRectangle(this.CanvasControl);
                            CanvasGeometry canvasGeometryTransform = canvasGeometry.Transform(matrix);
                            args.DrawingSession.DrawGeometryDodgerBlue(canvasGeometryTransform);
                        }
                        break;
                    case PenCurveTool.Hitter:
                        if (this.Hitter.IsContains)
                        {
                            Vector2 p = this.ToPoint(this.Hitter.Target);
                            args.DrawingSession.DrawLine(p, this.ToPoint(this.Position), this.Hitter.IsHit ? Colors.Orange : Colors.DodgerBlue, 2);

                            args.DrawingSession.FillCircle(p, 4, Colors.White);
                            args.DrawingSession.FillCircle(p, 3, Colors.DodgerBlue);
                        }
                        break;
                    case PenCurveTool.Cutter:
                        Vector2 sp = this.ToPoint(this.StartingPosition);
                        Vector2 pp = this.ToPoint(this.Position);
                        args.DrawingSession.DrawLine(sp, pp, Colors.Orange, 2);

                        if (this.Cutter.IsHit)
                        {
                            Vector2 p = this.ToPoint(this.Cutter.Target);
                            args.DrawingSession.FillCircle(p, 4, Colors.White);
                            args.DrawingSession.FillCircle(p, 3, Colors.Orange);
                        }
                        break;
                    default:
                        break;
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            if (layer.Anchors.Count is 0)
                            {
                                this.StartingPreviousPosition = this.PreviousPosition =
                                this.StartingPosition = this.Position =
                                this.ToPosition(point);

                                layer.Anchors.Add(new Anchor
                                {
                                    Point = this.Position,
                                    LeftControlPoint = this.Position,
                                    RightControlPoint = this.Position,
                                    IsSmooth = this.Mode is PenCurveTool.Curve,
                                });

                                layer.Anchors.Color = this.ColorPicker.Color;
                                layer.Anchors.StrokeWidth = (float)this.StrokeWidthSlider.Value;

                                layer.Anchors.ClosePoint = this.Position;
                                layer.Anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                                layer.Anchors.Segment(this.CanvasControl, false);
                            }
                            else
                            {
                                this.StartingPreviousPosition = this.PreviousPosition;
                                this.StartingPosition = this.Position = this.ToPosition(point);

                                layer.Anchors.Add(new Anchor
                                {
                                    Point = this.StartingPreviousPosition,
                                    LeftControlPoint = this.StartingPreviousPosition,
                                    RightControlPoint = this.StartingPreviousPosition,
                                    IsSmooth = this.Mode is PenCurveTool.Curve,
                                });

                                layer.Anchors.ClosePoint = this.StartingPreviousPosition;
                                layer.Anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                                layer.Anchors.Segment(this.CanvasControl, false);
                            }
                            break;
                        case PenCurveTool.RectChoose:
                            this.StartingPosition = this.Position = this.ToPosition(point);
                            this.TransformerRect = new TransformerRect(this.Position, this.Position);
                            layer.Anchors.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (layer is null) break;

                            this.StartingPosition = this.Position = this.ToPosition(point);

                            layer.Anchors.Index = -1;
                            foreach (Anchor item in layer.Anchors)
                            {
                                item.CacheTransform();
                                if (FanKit.Math.InNodeRadius(this.Position, item.Point))
                                {
                                    layer.Anchors.Index = layer.Anchors.IndexOf(item);
                                }
                            }
                            break;
                        case PenCurveTool.Hitter:
                            this.StartingPosition = this.Position = this.ToPosition(point);
                            foreach (Anchor item in layer.Anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) break;
                            }
                            break;
                        case PenCurveTool.Cutter:
                            this.StartingPosition = this.Position = this.ToPosition(point);
                            foreach (Anchor item in layer.Anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                                if (this.Cutter.IsHit) break;
                            }
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            this.PreviousPosition = this.StartingPreviousPosition + this.Position - this.StartingPosition;

                            layer.Anchors.ClosePoint = this.PreviousPosition;
                            layer.Anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                            layer.Anchors.Segment(this.CanvasControl, false);
                            layer.Anchors.Invalidate();
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = new TransformerRect(this.StartingPosition, this.Position);
                            layer.Anchors.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (layer.Anchors.Index is -1)
                            {
                                foreach (Anchor item in layer.Anchors)
                                {
                                    if (item.IsChecked)
                                    {
                                        item.TransformAdd(this.Position - this.StartingPosition);
                                    }
                                }
                            }
                            else
                            {
                                Anchor item = layer.Anchors.SelectedItem;
                                item.TransformAdd(this.Position - this.StartingPosition);
                            }

                            layer.Anchors.Segment(this.CanvasControl);
                            layer.Anchors.Invalidate();
                            break;
                        case PenCurveTool.Hitter:
                            foreach (Anchor item in layer.Anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) break;
                            }
                            break;
                        case PenCurveTool.Cutter:
                            foreach (Anchor item in layer.Anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                                if (this.Cutter.IsHit) break;
                            }
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    CurveLayer layer = this.ObservableCollection[index];
                    if (layer is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            this.PreviousPosition = this.StartingPreviousPosition + this.Position - this.StartingPosition;

                            layer.Anchors.ClosePoint = this.PreviousPosition;
                            layer.Anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                            layer.Anchors.Segment(this.CanvasControl, false);
                            layer.Anchors.Invalidate();
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = default;
                            break;
                        case PenCurveTool.Move:
                            break;
                        case PenCurveTool.Hitter:
                        case PenCurveTool.Cutter:
                            if (this.StartingPosition == this.Position)
                            {
                                Anchor previous = null;
                                Anchor current = null;
                                foreach (Anchor item in layer.Anchors)
                                {
                                    if (item is null) continue;

                                    if (previous is null)
                                    {
                                        if (item.Geometry is null) continue;

                                        this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                        if (this.Hitter.IsHit)
                                        {
                                            previous = item;
                                        }
                                    }
                                    else
                                    {
                                        current = item;
                                        break;
                                    }
                                }

                                if (previous is null) return;
                                if (current is null)
                                {
                                    layer.Anchors.Add(new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Position,
                                        Pressure = (previous.Pressure) / 2
                                    });
                                }
                                else
                                {
                                    layer.Anchors.Insert(layer.Anchors.IndexOf(current), new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Hitter.Target,
                                        Pressure = (previous.Pressure + current.Pressure) / 2
                                    });
                                }

                                layer.Anchors.Segment(this.CanvasControl);
                                layer.Anchors.Invalidate();
                            }
                            else
                            {
                                Anchor previous = null;
                                Anchor current = null;
                                foreach (Anchor item in layer.Anchors)
                                {
                                    if (item is null) continue;

                                    if (previous is null)
                                    {
                                        if (item.Geometry is null) continue;

                                        this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 8 / this.Transformer.Scale);
                                        if (this.Cutter.IsHit)
                                        {
                                            previous = item;
                                        }
                                    }
                                    else
                                    {
                                        current = item;
                                        break;
                                    }
                                }

                                if (previous is null) return;
                                if (current is null)
                                {
                                    layer.Anchors.Add(new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Position,
                                        Pressure = (previous.Pressure) / 2
                                    });
                                }
                                else
                                {
                                    layer.Anchors.Insert(layer.Anchors.IndexOf(current), new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Cutter.Target,
                                        Pressure = (previous.Pressure + current.Pressure) / 2
                                    });
                                }

                                layer.Anchors.Segment(this.CanvasControl);
                                layer.Anchors.Invalidate();
                            }
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        public async Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".bmp"
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.CanvasBitmap = bitmap;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}