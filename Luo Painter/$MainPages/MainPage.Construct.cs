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
                            this.ObservableCollection.Remove(Project.Add);
                            this.ObservableCollection.Insert(0, Project.Add);
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
                                break;
                            case StorageItemTypes.Folder:
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
            this.DupliateDocker.PrimaryButtonClick += (s, e) => this.Action(ProjectAction.DupliateHide);

            this.DeleteDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DeleteHide);
            this.DeleteDocker.PrimaryButtonClick += (s, e) => this.Action(ProjectAction.DeleteHide);

            this.MoveDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.MoveHide);
            this.MoveDocker.PrimaryButtonClick += (s, e) => this.Action(ProjectAction.MoveHide);

            this.RenameCommand.Click += (s, e) => this.Action(ProjectAction.Rename, e);
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
                    break;

                case ProjectAction.Add:
                    {
                        ContentDialogResult result = await this.AddDislog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                base.Frame.Navigate(typeof(DrawPage), this.SizePicker.Size);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Image:
                    break;

                case ProjectAction.DupliateShow:
                    this.ObservableCollection.Remove(Project.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.DupliateDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DupliateHide:
                    this.ObservableCollection.Insert(0, Project.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.DupliateDocker.IsShow = false;
                    break;
                case ProjectAction.DeleteShow:
                    this.ObservableCollection.Remove(Project.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.DeleteDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DeleteHide:
                    this.ObservableCollection.Insert(0, Project.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.DeleteDocker.IsShow = false;
                    break;
                case ProjectAction.MoveShow:
                    this.ObservableCollection.Remove(Project.Add);
                    this.ListView.IsItemClickEnabled = false;
                    this.MoveDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.MoveHide:
                    this.ObservableCollection.Insert(0, Project.Add);
                    this.ListView.IsItemClickEnabled = true;
                    this.MoveDocker.IsShow = false;
                    break;

                case ProjectAction.New:
                    {
                        this.NewTextBox.Text = this.New;
                        this.NewTextBox.SelectAll();

                        ContentDialogResult result = await this.NewDislog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Rename:
                    {
                        if (item is null) break;
                        this.RenameTextBox.Text = item.DisplayName;
                        this.RenameTextBox.SelectAll();

                        ContentDialogResult result = await this.RenameDislog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                string name = this.RenameTextBox.Text;
                                if (string.IsNullOrEmpty(name))
                                {
                                    break;
                                }

                                item.Rename(name);
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