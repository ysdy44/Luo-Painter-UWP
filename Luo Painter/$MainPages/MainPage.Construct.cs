using Luo_Painter.Elements;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum ProjectAction
    {
        File,
        Folder,

        Add,
        Image,

        DupliateShow,
        DupliateHide,
        DeleteShow,
        DeleteHide,
        MoveShow,
        MoveHide,

        New,
        Rename,
    }

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
                this.MoveDocker.Count = this.SelectedCount;
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
                        case Symbol.MoveToFolder: this.Action(ProjectAction.MoveShow); break;
                        case Symbol.NewFolder: this.Action(ProjectAction.New); break;
                        default: break;
                    }
                }
            };

            this.DupliateDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DupliateHide);
            this.DupliateDocker.PrimaryButtonClick += async (s, e) =>
            {
                this.Action(ProjectAction.DupliateHide);
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DeleteHide);
            this.DeleteDocker.PrimaryButtonClick += async (s, e) =>
            {
                await this.ObservableCollection.DeleteAsync(this.ListView.SelectedItems);
                this.Action(ProjectAction.DeleteHide);
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.MoveHide);
            this.MoveDocker.PrimaryButtonClick += async (s, e) =>
            {
                await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), this.ListView.SelectedItems);
                this.Action(ProjectAction.MoveHide);
            };

            this.RenameCommand.Click += (s, e) => this.Action(ProjectAction.Rename, e);

            this.DupliateItem.Click += (s, e) => this.Action(ProjectAction.DupliateShow, this.RenameItem.CommandParameter as Project);
            this.DupliateItem2.Click += (s, e) => this.Action(ProjectAction.DupliateShow, this.RenameItem.CommandParameter as Project);

            this.DeleteItem.Click += (s, e) => this.Action(ProjectAction.DeleteShow, this.RenameItem.CommandParameter as Project);
            this.DeleteItem2.Click += (s, e) => this.Action(ProjectAction.DeleteShow, this.RenameItem.CommandParameter as Project);

            this.MoveItem.Click += (s, e) => this.Action(ProjectAction.MoveShow, this.RenameItem.CommandParameter as Project);
            this.MoveItem2.Click += (s, e) => this.Action(ProjectAction.MoveShow, this.RenameItem.CommandParameter as Project);

            this.RenameDialog.IsPrimaryButtonEnabled = false;
            this.RenameTextBox.TextChanged += (s, e) => this.RenameDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.RenameTextBox.Text);
            this.NewTextBox.TextChanged += (s, e) => this.NewDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.NewTextBox.Text);
        }

        private async void Action(ProjectAction action, Project item = null)
        {
            switch (action)
            {
                case ProjectAction.File:
                    if (item is null) break;
                    base.Frame.Navigate(typeof(DrawPage), item);
                    break;
                case ProjectAction.Folder:
                    if (item is null) break;
                    this.Paths.Add(new Metadata(item.Path, item.Name));

                    this.Load();
                    this.UpdateBack();
                    break;

                case ProjectAction.Add:
                    {
                        ContentDialogResult result = await this.AddDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                StorageFolder file = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                this.ObservableCollection.Insert(file);
                                base.Frame.Navigate(typeof(DrawPage), new ProjectNone(this.SizePicker.Size, file));
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Image:
                    break;

                case ProjectAction.DupliateShow:
                    this.ObservableCollection.Remove(ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.DupliateDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DupliateHide:
                    this.ObservableCollection.Insert(0, ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.DupliateDocker.IsShow = false;
                    break;
                case ProjectAction.DeleteShow:
                    this.ObservableCollection.Remove(ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.DeleteDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DeleteHide:
                    this.ObservableCollection.Insert(0, ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.DeleteDocker.IsShow = false;
                    break;
                case ProjectAction.MoveShow:
                    this.ObservableCollection.Remove(ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.MoveDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.MoveHide:
                    this.ObservableCollection.Insert(0, ProjectNone.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.MoveDocker.IsShow = false;
                    break;

                case ProjectAction.New:
                    {
                        this.NewTextBox.Text = this.New;
                        this.NewTextBox.SelectAll();
                        this.RenameTextBox.Focus(FocusState.Keyboard);

                        ContentDialogResult result = await this.NewDialog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                string name = this.NewTextBox.Text;
                                if (string.IsNullOrEmpty(name)) break;

                                await this.ObservableCollection.AddFolderAsync(this.Paths.GetPath(), name);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Rename:
                    if (item is ProjectFile item2)
                    {
                        this.RenameTextBox.Text = item2.DisplayName;
                        this.RenameTextBox.SelectAll();
                        this.RenameTextBox.Focus(FocusState.Keyboard);

                        ContentDialogResult result = await this.RenameDialog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                string name = this.RenameTextBox.Text;
                                if (string.IsNullOrEmpty(name)) break;

                                await item2.RenameAsync(name);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }
}