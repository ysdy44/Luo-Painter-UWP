using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract class LayerBase
    {

        public string Id { get; } = Guid.NewGuid().ToString();


        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(Name)); // Notify 
            }
        }
        private string name;


        public BlendEffectMode? BlendMode
        {
            get => this.blendMode;
            set
            {
                this.blendMode = value;
                this.OnPropertyChanged(nameof(BlendMode)); // Notify 
            }
        }
        private BlendEffectMode? blendMode;


        public float Opacity
        {
            get => this.opacity;
            set
            {
                this.opacity = value;
                this.OnPropertyChanged(nameof(Opacity)); // Notify 
            }
        }
        private float opacity = 1;
        public float StartingOpacity { get; private set; }
        public void CacheOpacity() => this.StartingOpacity = this.Opacity;


        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.OnPropertyChanged(nameof(Visibility)); // Notify 
            }
        }
        private Visibility visibility;


        public IHistory GetBlendModeHistory(BlendEffectMode? mode)
        {
            BlendModeHistory blendMode = new BlendModeHistory
            {
                Id = this.Id,
                UndoParameter = this.BlendMode,
                RedoParameter = mode
            };

            this.BlendMode = mode;
            return blendMode;
        }
        public IHistory GetOpacityHistory() => new OpacityHistory
        {
            Id = this.Id,
            UndoParameter = this.StartingOpacity,
            RedoParameter = this.Opacity,
        };
        public IHistory GetVisibilityHistory()
        {
            switch (this.Visibility)
            {
                case Visibility.Visible:
                    this.Visibility = Visibility.Collapsed;
                    return new VisibilityHistory
                    {
                        Id = this.Id,
                        UndoParameter = Visibility.Visible,
                        RedoParameter = Visibility.Collapsed
                    };
                case Visibility.Collapsed:
                    this.Visibility = Visibility.Visible;
                    return new VisibilityHistory
                    {
                        Id = this.Id,
                        UndoParameter = Visibility.Collapsed,
                        RedoParameter = Visibility.Visible
                    };
                default:
                    return null;
            }
        }


        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}