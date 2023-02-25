using System;
using System.ComponentModel;

namespace Luo_Painter.Models
{
    public abstract partial class ProjectBase : INotifyPropertyChanged
    {
        //@Content
        public ProjectType Type { get; }
        public bool IsEnabled { get; private set; } = true;
        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public DateTimeOffset DateCreated { get; protected set; }

        public string Thumbnail { get; protected set; }

        //@Construct
        protected ProjectBase(ProjectType type) => this.Type = type;

        public void Enable() => this.IsEnabled = true;
        public void Enable(ProjectType types)
        {
            switch (this.Type)
            {
                case ProjectType.New:
                    this.IsEnabled = types.HasFlag(ProjectType.New);
                    this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
                    break;
                case ProjectType.Project:
                    this.IsEnabled = types.HasFlag(ProjectType.Project);
                    this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
                    break;
                case ProjectType.Folder:
                    this.IsEnabled = types.HasFlag(ProjectType.Folder);
                    this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
                    break;
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