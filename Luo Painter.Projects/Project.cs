using System;
using System.ComponentModel;
using Windows.Storage;

namespace Luo_Painter.Projects
{
    public abstract partial class Project : INotifyPropertyChanged
    {
        //@Content
        public StorageItemTypes Type { get; }
        public bool IsEnabled { get; private set; } = true;
        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public DateTimeOffset DateCreated { get; protected set; }
   
        public string Thumbnail { get; protected set; }

        //@Construct
        protected Project(StorageItemTypes type) => this.Type = type;

        public void Enable() => this.IsEnabled = true;
        public void Enable(StorageItemTypes types)
        {
            switch (this.Type)
            {
                case StorageItemTypes.None:
                    this.IsEnabled = types is StorageItemTypes.None;
                    this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
                    break;
                case StorageItemTypes.File:
                    this.IsEnabled = types.HasFlag(StorageItemTypes.File);
                    this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
                    break;
                case StorageItemTypes.Folder:
                    this.IsEnabled = types.HasFlag(StorageItemTypes.Folder);
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