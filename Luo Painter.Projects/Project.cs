using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace Luo_Painter.Projects
{
    public abstract class Project : INotifyPropertyChanged
    {
        public StorageItemTypes Type { get; }

        public bool isEnabled = true;
        public bool IsEnabled
        {
            get => this.isEnabled;
            set
            {
                this.isEnabled = value;
                this.OnPropertyChanged(nameof(IsEnabled)); // Notify 
            }
        }

        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public DateTimeOffset DateCreated { get; protected set; }

        public string Thumbnail { get; protected set; }

        internal Project(StorageItemTypes type) => this.Type = type;
        protected Project(StorageItemTypes type, StorageFolder item) : this(type)
        {
            this.Path = item.Path;
            this.Name = item.Name;
            this.DisplayName = item.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = item.DateCreated;
        }

        protected void Rename(StorageFolder zipFolder)
        {
            this.Path = zipFolder.Path;
            this.Name = zipFolder.Name;
            this.DisplayName = zipFolder.DisplayName.Replace(".luo", string.Empty);
            this.DateCreated = zipFolder.DateCreated;
        }

        public async Task DeleteAsync()
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(this.Path);
            await item.DeleteAsync();
        }

        public void Enable() => this.IsEnabled = true;
        public void Enable(StorageItemTypes types)
        {
            switch (this.Type)
            {
                case StorageItemTypes.None:
                    this.IsEnabled = types is StorageItemTypes.None;
                    break;
                case StorageItemTypes.File:
                    this.IsEnabled = types.HasFlag(StorageItemTypes.File);
                    break;
                case StorageItemTypes.Folder:
                    this.IsEnabled = types.HasFlag(StorageItemTypes.Folder);
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