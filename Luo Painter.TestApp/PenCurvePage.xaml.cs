using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
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

        CurveLayer CurveLayer;
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
            this.ListBox.ItemsSource = System.Enum.GetValues(typeof(PenCurveTool));
            this.ListBox.SelectedIndex = 0;

            this.ResetPressureButton.Click += (s, e) =>
            {
                AnchorCollection anchors = this.CurveLayer.SelectedItem;
                if (anchors is null) return;

                foreach (Anchor item in anchors)
                {
                    item.Pressure = 1;
                }

                anchors.Segment(this.CanvasControl);
                anchors.Invalidate();
                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.PressureSlider.ValueChanged += (s, e) =>
            {
                float value = this.ToPressure((float)e.NewValue);

                bool changedAll = false;
                foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                {
                    bool changed = false;
                    foreach (Anchor item in anchors)
                    {
                        if (item.IsChecked is false) continue;
                        item.Pressure = value;
                        changed = true;
                    }
                    if (changed is false) return;

                    anchors.Segment(this.CanvasControl);
                    anchors.Invalidate();
                    changedAll = true;
                }
                if (changedAll is false) return;

                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (e.NewColor == e.OldColor) return;

                foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                {
                    anchors.Color = e.NewColor;
                    anchors.Invalidate();
                }

                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.StrokeWidthSlider.ValueChanged += (s, e) =>
            {
                if (e.NewValue == e.OldValue) return;

                foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                {
                    anchors.StrokeWidth = (float)e.NewValue;
                    anchors.Segment(this.CanvasControl);
                    anchors.Invalidate();
                }

                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.CloseButton.Click += (s, e) =>
            {
                AnchorCollection anchors = this.CurveLayer.SelectedItem;
                if (anchors is null) return;

                if (anchors.IsClosed) return;
                anchors.IsClosed = true;

                anchors.Segment(this.CanvasControl);
                anchors.Invalidate();
                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                AnchorCollection anchors = this.CurveLayer.SelectedItem;
                if (anchors is null) return;

                Anchor[] uncheck = anchors.Where(c => c.IsChecked is false).ToArray();
                if (uncheck.Count() < 2)
                {
                    this.CurveLayer.RemoveSelectedItem();
                    this.CurveLayer.Invalidate();
                    this.CanvasControl.Invalidate(); // Invalidate
                    return;
                }

                anchors.Clear();
                foreach (Anchor item in uncheck)
                {
                    anchors.Add(item);
                }

                anchors.Segment(this.CanvasControl);
                anchors.Invalidate();
                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
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
                this.CurveLayer = new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height);
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

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CurveLayer.Source,
                    TransformMatrix = this.Transformer.GetMatrix(),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                });


                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);
                foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                {
                    args.DrawingSession.DrawAnchorCollection(anchors, matrix);
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
            this.Operator.Single_Start += (point, device, properties) =>
            {
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        {
                            AnchorCollection anchors = this.CurveLayer.SelectedItem;
                            if (anchors is null is false && anchors.IsClosed is false)
                            {
                                this.StartingPreviousPosition = this.PreviousPosition;
                                this.StartingPosition = this.Position = this.ToPosition(point);

                                anchors.Add(new Anchor
                                {
                                    Point = this.StartingPreviousPosition,
                                    LeftControlPoint = this.StartingPreviousPosition,
                                    RightControlPoint = this.StartingPreviousPosition,
                                    IsSmooth = this.Mode is PenCurveTool.Curve,
                                });

                                anchors.ClosePoint = this.StartingPreviousPosition;
                                anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                                anchors.Segment(this.CanvasControl, false);
                                break;
                            }
                        }

                        {
                            this.StartingPreviousPosition = this.PreviousPosition =
                            this.StartingPosition = this.Position =
                            this.ToPosition(point);

                            AnchorCollection anchors = new AnchorCollection(this.CanvasControl, this.Transformer.Width, this.Transformer.Height)
                            {
                                new Anchor
                                {
                                    Point = this.Position,
                                    LeftControlPoint = this.Position,
                                    RightControlPoint = this.Position,
                                    IsSmooth = this.Mode is PenCurveTool.Curve,
                                }
                            };

                            anchors.Color = this.ColorPicker.Color;

                            anchors.ClosePoint = this.Position;
                            anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                            anchors.Segment(this.CanvasControl, false);

                            int count = this.CurveLayer.Anchorss.Count;
                            this.CurveLayer.Anchorss.Add(anchors);
                            this.CurveLayer.Index = count;
                        }

                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    case PenCurveTool.RectChoose:
                        {
                            this.StartingPosition = this.Position = this.ToPosition(point);
                            this.TransformerRect = new TransformerRect(this.Position, this.Position);

                            this.CurveLayer.BoxChoose(this.TransformerRect);
                            this.CanvasControl.Invalidate(); // Invalidate
                        }
                        break;
                    case PenCurveTool.Move:
                        this.StartingPosition = this.Position = this.ToPosition(point);

                        foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                        {
                            anchors.CacheTransform();
                        }

                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    case PenCurveTool.Hitter:
                        this.StartingPosition = this.Position = this.ToPosition(point);

                        foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                        {
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit)
                                {
                                    this.CanvasControl.Invalidate(); // Invalidatex
                                    return;
                                }
                            }
                        }
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    case PenCurveTool.Cutter:
                        this.StartingPosition = this.Position = this.ToPosition(point);

                        foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                        {
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                                if (this.Cutter.IsHit)
                                {
                                    this.CanvasControl.Invalidate(); // Invalidatex
                                    return;
                                }
                            }
                        }
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);

                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        {
                            AnchorCollection anchors = this.CurveLayer.SelectedItem;
                            if (anchors is null) return;

                            this.PreviousPosition = this.StartingPreviousPosition + this.Position - this.StartingPosition;

                            anchors.ClosePoint = this.PreviousPosition;
                            anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                            anchors.Segment(this.CanvasControl, false);
                            anchors.Invalidate();
                            this.CurveLayer.Invalidate();
                            this.CanvasControl.Invalidate(); // Invalidatex
                        }
                        break;
                    case PenCurveTool.RectChoose:
                        this.TransformerRect = new TransformerRect(this.StartingPosition, this.Position);
                        this.CurveLayer.BoxChoose(this.TransformerRect);
                        this.CanvasControl.Invalidate(); // Invalidatex
                        break;
                    case PenCurveTool.Move:
                        {
                            bool changedAll = false;
                            foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                            {
                                bool changed = false;
                                foreach (Anchor item in anchors)
                                {
                                    if (item.IsChecked is false) continue;
                                    item.TransformAdd(this.Position - this.StartingPosition);
                                    changed = true;
                                }
                                if (changed is false) continue;

                                anchors.Segment(this.CanvasControl);
                                anchors.Invalidate();
                                changedAll = true;
                            }
                            if (changedAll is false) return;

                            this.CurveLayer.Invalidate();
                            this.CanvasControl.Invalidate(); // Invalidatex
                        }
                        break;
                    case PenCurveTool.Hitter:
                        foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                        {
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Hitter.Hit(item.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit)
                                {
                                    this.CanvasControl.Invalidate(); // Invalidate
                                    return;
                                }
                            }
                        }
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    case PenCurveTool.Cutter:
                        foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                        {
                            foreach (Anchor item in anchors)
                            {
                                if (item.Geometry is null) continue;
                                this.Cutter.Hit(item.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                                if (this.Cutter.IsHit)
                                {
                                    this.CanvasControl.Invalidate(); // Invalidate
                                    return;
                                }
                            }
                        }
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);

                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        {
                            AnchorCollection anchors = this.CurveLayer.SelectedItem;
                            if (anchors is null) return;

                            this.PreviousPosition = this.StartingPreviousPosition + this.Position - this.StartingPosition;

                            anchors.ClosePoint = this.PreviousPosition;
                            anchors.CloseIsSmooth = this.Mode is PenCurveTool.Curve;
                            anchors.Segment(this.CanvasControl);
                            anchors.Invalidate();
                            this.CurveLayer.Invalidate();
                            this.CanvasControl.Invalidate(); // Invalidate
                        }
                        break;
                    case PenCurveTool.RectChoose:
                        this.TransformerRect = default;
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    case PenCurveTool.Move:
                        break;
                    case PenCurveTool.Hitter:
                    case PenCurveTool.Cutter:
                        if (this.StartingPosition == this.Position)
                        {
                            foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                            {
                                Anchor previous = null;
                                Anchor current = null;
                                foreach (Anchor item in anchors)
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
                                    anchors.Add(new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Position,
                                        LeftControlPoint = this.Position,
                                        RightControlPoint = this.Position,
                                        Pressure = previous.Pressure
                                    });
                                }
                                else
                                {
                                    anchors.Insert(anchors.IndexOf(current), new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Hitter.Target,
                                        LeftControlPoint = this.Hitter.Target,
                                        RightControlPoint = this.Hitter.Target,
                                        Pressure = (previous.Pressure + current.Pressure) / 2
                                    });
                                }

                                anchors.Segment(this.CanvasControl);
                                anchors.Invalidate();
                                this.CurveLayer.Invalidate();
                                this.CanvasControl.Invalidate(); // Invalidate
                            }
                        }
                        else
                        {
                            foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                            {
                                Anchor previous = null;
                                Anchor current = null;
                                foreach (Anchor item in anchors)
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
                                    anchors.Add(new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Position,
                                        LeftControlPoint = this.Position,
                                        RightControlPoint = this.Position,
                                        Pressure = (previous.Pressure) / 2
                                    });
                                }
                                else
                                {
                                    anchors.Insert(anchors.IndexOf(current), new Anchor
                                    {
                                        IsSmooth = true,
                                        Point = this.Cutter.Target,
                                        LeftControlPoint = this.Cutter.Target,
                                        RightControlPoint = this.Cutter.Target,
                                        Pressure = (previous.Pressure + current.Pressure) / 2
                                    });
                                }

                                anchors.Segment(this.CanvasControl);
                                anchors.Invalidate();
                                this.CurveLayer.Invalidate();
                                this.CanvasControl.Invalidate(); // Invalidate
                            }
                        }
                        break;
                    default:
                        break;
                }
            };


            // Right
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point, isHolding) =>
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