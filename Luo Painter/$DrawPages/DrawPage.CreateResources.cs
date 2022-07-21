using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Projects;
using Luo_Painter.Shaders;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
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


        // Frist Open: Page.OnNavigatedTo (ReadyToDraw=false) > Canvas.CreateResources (ReadyToDraw=true)
        // Others Open: Page.OnNavigatedTo (ReadyToDraw=true)
        private bool ReadyToDraw => this.CanvasVirtualControl.ReadyToDraw;
        private bool IsEnabledToDraw { set => base.IsEnabled = value; }
        private string Title { get => this.ApplicationView.Title; set => this.ApplicationView.Title = value; }

        private StorageItemTypes NavigateType;


        private void CreateResources()
        {
            this.Transformer.Fit();
            this.ViewTool.Construct(this.Transformer);

            this.Mesh = new Mesh(this.CanvasDevice, this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(25), this.Transformer.Width, this.Transformer.Height);
            this.GradientMesh = new GradientMesh(this.CanvasDevice);
            this.Clipboard = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

            this.Displacement = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
            this.Displacement.RenderThumbnail();
        }

        private void CreateMarqueeResources()
        {
            this.Marquee = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
            this.Marquee.RenderThumbnail();

            this.LayerListView.MarqueeSource = this.Marquee.Thumbnail;
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


            // Load
            string path = this.Title;
            if (string.IsNullOrEmpty(path)) return;

            await this.Navigated(this.NavigateType);
        }

        private async Task CreateDottedLineResourcesAsync()
        {
            this.DottedLineTransformShaderCodeBytes = await ShaderType.DottedLineTransform.LoadAsync();
        }


        public async Task Navigated(StorageItemTypes type)
        {
            switch (this.NavigateType)
            {
                case StorageItemTypes.None:
                    this.CreateResources();
                    this.IsEnabledToDraw = true;
                    break;
                case StorageItemTypes.File:
                    await this.LoadImageAsync(this.Title);
                    this.CreateResources();
                    this.IsEnabledToDraw = true;
                    break;
                case StorageItemTypes.Folder:
                    await this.LoadAsync(this.Title);
                    this.CreateResources();
                    this.IsEnabledToDraw = true;
                    break;
                default:
                    break;
            }
        }

        public async Task Navigated(ProjectParameter item)
        {
            switch (item.Type)
            {
                case StorageItemTypes.None:
                    this.Load(item.Size);

                    if (this.ReadyToDraw)
                    {
                        this.CreateResources();
                        this.CreateMarqueeResources();
                        this.IsEnabledToDraw = true;
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                    }
                    break;
                case StorageItemTypes.File:
                    if (this.ReadyToDraw)
                    {
                        await this.LoadImageAsync(item.Path);
                        this.CreateResources();
                        this.CreateMarqueeResources();
                        this.IsEnabledToDraw = true;
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                    }
                    break;
                case StorageItemTypes.Folder:
                    if (this.ReadyToDraw)
                    {
                        await this.LoadAsync(item.Path);
                        this.CreateResources();
                        this.CreateMarqueeResources();
                        this.IsEnabledToDraw = true;
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                    }
                    break;
                default:
                    break;
            }
        }

    }
}