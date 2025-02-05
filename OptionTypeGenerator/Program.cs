using CategoryTreeNode = OptionTypeGenerator.Node;
using ItemTreeNode = OptionTypeGenerator.Node;
using RootTreeNode = OptionTypeGenerator.Node;

namespace OptionTypeGenerator
{
    // Code Generator for
    // ...\Luo Painter.Models\OptionType.cs
    internal class Program
    {
        public static int RootIndex;
        public static int CategoryIndex;
        public static int ItemIndex;

        public static List<RootTreeNode> Roots = new List<RootTreeNode>
        {
            new RootTreeNode("App", Bitwise.B24)
            {
                new CategoryTreeNode("File", Bitwise.B16)
                {
                    new ItemTreeNode("Home", IsItemClickEnabled: true),
                    new ItemTreeNode("Close", IsItemClickEnabled: true),
                    new ItemTreeNode("Save", IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Export", IsItemClickEnabled: true),
                    new ItemTreeNode("ExportAll", IsItemClickEnabled: true),
                    new ItemTreeNode("ExportCurrent", IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Undo", IsItemClickEnabled: true),
                    new ItemTreeNode("Redo", IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Layout", Bitwise.B16)
                {
                    new ItemTreeNode("FullScreen", IsItemClickEnabled: true),
                    new ItemTreeNode("UnFullScreen", IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("DockLeft", IsItemClickEnabled: true),
                    new ItemTreeNode("DockRight", IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Format", Bitwise.B16)
                {
                    new ItemTreeNode("JPEG", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PNG", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("BMP", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GIF", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("TIFF", ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Menu", Bitwise.B16)
                {
                    new ItemTreeNode("FileMenu", HasMenu: true, IsItemClickEnabled: true),
                    new ItemTreeNode("ExportMenu", HasMenu: true, IsItemClickEnabled: true),
                    new ItemTreeNode("HistogramMenu", HasMenu: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("ColorMenu", HasMenu: true, IsItemClickEnabled: true),
                    new ItemTreeNode("ColorHarmonyMenu", HasMenu: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("EditMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AdjustmentMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("OtherMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("PaintMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("BrushMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("SizeMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("EffectMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("HistoryMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("ToolMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("LayerMenu", HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("LayerNewMenu", HasMenu: true, IsItemClickEnabled: true),
                    new ItemTreeNode("LayerPropertyMenu", HasMenu: true, IsItemClickEnabled: true),
                    new ItemTreeNode("LayerRenameMenu", HasMenu: true, IsItemClickEnabled: true),
                },
            },
            new RootTreeNode("Option", Bitwise.B24)
            {
                new CategoryTreeNode("Edit", Bitwise.B16)
                {
                    new ItemTreeNode("Cut", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Copy", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Paste", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Clear", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Select", Bitwise.B16)
                {
                    new ItemTreeNode("All", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Deselect", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MarqueeInvert", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Pixel", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Marquees", Bitwise.B16)
                {
                    new ItemTreeNode("Feather", WithState: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MarqueeTransform", WithState: true, HasDifference: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Grow", WithState: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Shrink", WithState: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                CategoryTreeNode.Empty,
                new CategoryTreeNode("CropCanvas", Bitwise.B16 | Bitwise.B8, WithState: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true)
                {
                },
                new CategoryTreeNode("ResizeCanvas", Bitwise.B16, ExistIcon: true, IsItemClickEnabled: true)
                {
                    new ItemTreeNode("Stretch", WithState: true, HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Extend", WithState: true, HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Offset", WithState: true, HasMenu: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("RotateCanvas", Bitwise.B16, ExistIcon: true, IsItemClickEnabled: true)
                {
                    new ItemTreeNode("FlipHorizontal", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("FlipVertical", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("LeftTurn", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("RightTurn", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("OverTurn", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
            },
            new RootTreeNode("Layer", Bitwise.B24)
            {
                new CategoryTreeNode("New", Bitwise.B16)
                {
                    new ItemTreeNode("AddLayer", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AddBitmapLayer", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AddImageLayer", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AddCurveLayer", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AddFillLayer", ExistIcon: true, IsItemClickEnabled: true),
                },
                CategoryTreeNode.Empty,
                new CategoryTreeNode("Clipboard", Bitwise.B16)
                {
                    new ItemTreeNode("CutLayer", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("CopyLayer", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PasteLayer", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Layering", Bitwise.B16)
                {
                    new ItemTreeNode("Remove", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Duplicate", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Extract", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Merge", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Flatten", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Grouping", Bitwise.B16)
                {
                    new ItemTreeNode("Group", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Ungroup", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Release", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Combine", Bitwise.B16)
                {
                    new ItemTreeNode("Union", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Exclude", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Xor", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Intersect", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("ExpandStroke", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                CategoryTreeNode.Empty,
                new CategoryTreeNode("Transforms", Bitwise.B16)
                {
                    new ItemTreeNode("MirrorHorizontally", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MirrorVertically", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("RotateLeft", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("RotateRight", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Arrange", Bitwise.B16)
                {
                    new ItemTreeNode("BackOne", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("ForwardOne", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MoveBack", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MoveFront", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Align", Bitwise.B16)
                {
                    new ItemTreeNode("AlignLeft", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AlignCenter", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AlignRight", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("SpaceHorizontally", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("AlignTop", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AlignMiddle", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("AlignBottom", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("SpaceVertically", WithState: true, ExistIcon: true, IsItemClickEnabled: true),
                },
            },
            new RootTreeNode("Effect", Bitwise.B24)
            {
                 new CategoryTreeNode("Other", Bitwise.B16)
                 {
                     new ItemTreeNode("Move", HasDifference: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Transform", HasDifference: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("FreeTransform", HasDifference: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     ItemTreeNode.Empty,
                     new ItemTreeNode("DisplacementLiquefaction", HasDifference: true, HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("GradientMapping", HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("RippleEffect", HasPreview: true, ExistIcon: true, HasDifference: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Threshold", HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                     ItemTreeNode.Empty,
                     new ItemTreeNode("HSB", HasPreview: true, ExistIcon: true, IsItemClickEnabled: true),
                 },
                 new CategoryTreeNode("Adjustment", Bitwise.B16)
                 {
                     new ItemTreeNode("Gray", ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Invert", ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Exposure", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Brightness", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Saturation", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("HueRotation", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Contrast", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Temperature", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("HighlightsAndShadows", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                 },
                 new CategoryTreeNode("Adjustment2", Bitwise.B16)
                 {
                     new ItemTreeNode("GammaTransfer", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Vignette", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("ColorMatrix", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("ColorMatch", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                 },
                 new CategoryTreeNode("Effect1", Bitwise.B16)
                 {
                     new ItemTreeNode("GaussianBlur", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("DirectionalBlur", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Sharpen", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Shadow", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("EdgeDetection", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Morphology", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Emboss", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Straighten", HasPreview: true, ExistThumbnail: true, ExistIcon: true, IsItemClickEnabled: true),
                 },
                 new CategoryTreeNode("Effect2", Bitwise.B16)
                 {
                     new ItemTreeNode("Sepia", ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Posterize", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("LuminanceToAlpha", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("ChromaKey", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Border", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Colouring", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Tint", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("DiscreteTransfer", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                 },
                 new CategoryTreeNode("Effect3", Bitwise.B16)
                 {
                     new ItemTreeNode("Lighting", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Fog", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("Glass", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                     new ItemTreeNode("PinchPunch", HasPreview: true, ExistThumbnail: true, IsItemClickEnabled: true),
                 },
            },
            new RootTreeNode("Tool", Bitwise.B24)
            {
                new CategoryTreeNode("Marquee", Bitwise.B16)
                {
                    new ItemTreeNode("MarqueeRectangular", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MarqueeElliptical", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MarqueePolygon", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("MarqueeFreeHand", ExistIcon: true, IsItemClickEnabled: true),
                 },
                new CategoryTreeNode("Selection", Bitwise.B16)
                {
                    new ItemTreeNode("SelectionFlood", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("SelectionBrush", ExistIcon: true, IsItemClickEnabled: true),
                 },
                new CategoryTreeNode("Paint", Bitwise.B16)
                {
                    new ItemTreeNode("PaintBrush", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PaintLine", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PaintBrushForce", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PaintBrushMulti", ExistIcon: true, IsItemClickEnabled: true),
                 },
                new CategoryTreeNode("Vector", Bitwise.B16)
                {
                    new ItemTreeNode("Cursor", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("View", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Straw", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Fill", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Brush", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Transparency", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("Image", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Crop", ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Curve", Bitwise.B16)
                {
                    new ItemTreeNode("Node", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("Pen", ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Text", Bitwise.B16)
                {
                    new ItemTreeNode("TextArtistic", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("TextFrame", ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Geometry", Bitwise.B16)
                {
                    ItemTreeNode.Empty,
                    new ItemTreeNode("GeometryRectangle", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryEllipse", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("GeometryRoundRect", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryTriangle", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryDiamond", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("GeometryPentagon", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryStar", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryCog", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("GeometryDonut", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryPie", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryCookie", ExistIcon: true, IsItemClickEnabled: true),
                    ItemTreeNode.Empty,
                    new ItemTreeNode("GeometryArrow", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryCapsule", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("GeometryHeart", ExistIcon: true, IsItemClickEnabled: true),
                },
                new CategoryTreeNode("Pattern", Bitwise.B16)
                {
                    new ItemTreeNode("PatternGrid", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PatternDiagonal", ExistIcon: true, IsItemClickEnabled: true),
                    new ItemTreeNode("PatternSpotted", ExistIcon: true, IsItemClickEnabled: true),
                },
            },
        };

        public static List<CategoryTreeNode> TransformRoots = new List<CategoryTreeNode>
        {
            new CategoryTreeNode("Geometry")
            {
                new ItemTreeNode("GeometryRectangle"),
                new ItemTreeNode("GeometryEllipse"),
                ItemTreeNode.Empty,
                new ItemTreeNode("GeometryRoundRect"),
                new ItemTreeNode("GeometryTriangle"),
                new ItemTreeNode("GeometryDiamond"),
                ItemTreeNode.Empty,
                new ItemTreeNode("GeometryPentagon"),
                new ItemTreeNode("GeometryStar"),
                new ItemTreeNode("GeometryCog"),
                ItemTreeNode.Empty,
                new ItemTreeNode("GeometryDonut"),
                new ItemTreeNode("GeometryPie"),
                new ItemTreeNode("GeometryCookie"),
                ItemTreeNode.Empty,
                new ItemTreeNode("GeometryArrow"),
                new ItemTreeNode("GeometryCapsule"),
                new ItemTreeNode("GeometryHeart"),
            },
            CategoryTreeNode.Empty,
            new CategoryTreeNode("Pattern")
            {
                new ItemTreeNode("PatternGrid"),
                new ItemTreeNode("PatternDiagonal"),
                new ItemTreeNode("PatternSpotted"),
            }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("using System;");
            Console.WriteLine();

            Console.WriteLine("namespace Luo_Painter.Models");
            Console.WriteLine("{");
            Console.WriteLine("/// <summary>");
            Console.WriteLine("/// None: <para/>");
            Console.WriteLine("/// 0b_00000000_00000000_00000000_00000000 <para/>");
            Console.WriteLine("/// ");
            Console.WriteLine("/// Root: <para/>");
            Console.WriteLine("/// 0b_00011111_00000000_00000000_00000000 <para/>");
            Console.WriteLine("/// ");
            Console.WriteLine("/// Category: <para/>");
            Console.WriteLine("/// 0b_00000000_11111111_00000000_00000000 <para/>");
            Console.WriteLine("/// ");
            Console.WriteLine("/// Item: <para/>");
            Console.WriteLine("/// 0b_00000000_00000000_11111111_00000000 <para/>");
            Console.WriteLine("/// ");
            Console.WriteLine("/// Flag: <para/>");
            Console.WriteLine("/// 0b_00000000_00000000_00000000_11111111 <para/>");
            Console.WriteLine("/// </summary>");
            Console.WriteLine("[Flags]");
            Console.WriteLine("public enum OptionType");
            Console.WriteLine("{");
            Console.WriteLine("        // None");
            Console.WriteLine("        None = 0,");
            Console.WriteLine();

            Console.WriteLine("        // Flag");
            Console.WriteLine($"        {nameof(ItemTreeNode.IsItemClickEnabled)} = 1,");
            Console.WriteLine($"        {nameof(ItemTreeNode.ExistIcon)} = 2,");
            Console.WriteLine($"        {nameof(ItemTreeNode.ExistThumbnail)} = 4,");
            Console.WriteLine($"        {nameof(ItemTreeNode.HasMenu)} = 8,");
            Console.WriteLine($"        {nameof(ItemTreeNode.HasPreview)} = 16,");
            Console.WriteLine($"        {nameof(ItemTreeNode.HasDifference)} = 32,");
            Console.WriteLine($"        {nameof(ItemTreeNode.WithState)} = 64,");
            Console.WriteLine($"        {nameof(ItemTreeNode.WithTransform)} = 128,");
            Console.WriteLine();

            Console.WriteLine("        // Root");
            RootIndex = 1;
            foreach (RootTreeNode root in Roots)
            {
                Console.WriteLine($"{root.Title} = {GetBitwiseString(root)}{GetFlagString(root)},");
                RootIndex *= 2;
            }

            foreach (RootTreeNode root in Roots)
            {
                Console.WriteLine();
                Console.WriteLine($"        #region {root.Title}");
                Console.WriteLine();

                RootIndex = 1;
                CategoryIndex = 1;
                ItemIndex = 1;
                Console.WriteLine("        // Category");
                foreach (CategoryTreeNode category in root)
                {
                    if (category.IsEmpty)
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine($"        {category.Title} = {root.Title}{GetBitwiseString(category)}{GetFlagString(category)},");
                        CategoryIndex *= 2;
                    }
                }

                foreach (CategoryTreeNode category in root)
                {
                    if (category.IsEmpty)
                    {
                        //Console.WriteLine();
                    }
                    else
                    {
                        CategoryIndex = 1;
                        ItemIndex = 1;
                        Console.WriteLine();
                        Console.WriteLine($"        // {category.Title}");
                        foreach (ItemTreeNode item in category)
                        {
                            if (item.IsEmpty)
                            {
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine($"        {item.Title} = {category.Title}{GetBitwiseString(item)}{GetFlagString(item)},");
                                ItemIndex++;
                            }
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("        #endregion");
            }

            foreach (CategoryTreeNode category in TransformRoots)
            {
                if (category.IsEmpty)
                {
                    //Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"        // {category.Title}Transform");
                    foreach (ItemTreeNode item in category)
                    {
                        if (item.IsEmpty)
                        {
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine($"        {item.Title}Transform = {item.Title} | WithTransform | HasPreview,");
                        }
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("    }");
            Console.Write("}");

            Console.ReadKey();
            Console.ReadKey();
        }

        public static string GetBitwiseString(Node node)
        {
            switch (node.Bitwise)
            {
                case Bitwise.B24:
                    return $"{RootIndex} << 24";
                default:
                    string s = "";
                    if (node.Bitwise.HasFlag(Bitwise.B8)) s = $" | {ItemIndex} << 8" + s;
                    if (node.Bitwise.HasFlag(Bitwise.B16)) s = $" | {CategoryIndex} << 16" + s;
                    if (node.Bitwise.HasFlag(Bitwise.B24)) s = $" | {RootIndex} << 24" + s;
                    return s;
            }
        }

        public static string GetFlagString(Node node)
        {
            string s = "";
            if (node.IsItemClickEnabled) s = $"| {nameof(Node.IsItemClickEnabled)}" + s;
            if (node.ExistIcon) s = $"| {nameof(Node.ExistIcon)}" + s;
            if (node.ExistThumbnail) s = $"| {nameof(Node.ExistThumbnail)}" + s;
            if (node.HasMenu) s = $"| {nameof(Node.HasMenu)}" + s;
            if (node.HasPreview) s = $"| {nameof(Node.HasPreview)}" + s;
            if (node.HasDifference) s = $"| {nameof(Node.HasDifference)}" + s;
            if (node.WithState) s = $"| {nameof(Node.WithState)}" + s;
            if (node.WithTransform) s = $"| {nameof(Node.WithTransform)}" + s;
            return s;
        }
    }
}