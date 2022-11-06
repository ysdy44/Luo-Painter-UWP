using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Historys.Models;
using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Projects;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Reflection.Metadata;
using Windows.UI.Popups;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTransformShaderCodeBytes;
        byte[] GradientMappingShaderCodeBytes;
        byte[] RippleEffectShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;
        byte[] DottedLineTransformShaderCodeBytes;
        byte[] LalphaMaskShaderCodeBytes;
        byte[] RalphaMaskShaderCodeBytes;
        byte[] DisplacementLiquefactionShaderCodeBytes;
        byte[] LalphaMaskEffectShaderCodeBytes;
        byte[] RalphaMaskEffectShaderCodeBytes;

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;


        private void CreateResources(int width, int height)
        {
            float scale = this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(25);
            using (ScaleEffect scaleEffect = new ScaleEffect
            {
                Scale = new Vector2(scale),
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Source = this.GrayAndWhiteMesh
            })
            using (BorderEffect borderEffect = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = scaleEffect
            })
            {
                this.Mesh = new BitmapLayer(this.CanvasDevice, borderEffect, width, height);
            }

            this.Clipboard = new BitmapLayer(this.CanvasDevice, width, height);

            this.Displacement = new BitmapLayer(this.CanvasDevice, width, height);
            this.Displacement.RenderThumbnail();
        }

        private void CreateMarqueeResources(int width, int height)
        {
            this.Marquee = new BitmapLayer(this.CanvasDevice, width, height);
            this.Marquee.RenderThumbnail();

            this.MarqueeImage.Source = this.Marquee.Thumbnail;
        }


        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTransformShaderCodeBytes = await ShaderType.FreeTransform.LoadAsync();
            this.GradientMappingShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
            this.RippleEffectShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
            this.DifferenceShaderCodeBytes = await ShaderType.Difference.LoadAsync();
            this.LalphaMaskShaderCodeBytes = await ShaderType.LalphaMask.LoadAsync();
            this.RalphaMaskShaderCodeBytes = await ShaderType.RalphaMask.LoadAsync();
            this.DisplacementLiquefactionShaderCodeBytes = await ShaderType.DisplacementLiquefaction.LoadAsync();
            this.LalphaMaskEffectShaderCodeBytes = await ShaderType.LalphaMaskEffect.LoadAsync();
            this.RalphaMaskEffectShaderCodeBytes = await ShaderType.RalphaMaskEffect.LoadAsync();

            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
            this.BrushEdgeHardnessWithTextureShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();
        }

        private async Task CreateDottedLineResourcesAsync()
        {
            this.DottedLineTransformShaderCodeBytes = await ShaderType.DottedLineTransform.LoadAsync();
        }

        public async void AddAsync(IEnumerable<IStorageFile> items)
        {
            if (items is null) return;
            if (items.Count() is 0) return;

            Layerage[] undo = this.Nodes.Convert();

            int count = 0;
            int index = this.LayerSelectedIndex;
            if (index > 0 && this.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = this.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = this.Nodes.IndexOf(neighbor);

                    foreach (IStorageFile item in items)
                    {
                        using (CanvasBitmap bitmap = await this.CreateBitmap(item))
                        {
                            if (bitmap is null)
                            {
                                await new MessageDialog(item.Path, "File not found.").ShowAsync();
                                continue;
                            }

                            BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                            this.Nodes.Insert(indexChild, add);
                            this.ObservableCollection.InsertChild(index, add);
                            count++;
                        }
                    }
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    foreach (IStorageFile item in items)
                    {
                        using (CanvasBitmap bitmap = await this.CreateBitmap(item))
                        {
                            if (bitmap is null)
                            {
                                await new MessageDialog(item.Path, "File not found.").ShowAsync();
                                continue;
                            }

                            BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                            parent.Children.Insert(indexChild, add);
                            this.ObservableCollection.InsertChild(index, add);
                            count++;
                        }
                    }
                }

                this.LayerSelectedIndex = index;
            }
            else
            {
                foreach (IStorageFile item in items)
                {
                    using (CanvasBitmap bitmap = await this.CreateBitmap(item))
                    {
                        if (bitmap is null)
                        {
                            await new MessageDialog(item.Path, "File not found.").ShowAsync();
                            continue;
                        }

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                        this.Nodes.Insert(0, add);
                        this.ObservableCollection.InsertChild(0, add);
                        count++;
                    }
                }

                this.LayerSelectedIndex = 0;
            }

            /// History
            Layerage[] redo = this.Nodes.Convert();
            int removes = this.History.Push(new ArrangeHistory(undo, redo));

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

        private async Task<CanvasBitmap> CreateBitmap(IRandomAccessStreamReference item)
        {
            if (item is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await item.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(this.CanvasDevice, stream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}