using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
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

        readonly ObservableCollection<AnchorCollection> ObservableCollection = new ObservableCollection<AnchorCollection>
        {
            new AnchorCollection()
        };

        CanvasBitmap CanvasBitmap;
        Transformer Border;

        TransformerRect TransformerRect;
        Vector2 StartingPosition;
        Vector2 Position;

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

                this.ObservableCollection.Add(new AnchorCollection());
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
            this.ListBox.SelectionChanged += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection anchors = this.ObservableCollection[index];
                    if (anchors is null) return;

                    anchors.BuildGeometry(this.CanvasControl);
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.ResetPressureButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection curve = this.ObservableCollection[index];
                    if (curve is null) return;

                    foreach (Anchor item in curve)
                    {
                        item.Pressure = 1;
                    }

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.PressureSlider.ValueChanged += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection curve = this.ObservableCollection[index];
                    if (curve is null) return;

                    float value = (float)e.NewValue;
                    if (value > 0)
                    {
                        foreach (Anchor item in curve)
                        {
                            if (item.IsChecked) item.Pressure = 1 + value;
                        }
                    }
                    else if (value < 0)
                    {
                        foreach (Anchor item in curve)
                        {
                            if (item.IsChecked) item.Pressure = -1 / (value - 1);
                        }
                    }
                    else
                    {
                        foreach (Anchor item in curve)
                        {
                            if (item.IsChecked) item.Pressure = 1;
                        }
                    }

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
                    AnchorCollection curve = this.ObservableCollection[index];

                    if (curve is null)
                    {
                        this.ObservableCollection.RemoveAt(index);
                        this.ListView.SelectedIndex = index - 1;

                        this.CanvasControl.Invalidate(); // Invalidate
                        return;
                    }

                    Anchor[] uncheck = curve.Where(c => c.IsChecked is false).ToArray();
                    if (uncheck.Length < 2)
                    {
                        this.ObservableCollection.RemoveAt(index);
                        this.ListView.SelectedIndex = index - 1;

                        this.CanvasControl.Invalidate(); // Invalidate
                        return;
                    }

                    curve.Clear();
                    foreach (Anchor item in uncheck)
                    {
                        curve.Add(item);
                    }

                    curve.BuildGeometry(this.CanvasControl);
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
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CanvasBitmap,
                    TransformMatrix = matrix
                });

                foreach (AnchorCollection items in this.ObservableCollection)
                {
                    if (items is null) continue;
                    if (items.Count is 0) continue;

                    Anchor previousItem = null;
                    foreach (Anchor item in items)
                    {
                        if (previousItem is null is false)
                        {
                            if (previousItem.Geometry is null is false)
                            {
                                if (previousItem.Pressure == item.Pressure)
                                {
                                    float sw = item.Pressure * 2 * this.Transformer.Scale;

                                    for (float i = 0; i < previousItem.ComputePathLength; i += sw)
                                    {
                                        Vector2 p = this.ToPoint(previousItem.Geometry.ComputePointOnPath(i));
                                        args.DrawingSession.FillCircle(p, sw + sw, Colors.Red);
                                    }
                                }
                                else
                                {
                                    float i = 0;
                                    do
                                    {
                                        Vector2 p = this.ToPoint(previousItem.Geometry.ComputePointOnPath(i));

                                        float percentage = i / previousItem.ComputePathLength;
                                        float cos = (float)System.Math.Cos(percentage * System.Math.PI) / 2 + 0.5f;
                                        float strokeWidth = cos * previousItem.Pressure + (1 - cos) * item.Pressure;

                                        if (strokeWidth < 0.2f)
                                        {
                                            i += 0.5f;
                                        }
                                        else
                                        {
                                            float sw = strokeWidth * 2 * this.Transformer.Scale;
                                            args.DrawingSession.FillCircle(p, sw + sw, Colors.Red);
                                            i += sw;
                                        }
                                    } while (i < previousItem.ComputePathLength);
                                }
                            }
                        }

                        previousItem = item;
                    }

                    previousItem = items.Last();
                    if (previousItem is null is false)
                    {
                        if (previousItem.Geometry is null is false)
                        {
                            if (previousItem.Pressure == 1)
                            {
                                float sw = 2 * this.Transformer.Scale;

                                for (float i = 0; i < previousItem.ComputePathLength; i += sw)
                                {
                                    Vector2 p = this.ToPoint(previousItem.Geometry.ComputePointOnPath(i));
                                    args.DrawingSession.FillCircle(p, sw + sw, Colors.Red);
                                }
                            }
                            else
                            {
                                float i = 0;
                                do
                                {
                                    Vector2 p = this.ToPoint(previousItem.Geometry.ComputePointOnPath(i));

                                    float percentage = i / previousItem.ComputePathLength;
                                    float cos = (float)System.Math.Cos(percentage * System.Math.PI) / 2 + 0.5f;
                                    float strokeWidth = cos * previousItem.Pressure + (1 - cos);

                                    if (strokeWidth < 0.2f)
                                    {
                                        i += 0.5f;
                                    }
                                    else
                                    {
                                        float sw = strokeWidth * 2 * this.Transformer.Scale;
                                        args.DrawingSession.FillCircle(p, sw + sw, Colors.Red);
                                        i += sw;
                                    }
                                } while (i < previousItem.ComputePathLength);
                            }
                        }
                    }

                }

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection anchors = this.ObservableCollection[index];
                    if (anchors is null is false)
                    {
                        foreach (Anchor item in anchors)
                        {
                            if (item is null) continue;

                            if (item.Geometry is null is false)
                            {
                                args.DrawingSession.DrawGeometry(this.ToPoint(item.Geometry), Colors.DodgerBlue, 1);
                            }

                            Vector2 p = this.ToPoint(item.Point);
                            if (item.IsSmooth)
                            {
                                if (item.IsChecked) args.DrawingSession.FillCircle(p, 4, Colors.White);
                                args.DrawingSession.FillCircle(p, 3, Colors.DodgerBlue);
                            }
                            else
                            {
                                if (item.IsChecked) args.DrawingSession.FillRectangle(p.X - 3, p.Y - 3, 6, 6, Colors.White);
                                args.DrawingSession.FillRectangle(p.X - 2, p.Y - 2, 4, 4, Colors.DodgerBlue);
                            }
                        }

                        //for (int i = 0; i < anchors.Count; i++)
                        //{
                        //    args.DrawingSession.DrawText(i.ToString(), Vector2.Transform(anchors[i].Point, matrix) - new Vector2(40, 40), Colors.Red);
                        //}
                    }
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
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection anchors = this.ObservableCollection[index];

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            if (anchors is null)
                            {
                                this.ObservableCollection[index] = new AnchorCollection
                                {
                                    new Anchor
                                    {
                                        Point = this.Position,
                                        IsSmooth = this.Mode is PenCurveTool.Curve,
                                    }
                                };
                            }
                            else
                            {
                                anchors.Add(new Anchor
                                {
                                    Point = this.Position,
                                    IsSmooth = this.Mode is PenCurveTool.Curve,
                                });
                            }
                            anchors.BuildGeometry(this.CanvasControl, this.Position, this.Mode is PenCurveTool.Curve);
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = new TransformerRect(this.Position, this.Position);
                            anchors.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (anchors is null) break;

                            anchors.Index = -1;
                            foreach (Anchor item in anchors)
                            {
                                item.CacheTransform();
                                if (FanKit.Math.InNodeRadius(this.Position, item.Point))
                                {
                                    anchors.Index = anchors.IndexOf(item);
                                }
                            }
                            break;
                        case PenCurveTool.Hitter:
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) break;
                            }
                            break;
                        case PenCurveTool.Cutter:
                            foreach (Anchor item in anchors)
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

                this.StartingPosition = this.Position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection anchors = this.ObservableCollection[index];
                    if (anchors is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            anchors.BuildGeometry(this.CanvasControl, this.Position, this.Mode is PenCurveTool.Curve);
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = new TransformerRect(this.StartingPosition, this.Position);
                            anchors.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (anchors.Index is -1)
                            {
                                foreach (Anchor item in anchors)
                                {
                                    if (item.IsChecked)
                                    {
                                        item.TransformAdd(this.Position - this.StartingPosition);
                                    }
                                }
                            }
                            else
                            {
                                Anchor item = anchors.SelectedItem;
                                item.TransformAdd(this.Position - this.StartingPosition);
                            }

                            anchors.BuildGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.Hitter:
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) break;
                            }
                            break;
                        case PenCurveTool.Cutter:
                            foreach (Anchor item in anchors)
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
                    AnchorCollection anchors = this.ObservableCollection[index];
                    if (anchors is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            anchors.BuildGeometry(this.CanvasControl, this.Position, this.Mode is PenCurveTool.Curve);
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
                                Anchor target = null;
                                foreach (Anchor item in anchors)
                                {
                                    this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                    if (this.Hitter.IsHit)
                                    {
                                        target = item;
                                        break;
                                    }
                                }
                                if (target is null) return;

                                anchors.Insert(anchors.IndexOf(target) + 1, new Anchor
                                {
                                    IsSmooth = true,
                                    Point = this.Hitter.Target,
                                });
                                anchors.BuildGeometry(this.CanvasControl);
                            }
                            else
                            {
                                Anchor target = null;
                                foreach (Anchor item in anchors)
                                {
                                    this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 8 / this.Transformer.Scale);
                                    if (this.Cutter.IsHit)
                                    {
                                        target = item;
                                        break;
                                    }
                                }
                                if (target is null) return;

                                anchors.Insert(anchors.IndexOf(target) + 1, new Anchor
                                {
                                    IsSmooth = true,
                                    Point = this.Cutter.Target,
                                });
                                anchors.BuildGeometry(this.CanvasControl);
                            }
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                    case PenCurveTool.Hitter:
                        break;
                    default:
                        return;
                }

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                switch (pp.PointerDevice.PointerDeviceType)
                {
                    case PointerDeviceType.Touch:
                        return;
                }

                this.Position = this.ToPosition(pp.Position.ToVector2());

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    AnchorCollection anchors = this.ObservableCollection[index];
                    if (anchors is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            anchors.BuildGeometry(this.CanvasControl, this.Position, this.Mode is PenCurveTool.Curve);
                            break;
                        case PenCurveTool.Hitter:
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) break;
                            }
                            break;
                        case PenCurveTool.Cutter:
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