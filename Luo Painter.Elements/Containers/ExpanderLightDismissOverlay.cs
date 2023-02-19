using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// LightDismissOverlay for <see cref="Expander"/>.
    /// </summary>
    public sealed class ExpanderLightDismissOverlay : Canvas
    {

        //@Delegate
        public event TypedEventHandler<ExpanderLightDismissOverlay, bool> IsFlyoutChanged;

        readonly Stack<Expander> Items = new Stack<Expander>();

        //@Construct     
        /// <summary>
        /// Initializes a ExpanderLightDismissOverlay. 
        /// </summary>
        public ExpanderLightDismissOverlay()
        {
            base.Unloaded += (s, e) =>
            {
                foreach (Expander item in this.Items)
                {
                    item.IsFlyoutChanged -= this.OnIsFlyoutChanged;
                    item.IsShowChanged -= this.IsShowChanged;
                    item.OnZIndexChanging -= this.OnZIndexChanging;

                    base.SizeChanged -= item.CanvasSizeChanged;
                }
                this.Items.Clear();
            };
            base.Loaded += (s, e) =>
            {
                foreach (UIElement item in base.Children)
                {
                    if (item is Expander expander)
                    {
                        this.Items.Push(expander);
                        expander.IsFlyoutChanged += this.OnIsFlyoutChanged;
                        expander.IsShowChanged += this.IsShowChanged;
                        expander.OnZIndexChanging += this.OnZIndexChanging;

                        expander.CanvasSizeChanged(base.ActualWidth, base.ActualHeight);
                        base.SizeChanged += expander.CanvasSizeChanged;
                    }
                }
            };
        }

        /// <summary>
        /// Hide all flyout.
        /// </summary>
        public bool Hide()
        {
            bool result = false;
            foreach (Expander item in this.Items)
            {
                switch (item.State)
                {
                    case ExpanderState.Flyout:
                        result = true;
                        item.Hide();
                        break;
                }
            }
            return result;
        }

        private void OnIsFlyoutChanged(Expander sender, bool isFlyout)
        {
            if (isFlyout)
            {
                this.IsFlyoutChanged?.Invoke(this, true); // Delegate
                foreach (Expander item in this.Items)
                {
                    item.IsHitTestVisible = false;
                }
                sender.IsHitTestVisible = true;
            }
            else
            {
                this.IsFlyoutChanged?.Invoke(this, false); // Delegate
                foreach (Expander item in this.Items)
                {
                    item.IsHitTestVisible = true;
                }
                Canvas.SetZIndex(sender, 0);
            }
        }

        private void IsShowChanged(Expander sender, bool isFlyout)
        {
        }

        private void OnZIndexChanging(Expander sender, int args)
        {
            int top = base.Children.Count - 1;
            if (args == top) return;

            Canvas.SetZIndex(sender, top);

            foreach (Expander item in this.Items)
            {
                switch (item.State)
                {
                    case ExpanderState.Flyout:
                        continue;
                    case ExpanderState.Overlay:
                        if (item == sender) continue;

                        int index = Canvas.GetZIndex(item);
                        if (index is 0) continue;

                        Canvas.SetZIndex(item, index - 1);
                        break;
                }
            }
        }
    }
}