using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        float[] RedHistogram;
        float[] GreenHistogram;
        float[] BlueHistogram;

        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTransformShaderCodeBytes;
        byte[] GradientMappingShaderCodeBytes;
        byte[] RippleEffectShaderCodeBytes;
        byte[] ThresholdShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;
        byte[] DottedLineTransformShaderCodeBytes;
        byte[] LalphaMaskShaderCodeBytes;
        byte[] RalphaMaskShaderCodeBytes;
        byte[] DisplacementLiquefactionShaderCodeBytes;
        byte[] LalphaMaskEffectShaderCodeBytes;
        byte[] RalphaMaskEffectShaderCodeBytes;

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;


        readonly BorderEffect Turbulences = new BorderEffect
        {
            ExtendX = CanvasEdgeBehavior.Mirror,
            ExtendY = CanvasEdgeBehavior.Mirror,
            Source = new TurbulenceEffect()
        };
        readonly BorderEffect RalphaTurbulences = new BorderEffect
        {
            ExtendX = CanvasEdgeBehavior.Mirror,
            ExtendY = CanvasEdgeBehavior.Mirror,
            Source = new AlphaMaskEffect
            {
                AlphaMask = new TurbulenceEffect(),
                Source = new ColorSourceEffect
                {
                    Color = Windows.UI.Colors.White
                }
            }
        };


        private void CreateResources(int width, int height)
        {
            //@DPI
            this.Mesh = new CanvasRenderTarget(this.CanvasDevice, width, height, 96);

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
            using (CanvasDrawingSession ds = this.Mesh.CreateDrawingSession())
            {
                ds.DrawImage(borderEffect);
            }

            this.Clipboard = new BitmapLayer(this.CanvasDevice, width, height);

            this.Displacement = new BitmapLayer(this.CanvasDevice, width, height);
            this.Displacement.RenderThumbnail();
        }

        private void CreateMarqueeResources(int width, int height)
        {
            this.Marquee = new BitmapLayer(this.CanvasDevice, width, height);
            this.Marquee.RenderThumbnail();

            this.LayerListView.Source = this.Marquee.Thumbnail;
        }


        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTransformShaderCodeBytes = await ShaderType.FreeTransform.LoadAsync();
            this.GradientMappingShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
            this.RippleEffectShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
            this.ThresholdShaderCodeBytes = await ShaderType.Threshold.LoadAsync();
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
                        CanvasBitmap bitmap = await this.CreateBitmap(item);
                        if (bitmap is null)
                        {
                            await new MessageDialog(item.Path, App.Resource.GetString($"Tip_{TipType.NoSupport}")).ShowAsync();
                            continue;
                        }

                        StorageFile copy = await item.CopyAsync(ApplicationData.Current.TemporaryFolder, item.Name, NameCollisionOption.GenerateUniqueName);
                        ImageLayer.Instance.Add(copy.Name, bitmap);

                        ImageLayer add = new ImageLayer(this.CanvasDevice, copy.Name, this.Transformer.Width, this.Transformer.Height);

                        this.Nodes.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                        count++;
                    }
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    foreach (IStorageFile item in items)
                    {
                        CanvasBitmap bitmap = await this.CreateBitmap(item);
                        if (bitmap is null)
                        {
                            await new MessageDialog(item.Path, App.Resource.GetString($"Tip_{TipType.NoSupport}")).ShowAsync();
                            continue;
                        }

                        StorageFile copy = await item.CopyAsync(ApplicationData.Current.TemporaryFolder, item.Name, NameCollisionOption.GenerateUniqueName);
                        ImageLayer.Instance.Add(copy.Name, bitmap);

                        ImageLayer add = new ImageLayer(this.CanvasDevice, copy.Name, this.Transformer.Width, this.Transformer.Height);

                        parent.Children.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                        count++;
                    }
                }

                this.LayerSelectedIndex = index;
            }
            else
            {
                foreach (IStorageFile item in items)
                {
                    CanvasBitmap bitmap = await this.CreateBitmap(item);
                    if (bitmap is null)
                    {
                        await new MessageDialog(item.Path, App.Resource.GetString($"Tip_{TipType.NoSupport}")).ShowAsync();
                        continue;
                    }

                    StorageFile copy = await item.CopyAsync(ApplicationData.Current.TemporaryFolder, item.Name, NameCollisionOption.GenerateUniqueName);
                    ImageLayer.Instance.Add(copy.Name, bitmap);

                    ImageLayer add = new ImageLayer(this.CanvasDevice, copy.Name, this.Transformer.Width, this.Transformer.Height);

                    this.Nodes.Insert(0, add);
                    this.ObservableCollection.InsertChild(0, add);
                    count++;
                }

                this.LayerSelectedIndex = 0;
            }

            // History
            Layerage[] redo = this.Nodes.Convert();
            IHistory history = new ArrangeHistory(undo, redo);
            int removes = this.History.Push(history);

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