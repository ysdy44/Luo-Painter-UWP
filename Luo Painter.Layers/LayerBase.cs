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
            get => this.title;
            set
            {
                this.title = value;
                this.OnPropertyChanged(nameof(Name)); // Notify 
            }
        }
        private string title;

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