using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System.Linq;
using Windows.ApplicationModel.Resources;
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
                if (this.Disabler) return;

                foreach (Project item in e.Items.Cast<Project>())
                {
                    switch (item.Type)
                    {
                        case ProjectType.Add:
                            e.Cancel = true;
                            return;
                    }
                }
            };
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                if (this.Disabler) return;

                if (this.ObservableCollection.FirstOrDefault() is Project item)
                {
                    switch (item.Type)
                    {
                        case ProjectType.Add:
                            return;
                        default:
                            if (this.ObservableCollection.FirstOrDefault() == ProjectAdd.Add) break;

                            this.ObservableCollection.Remove(ProjectAdd.Add);
                            this.ObservableCollection.Insert(0, ProjectAdd.Add);
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
                if (this.Disabler) return;

                if (this.DupliateDocker.IsShow) return;
                if (this.DeleteDocker.IsShow) return;
                if (this.SelectDocker.IsShow) return;

                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is Project item)
                    {
                        switch (item.Type)
                        {
                            case ProjectType.File:
                                this.RenameItem.CommandParameter = item;
                                this.FileMenuFlyout.ShowAt(this.ListView, e.GetPosition(this.ListView));
                                break;
                            case ProjectType.Folder:
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
                if (this.Disabler) return;

                if (e.ClickedItem is Project item)
                {
                    switch (item.Type)
                    {
                        case ProjectType.Add: this.Action(ProjectAction.Add); break;
                        case ProjectType.File: this.Action(ProjectAction.File, item); break;
                        case ProjectType.Folder: this.Action(ProjectAction.Folder, item); break;
                        default: break;
                    }
                }
            };
        }

        private void ConstructDialog()
        {
            this.AppBarListView.ItemClick += (s, e) =>
            {
                if (this.Disabler) return;

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
                if (this.Disabler) return;

                await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), this.ListView.SelectedItems);
                this.Action(ProjectAction.DupliateHide);
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.DeleteHide);
            this.DeleteDocker.PrimaryButtonClick += async (s, e) =>
            {
                if (this.Disabler) return;

                await this.ObservableCollection.DeleteAsync(this.ListView.SelectedItems);
                this.Action(ProjectAction.DeleteHide);
            };

            this.SelectDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.SelectHide);
            this.SelectDocker.PrimaryButtonClick += (s, e) =>
            {
                if (this.Disabler) return;

                this.ClipboardPath = this.Paths.GetPath();
                foreach (Project item in this.ListView.SelectedItems.Cast<Project>())
                {
                    switch (item.Type)
                    {
                        case ProjectType.File:
                            this.ClipboardProjects.Add(item.Path);
                            break;
                    }
                }
                this.Action(ProjectAction.SelectToMove);
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) => this.Action(ProjectAction.MoveHide);
            this.MoveDocker.PrimaryButtonClick += async (s, e) =>
            {
                if (this.Disabler) return;

                string path = this.Paths.GetPath();
                if (this.ClipboardPath != path)
                {
                    await this.ObservableCollection.Move(path, this.ClipboardProjects);
                }
                this.ClipboardProjects.Clear();
                this.Action(ProjectAction.MoveHide);
            };

            this.RenameCommand.Click += (s, e) => this.Action(ProjectAction.Rename, e);

            this.DupliateItem.Click += (s, e) => this.Action(ProjectAction.Dupliate, this.RenameItem.CommandParameter);

            this.DeleteItem.Click += (s, e) => this.Action(ProjectAction.Delete, this.RenameItem.CommandParameter);
            this.DeleteItem2.Click += (s, e) => this.Action(ProjectAction.Delete, this.RenameItem.CommandParameter);

            this.MoveItem.Click += (s, e) => this.Action(ProjectAction.Move, this.RenameItem.CommandParameter);

            this.LocalItem.Click += (s, e) => this.Action(ProjectAction.Local, this.RenameItem.CommandParameter);
            this.LocalItem2.Click += (s, e) => this.Action(ProjectAction.Local, this.RenameItem.CommandParameter);

            this.RenameDialog.IsPrimaryButtonEnabled = false;
            this.RenameTextBox.TextChanged += (s, e) => this.RenameDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.RenameTextBox.Text);
            this.NewTextBox.TextChanged += (s, e) => this.NewDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.NewTextBox.Text);
        }

    }
}