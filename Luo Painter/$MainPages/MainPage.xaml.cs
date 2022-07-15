using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter
{
    internal class ProjectCommand : RelayCommand<Project> { }

    internal class ProjectDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate None { get; set; }
        public DataTemplate File { get; set; }
        public DataTemplate Folder { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Project item2)
            {
                switch (item2.Type)
                {
                    case StorageItemTypes.None: return this.None;
                    case StorageItemTypes.File: return this.File;
                    case StorageItemTypes.Folder: return this.Folder;
                    default: return null;
                }
            }
            else return null;
        }
    }

    internal sealed class ProjectIcon : TIcon<Symbol>
    {
        public ProjectIcon()
        {
            base.Loaded += (s, e) =>
            {
                ListViewItem parent = this.FindAncestor<ListViewItem>();
                if (parent is null) return;
                ToolTipService.SetToolTip(parent, new ToolTip
                {
                    Content = this.Type.ToString(),
                    Placement = PlacementMode.Left,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            };
        }
        protected override void OnTypeChanged(Symbol value)
        {
            base.Content = new SymbolIcon(value);
        }
    }

    public class Project : INotifyPropertyChanged
    {

        //@Static
        public static readonly Project Add = new Project();

        public StorageItemTypes Type { get; set; }

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

        public string Thumbnail
        {
            get => this.thumbnail;
            set
            {
                this.thumbnail = value;
                this.OnPropertyChanged(nameof(Thumbnail)); // Notify 
            }
        }
        private string thumbnail;

        public string ThumbnailLeft
        {
            get => this.thumbnailLeft;
            set
            {
                this.thumbnailLeft = value;
                this.OnPropertyChanged(nameof(ThumbnailLeft)); // Notify 
            }
        }
        private string thumbnailLeft;

        public string ThumbnailRight
        {
            get => this.thumbnailRight;
            set
            {
                this.thumbnailRight = value;
                this.OnPropertyChanged(nameof(ThumbnailRight)); // Notify 
            }
        }
        private string thumbnailRight;

        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed class ProjectObservableCollection : ObservableCollection<Project>
    {
    }

    public sealed partial class MainPage : Page
    {
        //@Converter
        private ListViewSelectionMode BooleanToSelectionModeConverter(bool value) => value ? ListViewSelectionMode.None : ListViewSelectionMode.Multiple;
        private ListViewReorderMode BooleanToReorderModeConverter(bool value) => value ? ListViewReorderMode.Enabled : ListViewReorderMode.Disabled;

        public int SelectedCount { get; private set; }
        public ProjectObservableCollection ObservableCollection { get; } = new ProjectObservableCollection
        {
            Project.Add
        };

        public MainPage()
        {
            this.InitializeComponent();
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);
            };

            this.ListView.SelectionChanged += (s, e) =>
            {
                this.SelectedCount += e.AddedItems.Count;
                this.SelectedCount -= e.RemovedItems.Count;
            };
            this.ListView.ItemClick += async (s, e) =>
            {
                if (e.ClickedItem is Project item)
                {
                    switch (item.Type)
                    {
                        case StorageItemTypes.File:
                            base.Frame.Navigate(typeof(DrawPage));
                            break;
                        case StorageItemTypes.Folder:
                            break;
                        case StorageItemTypes.None:
                            ContentDialogResult result = await this.AddDislog.ShowInstance();

                            switch (result)
                            {
                                case ContentDialogResult.Primary:
                                    base.Frame.Navigate(typeof(DrawPage));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            };

            this.AppBarListView.ItemClick += async (s, e) =>
            {
                if (e.ClickedItem is Symbol item)
                {
                    switch (item)
                    {
                        case Symbol.Add:
                            ContentDialogResult result1 = await this.AddDislog.ShowInstance();

                            switch (result1)
                            {
                                case ContentDialogResult.Primary:
                                    base.Frame.Navigate(typeof(DrawPage));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case Symbol.Pictures:
                            break;

                        case Symbol.Copy:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.DupliateDocker.IsShow = true;
                            break;
                        case Symbol.Delete:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.DeleteDocker.IsShow = true;
                            break;

                        case Symbol.MoveToFolder:
                            this.ObservableCollection.Remove(Project.Add);
                            this.ListView.IsItemClickEnabled = false;
                            this.MoveDocker.IsShow = true;
                            break;
                        case Symbol.NewFolder:
                            this.NewTextBox.Text = "New Folder";
                            this.NewTextBox.SelectAll();

                            ContentDialogResult result2 = await this.NewDislog.ShowInstance();
                            break;
                        default:
                            break;
                    }
                }
            };

            this.DupliateDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DupliateDocker.IsShow = false;
            };
            this.DupliateDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DupliateDocker.IsShow = false;
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DeleteDocker.IsShow = false;
            };
            this.DeleteDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.DeleteDocker.IsShow = false;
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.MoveDocker.IsShow = false;
            };
            this.MoveDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ObservableCollection.Insert(0, Project.Add);
                this.ListView.IsItemClickEnabled = true;
                this.MoveDocker.IsShow = false;
            };

            this.RenameCommand.Click += async (s, e) =>
            {
                this.RenameTextBox.Text = e.Name;
                this.RenameTextBox.SelectAll();

                ContentDialogResult result = await this.RenameDislog.ShowInstance();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                        e.Name = this.RenameTextBox.Text;
                        break;
                    default:
                        break;
                }
            };
        }
    }
}