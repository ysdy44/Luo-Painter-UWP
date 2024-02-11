using System;
using System.ComponentModel;

namespace Luo_Painter.Models
{
    public sealed class ProjectNew : ProjectBase, INotifyPropertyChanged
    {
        //@Static
        public static readonly ProjectNew New = new ProjectNew();

        //@Content
        public ProjectType Type => ProjectType.New;
        public bool IsEnabled { get; private set; } = true;
        public string Path => null;
        public string Name => null;
        public string DisplayName => null;
        //@Debug
        // Order by DateCreated
        public DateTimeOffset DateCreated => DateTimeOffset.MaxValue;

        public string Thumbnail => null;

        public void Enable(ProjectType types)
        {
            bool isEnabled = types.HasFlag(ProjectType.New);
            if (this.IsEnabled == isEnabled) return;

            this.IsEnabled = isEnabled;
            this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
        }

        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}