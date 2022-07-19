using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class MainPage : Page
    {

        string Untitled = "Untitled";
        string New = "New Folder";

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        private void ConstructListView()
        {
            this.ListView.DragItemsStarting += (s, e) =>
            {
                foreach (Project item in e.Items)
                {
                    switch (item.Type)
                    {
                        case StorageItemTypes.None:
                            e.Cancel = true;
                            return;
                    }
                }
            };
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                if (this.ObservableCollection.FirstOrDefault() is Project item)
                {
                    switch (item.Type)
                    {
                        case StorageItemTypes.None:
                            return;
                        default:
                            if (this.ObservableCollection.FirstOrDefault() == ProjectNone.Add) break;

                            this.ObservableCollection.Remove(ProjectNone.Add);
                            this.ObservableCollection.Insert(0, ProjectNone.Add);
                            break;
                    }
                }
            };

            this.ListView.SelectionChanged += (s, e) =>
            {
                this.SelectedCount += e.AddedItems.Count;
                this.SelectedCount -= e.RemovedItems.Count;

                this.DupliateDocker.Count = this.SelectedCount;
                this.DeleteDocker.Count = this.SelectedCount;
                this.SelectDocker.Count = this.SelectedCount;
            };
            this.ListView.RightTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is Project item)
                    {
                        switch (item.Type)
                        {
                            case StorageItemTypes.File:
                                this.RenameItem.CommandParameter = item;
                                this.FileMenuFlyout.ShowAt(this.ListView, e.GetPosition(this.ListView));
                                break;
                            case StorageItemTypes.Folder:
                                this.RenameItem.CommandParameter = item;
                                this.FolderMenuFlyout.ShowAt(this.ListView, e.GetPosition(this.ListView));
                                break;
                            default:
                                break;
                        }
                    }
                }
            };

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Project item)
                {
                    switch (item.Type)
                    {
                        case StorageItemTypes.None: this.Action(ProjectAction.Add); break;
                        case StorageItemTypes.File: this.Action(ProjectAction.File, item); break;
                        case StorageItemTypes.Folder: this.Action(ProjectAction.Folder, item); break;
                        default: break;
                    }
                }
            };
        }

        private void ConstructDialog()
        {
            this.AppBarListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon item)
                {
                    switch (item.Symbol)
                    {
                        case Symbol.Add: this.Action(ProjectAction.Add); break;
                        case Symbol.Pictures: this.Action(ProjectAction.Image); break;
                        case Symbol.Copy: this.Action(ProjectAction.DupliateShow); break;
                        case Symbol.Delete: this.Action(ProjectAction.DeleteShow); break;
                        case Symbol.MoveToFolder: this.Action(ProjectAction.SelectShow); break;
                        case Symbol.NewFolder: this.Action(ProjectAction.New); break;
                        default: break;
                    }
                }
            };

            this.DupliateDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DupliateHide);
            this.DupliateDocker.PrimaryButtonClick += async (s, e) =>
            {
                await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), this.ListView.SelectedItems);
                this.Action(ProjectAction.DupliateHide);
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DeleteHide);
            this.DeleteDocker.PrimaryButtonClick += async (s, e) =>
            {
                await this.ObservableCollection.DeleteAsync(this.ListView.SelectedItems);
                this.Action(ProjectAction.DeleteHide);
            };

            this.SelectDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.SelectHide);
            this.SelectDocker.PrimaryButtonClick += (s, e) =>
            {
                this.ClipboardPath = this.Paths.GetPath();
                foreach (Project item in this.ListView.SelectedItems)
                {
                    switch (item.Type)
                    {
                        case StorageItemTypes.File:
                            this.ClipboardProjects.Add(item.Path);
                            break;
                    }
                }
                this.Action(ProjectAction.SelectToMove);
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.MoveHide);
            this.MoveDocker.PrimaryButtonClick += async (s, e) =>
            {
                string path = this.Paths.GetPath();
                if (this.ClipboardPath != path) 
                {
                    await this.ObservableCollection.Move(path, this.ClipboardProjects);
                }
                this.ClipboardProjects.Clear();
                this.Action(ProjectAction.MoveHide);
            };

            this.RenameCommand.Click += (s, e) => this.Action(ProjectAction.Rename, e);

            this.DupliateItem.Click += (s, e) => this.Action(ProjectAction.DupliateShow, this.RenameItem.CommandParameter as Project);
            this.DupliateItem2.Click += (s, e) => this.Action(ProjectAction.DupliateShow, this.RenameItem.CommandParameter as Project);

            this.DeleteItem.Click += (s, e) => this.Action(ProjectAction.DeleteShow, this.RenameItem.CommandParameter as Project);
            this.DeleteItem2.Click += (s, e) => this.Action(ProjectAction.DeleteShow, this.RenameItem.CommandParameter as Project);

            this.MoveItem.Click += (s, e) => this.Action(ProjectAction.SelectShow, this.RenameItem.CommandParameter as Project);
        
            this.LocalItem.Click += (s, e) => this.Action(ProjectAction.Local, this.RenameItem.CommandParameter as Project);
            this.LocalItem2.Click += (s, e) => this.Action(ProjectAction.Local, this.RenameItem.CommandParameter as Project);
            
            this.RenameDialog.IsPrimaryButtonEnabled = false;
            this.RenameTextBox.TextChanged += (s, e) => this.RenameDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.RenameTextBox.Text);
            this.NewTextBox.TextChanged += (s, e) => this.NewDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.NewTextBox.Text);
        }

    }
}