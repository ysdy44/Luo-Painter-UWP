using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.TestApp
{
    public sealed partial class TitelGrid : Grid
    {
        UIElement Root;
        double X;
        double Y;
        public TitelGrid()
        {
            base.Unloaded += (s, e) => this.Root = null;
            base.Loaded += (s, e) => this.Root = base.Parent as UIElement;
            base.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            base.ManipulationStarted += (s, e) =>
            {
                this.X = Canvas.GetLeft(this.Root);
                this.Y = Canvas.GetTop(this.Root);
            };
            base.ManipulationDelta += (s, e) =>
            {
                Canvas.SetLeft(this.Root, this.X + e.Cumulative.Translation.X);
                Canvas.SetTop(this.Root, this.Y + e.Cumulative.Translation.Y);
            };
        }
    }

    public sealed partial class ExpanderPage : Page
    {
        public ExpanderPage()
        {
            this.InitializeComponent();
        }
    }
}