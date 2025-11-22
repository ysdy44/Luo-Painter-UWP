using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Luo_Painter.TestApp
{
    internal sealed class Kvp
    {
        public readonly string Key;
        public readonly string Text;
        public readonly Type PageType;

        public Kvp(string key, string text, Type value)
        {
            this.Key = key;
            this.Text = text;
            this.PageType = value;
        }
    }

    public sealed partial class MainPage : Page, ICommand
    {
        //@Instance
        private readonly Lazy<SystemNavigationManager> ManagerLazy = new Lazy<SystemNavigationManager>(() => SystemNavigationManager.GetForCurrentView());
        private readonly Lazy<ApplicationView> ViewLazy = new Lazy<ApplicationView>(() => ApplicationView.GetForCurrentView());
        private SystemNavigationManager Manager => this.ManagerLazy.Value;
        private ApplicationView View => this.ViewLazy.Value;

        readonly IDictionary<string, Kvp> DictionaryKey = (
            from item in Tree
            from kvp in item.Value
            where kvp != null
            select kvp).ToDictionary(c => c.Key);
        readonly IDictionary<string, Kvp> DictionaryText = (
            from item in Tree
            from kvp in item.Value
            where kvp != null
            select kvp).ToDictionary(c => c.Text);

        public MainPage()
        {
            this.InitializeComponent();
            foreach (KeyValuePair<string, Kvp[]> item in Tree)
            {
                MenuFlyoutSubItem subItem = new MenuFlyoutSubItem
                {
                    Text = item.Key,
                };

                foreach (Kvp type in item.Value)
                {
                    if (type == null)
                        subItem.Items.Add(new MenuFlyoutSeparator());
                    else
                        subItem.Items.Add(new MenuFlyoutItem
                        {
                            Text = type.Text,
                            CommandParameter = type.Key,
                            Command = this,
                        });
                }

                this.MenuFlyout.Items.Add(subItem);
            }

            // Register a handler for BackRequested events
            base.Unloaded += delegate { this.Manager.BackRequested -= this.OnBackRequested; };
            base.Loaded += delegate
            {
                this.Manager.BackRequested += this.OnBackRequested;
                this.AutoSuggestBox.Focus(FocusState.Keyboard);
            };

            this.Hyperlink0.Inlines.Add(new Run { Text = nameof(PenCurve2Page) });
            this.Hyperlink0.Click += delegate { this.NavigateKey("PenCurve2"); };

            this.Hyperlink1.Inlines.Add(new Run { Text = nameof(PenCurvePage) });
            this.Hyperlink1.Click += delegate { this.NavigateKey("PenCurve"); };

            this.Hyperlink2.Inlines.Add(new Run { Text = nameof(TextLayoutsPage) });
            this.Hyperlink2.Click += delegate { this.NavigateKey("TextLayouts"); };

            this.Hyperlink3.Inlines.Add(new Run { Text = nameof(LayerManagerPage) });
            this.Hyperlink3.Click += delegate { this.NavigateKey("LayerManager"); };

            this.Hyperlink4.Inlines.Add(new Run { Text = nameof(DisplacementLiquefactionPage) });
            this.Hyperlink4.Click += delegate { this.NavigateKey("DisplacementLiquefaction"); };

            this.ListView.ItemsSource = this.Overlay.Children.Select(c => ((FrameworkElement)c).Tag).ToArray();
            this.ListView.SelectionChanged += delegate
            {
                int index = this.ListView.SelectedIndex;

                for (int i = 0; i < this.Overlay.Children.Count; i++)
                {
                    UIElement item = this.Overlay.Children[i];

                    item.Visibility = index == i ? Visibility.Visible : Visibility.Collapsed;
                }
                this.Overlay.Visibility = index < 0 ? Visibility.Collapsed : Visibility.Visible;
            };

            this.Overlay.Tapped += delegate { this.ListView.SelectedIndex = -1; };
            this.AutoSuggestBox.SuggestionChosen += (s, e) => this.NavigateText($"{e.SelectedItem}");
            this.AutoSuggestBox.TextChanged += (sender, args) =>
            {
                switch (args.Reason)
                {
                    case AutoSuggestionBoxTextChangeReason.ProgrammaticChange:
                    case AutoSuggestionBoxTextChangeReason.SuggestionChosen:
                        break;
                    default:
                        if (string.IsNullOrEmpty(sender.Text))
                        {
                            sender.ItemsSource = null;
                            this.View.Title = "Pages";
                        }
                        else
                        {
                            string text = sender.Text.ToLower();
                            IEnumerable<string> suitableItems = this.DictionaryText.Keys.Where(x => x.ToLower().Contains(text));

                            int count = suitableItems.Count();
                            if (count is 0)
                            {
                                sender.ItemsSource = null;
                                this.View.Title = "No results found";
                            }
                            else
                            {
                                sender.ItemsSource = suitableItems;
                                this.View.Title = $"{count} results";
                            }
                        }
                        break;
                }
            };
        }

        // Command
        public ICommand Command => this;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => this.NavigateKey($"{parameter}");

        private void NavigateKey(string key)
        {
            if (this.DictionaryKey.ContainsKey(key))
            {
                Kvp item = this.DictionaryKey[key];

                this.ListView.SelectedIndex = -1;
                this.ContentFrame.Navigate(item.PageType);

                this.Manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                this.View.Title = item.Text;
            }
        }
        private void NavigateText(string text)
        {
            if (this.DictionaryText.ContainsKey(text))
            {
                Kvp item = this.DictionaryText[text];

                this.ListView.SelectedIndex = -1;
                this.ContentFrame.Navigate(item.PageType);

                this.Manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                this.View.Title = item.Text;
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            if (this.ContentFrame.CanGoBack)
            {
                this.ListView.SelectedIndex = -1;
                this.ContentFrame.GoBack();

                this.Manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                this.View.Title = this.ContentFrame.Content.GetType().Name;
            }
            else
            {
                this.ListView.SelectedIndex = -1;
                this.ContentFrame.Content = this.ContentPage;

                this.Manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                this.View.Title = string.Empty;
            }
        }

        static readonly IDictionary<string, Kvp[]> Tree = new Dictionary<string, Kvp[]>
        {
            ["Color"] = new Kvp[]
            {
                new Kvp("ColorPicker", "Color Picker", typeof(ColorPickerPage)),
                new Kvp("ColorValuePicker", "Color Value Picker", typeof(ColorValuePickerPage)),
                new Kvp("ColorHarmonyPicker", "Color Harmony Picker", typeof(ColorHarmonyPickerPage)),
                default,
                new Kvp("AlignmentGrid", "Alignment Grid", typeof(AlignmentGridPage)),
                new Kvp("NightSkyTimeLapse", "Night Sky Time Lapse", typeof(NightSkyTimeLapsePage)),
                new Kvp("SpottedPattern", "Spotted Pattern", typeof(SpottedPatternPage)),
                new Kvp("SolarSystem", "Solar System", typeof(SolarSystemPage)),
            },
            ["Input"] = new Kvp[]
            {
                new Kvp("TextLayouts", "Text Layouts", typeof(TextLayoutsPage)),
                new Kvp("CustomEditor", "Custom Editor", typeof(CustomEditorPage)),
                default,
                new Kvp("Eyedropper", "Eyedropper", typeof(EyedropperPage)),
                new Kvp("Number", "Number", typeof(NumberPage)),
                new Kvp("SliderToolTip", "Slider Tool Tip", typeof(SliderToolTipPage)),
            },
            ["Transform"] = new Kvp[]
            {
                new Kvp("Transform", "Transform", typeof(TransformPage)),
                new Kvp("FreeTransform", "Free Transform", typeof(FreeTransformPage)),
                new Kvp("MarqueeTool", "Marquee Tool", typeof(MarqueeToolPage)),
                default,
                new Kvp("CanvasTransformer", "Canvas Transformer", typeof(CanvasTransformerPage)),
                new Kvp("CanvasTransformer2", "Canvas Transformer 2", typeof(CanvasTransformer2Page)),
                new Kvp("CanvasTransformer3", "Canvas Transformer 3", typeof(CanvasTransformer3Page)),
                new Kvp("MarqueeToolTransform", "Marquee Tool Transform", typeof(MarqueeToolTransformPage)),
            },
            ["Control"] = new Kvp[]
            {
                new Kvp("Breadcrumb", "Breadcrumb", typeof(BreadcrumbPage)),
                new Kvp("Marble", "Marble", typeof(MarblePage)),
                new Kvp("Scroller", "Scroller", typeof(ScrollerPage)),
                default,
                new Kvp("Expander", "Expander", typeof(ExpanderPage)),
                new Kvp("ShyHeader", "Shy Header", typeof(ShyHeaderPage)),
                new Kvp("SwitchPresenter", "Switch Presenter", typeof(SwitchPresenterPage)),
            },
            ["Pen"] = new Kvp[]
            {
                new Kvp("PenBSplines", "Pen B Splines", typeof(PenBSplinesPage)),
                new Kvp("PenCurve2", "Pen Curve 2", typeof(PenCurve2Page)),
                new Kvp("PenCurve", "Pen Curve", typeof(PenCurvePage)),
                new Kvp("PenStabilizer", "Pen Stabilizer", typeof(PenStabilizerPage)),
                default,
                new Kvp("PenCutter", "Pen Cutter", typeof(PenCutterPage)),
                new Kvp("PenHitter", "Pen Hitter", typeof(PenHitterPage)),
                new Kvp("Pen", "Pen", typeof(PenPage)),
                new Kvp("PenPreview", "Pen Preview", typeof(PenPreviewPage)),
            },
            ["Ink"] = new Kvp[]
            {
                new Kvp("InkBrownian", "Ink Brownian", typeof(InkBrownianPage)),
                new Kvp("InkForce", "Ink Force", typeof(InkForcePage)),
                new Kvp("InkInertia", "Ink Inertia", typeof(InkInertiaPage)),
                new Kvp("InkStabilizer", "Ink Stabilizer", typeof(InkStabilizerPage)),
                default,
                new Kvp("InkTouchMode", "Ink Touch Mode", typeof(InkTouchModePage)),
                new Kvp("InkMixer", "Ink Mixer", typeof(InkMixerPage)),
                new Kvp("InverseProportion", "Inverse Proportion", typeof(InverseProportionPage)),
            },
            ["Brush"] = new Kvp[]
            {
                new Kvp("BitmapPixelBrush", "Bitmap Pixel Brush", typeof(BitmapPixelBrushPage)),
                new Kvp("BrushShader", "Brush Shader", typeof(BrushShaderPage)),
                new Kvp("BrushEasePressure", "Brush Ease Pressure", typeof(BrushEasePressurePage)),
                new Kvp("BrushEdgeHardness", "Brush Edge Hardness", typeof(BrushEdgeHardnessPage)),
                default,
                new Kvp("BitmapRegion", "Bitmap Region", typeof(BitmapRegionPage)),
                new Kvp("RegionsInvalidated", "Regions Invalidated", typeof(RegionsInvalidatedPage)),
                new Kvp("PixelBounds", "Pixel Bounds", typeof(PixelBoundsPage)),
            },
            ["Effect"] = new Kvp[]
            {
                new Kvp("ArithmeticCompositeEffect", "Arithmetic Composite Effect", typeof(ArithmeticCompositeEffectPage)),
                new Kvp("AtlasEffect", "Atlas Effect", typeof(AtlasEffectPage)),
                new Kvp("GradientMapping", "Gradient Mapping", typeof(GradientMappingPage)),
                new Kvp("ShaderTest", "Shader Test", typeof(ShaderTestPage)),
                default,
                new Kvp("FloodSelect", "Flood Select", typeof(FloodSelectPage)),
                new Kvp("RippleEffect", "Ripple Effect", typeof(RippleEffectPage)),
                new Kvp("Threshold", "Threshold", typeof(ThresholdPage)),
                new Kvp("MagicWand", "Magic Wand", typeof(MagicWandPage)),
            },
            ["Displacement"] = new Kvp[]
            {
                new Kvp("LiquefactionShader", "Liquefaction Shader", typeof(LiquefactionShaderPage)),
                new Kvp("DisplacementLiquefaction", "Displacement Liquefaction", typeof(DisplacementLiquefactionPage)),
                new Kvp("DisplacementMap", "Displacement Map", typeof(DisplacementMapPage)),
                new Kvp("DirectionDistance", "Direction Distance", typeof(DirectionDistancePage)),
                default,
                new Kvp("LightingDistant", "Lighting Distant", typeof(LightingDistantPage)),
                new Kvp("LightingPoint", "Lighting Point", typeof(LightingPointPage)),
                new Kvp("LightingSpot", "Lighting Spot", typeof(LightingSpotPage)),
            },
            ["Layer"] = new Kvp[]
            {
                new Kvp("LayerManager", "Layer Manager", typeof(LayerManagerPage)),
                new Kvp("Layer", "Layer", typeof(LayerPage)),
                new Kvp("Historian", "Historian", typeof(HistorianPage)),
                default,
                new Kvp("SelectedButton", "Selected Button", typeof(SelectedButtonPage)),
                new Kvp("SelectedSwiper", "Selected Swiper", typeof(SelectedSwiperPage)),
            },
            ["System"] = new Kvp[]
            {
                new Kvp("Battery", "Battery", typeof(BatteryPage)),
                new Kvp("Toast", "Toast", typeof(ToastPage)),
                new Kvp("CanvasManipulations", "Canvas Manipulations", typeof(CanvasManipulationsPage)),
                new Kvp("PointerWheelChanged", "Pointer Wheel Changed", typeof(PointerWheelChangedPage)),
                default,
                new Kvp("ListView", "List View", typeof(ListViewPage)),
                new Kvp("FileImageSource", "File Image Source", typeof(FileImageSourcePage)),
                new Kvp("WriteableBitmapSource", "Writeable Bitmap Source", typeof(WriteableBitmapSourcePage)),
            }
        };
    }
}