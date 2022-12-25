using System.ComponentModel;

namespace Luo_Painter.HSVColorPickers
{    
    public class CurveStop : INotifyPropertyChanged
    {
        public float Offset
        {
            get => this.offset;
            set
            {
                this.offset = value;
                this.OnPropertyChanged(nameof(Offset)); // Notify 
            }
        }
        private float offset;

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