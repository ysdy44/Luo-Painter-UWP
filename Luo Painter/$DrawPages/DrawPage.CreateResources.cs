using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Projects;
using Luo_Painter.Shaders;
using System.Threading.Tasks;
using System.Xml.Linq;
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


        private void CreateResources(int width, int height)
        {
            this.Mesh = new Mesh(this.CanvasDevice, this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(25), width, height);
            this.Clipboard = new BitmapLayer(this.CanvasDevice, width, height);

            this.Displacement = new BitmapLayer(this.CanvasDevice, width, height);
            this.Displacement.RenderThumbnail();
        }

        private void CreateMarqueeResources(int width, int height)
        {
            this.Marquee = new BitmapLayer(this.CanvasDevice, width, height);
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
        }

        private async Task CreateDottedLineResourcesAsync()
        {
            this.DottedLineTransformShaderCodeBytes = await ShaderType.DottedLineTransform.LoadAsync();
        }


        public void Navigated(ProjectParameter item)
        {
            switch (item.Type)
            {
                case StorageItemTypes.None:
                    BitmapLayer bitmapLayer1 = new BitmapLayer(this.CanvasDevice, item.Width, item.Height);
                    this.Nodes.Add(bitmapLayer1);
                    this.ObservableCollection.Add(bitmapLayer1);

                    this.LayerSelectedIndex = 0;
                    break;
                case StorageItemTypes.File:
                    BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasDevice, item.Bitmap, item.Width, item.Height);
                    this.Nodes.Add(bitmapLayer2);
                    this.ObservableCollection.Add(bitmapLayer2);

                    this.LayerSelectedIndex = 0;
                    break;
                case StorageItemTypes.Folder:
                    // 2. Load Layers.xml
                    // Layers
                    XDocument docLayers = XDocument.Load(item.DocLayers);
                    foreach (XElement item2 in docLayers.Root.Elements("Layer"))
                    {
                        if (item2.Attribute("Id") is XAttribute id2 && item2.Attribute("Type") is XAttribute type2)
                        {
                            string id = id2.Value;
                            if (string.IsNullOrEmpty(id)) continue;

                            string type = type2.Value;
                            if (string.IsNullOrEmpty(id)) continue;

                            switch (type)
                            {
                                case "Bitmap":
                                    BitmapLayer bitmapLayer3 =
                                        item.Bitmaps.ContainsKey(id) ?
                                        new BitmapLayer(id, this.CanvasDevice, item.Bitmaps[id], item.Width, item.Height) :
                                        new BitmapLayer(id, this.CanvasDevice, item.Width, item.Height);
                                    bitmapLayer3.Load(item2);
                                    break;
                                case "Group":
                                    GroupLayer groupLayer = new GroupLayer(id, this.CanvasDevice, item.Width, item.Height);
                                    groupLayer.Load(item2);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                    // 2. Load Project.xml
                    // Nodes 
                    XDocument docProject = XDocument.Load(item.DocProject);
                    if (docProject.Root.Element("Layerages") is XElement layerages)
                    {
                        this.Nodes.Load(layerages);
                    }

                    // 4. UI
                    foreach (ILayer item2 in this.Nodes)
                    {
                        item2.Arrange(0);
                        this.ObservableCollection.AddChild(item2);
                    }

                    if (docProject.Root.Element("Index") is XElement index)
                        this.LayerSelectedIndex = (int)index;
                    else if (this.ObservableCollection.Count > 0)
                        this.LayerSelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

    }
}