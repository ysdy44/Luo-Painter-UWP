using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml;

namespace Luo_Painter.UI
{
    public sealed class MorphologyNumberSlider : NumberSlider
    {
        //@Delegate
        public event RoutedEventHandler Toggled;
        //@Content
        public int Size { get; private set; } = 1;
        public bool IsEmpty { get; private set; } = true;
        public MorphologyEffectMode Mode { get; private set; } = MorphologyEffectMode.Dilate;
        //@Construct
        public MorphologyNumberSlider()
        {
            base.Value = 1;
            base.Minimum = -90;
            base.Maximum = 90;
            base.ValueChanged += (s, e) =>
            {
                int size = (int)e.NewValue;
                this.IsEmpty = size == 0;
                this.Mode = size < 0 ? MorphologyEffectMode.Erode : MorphologyEffectMode.Dilate;
                this.Size = Math.Clamp(Math.Abs(size), 1, 90);
                this.Toggled?.Invoke(s, e); //Delegate
                if (this.HeaderButton is null) return;
                this.HeaderButton.Content = size;
            };
        }
    }
}