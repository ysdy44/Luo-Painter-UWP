﻿using Luo_Painter.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class MainPage : Page
    {

        private void ConstructListView()
        {
            this.ListView.DragItemsStarting += (s, e) =>
            {
                if (this.Disabler) return;

                foreach (ProjectBase item in e.Items.Cast<ProjectBase>())
                {
                    switch (item.Type)
                    {
                        case ProjectType.New:
                            e.Cancel = true;
                            return;
                    }
                }
            };
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                if (this.Disabler) return;

                if (this.ObservableCollection.FirstOrDefault() is ProjectBase item)
                {
                    switch (item.Type)
                    {
                        case ProjectType.New:
                            return;
                        default:
                            if (this.ObservableCollection.FirstOrDefault() == ProjectNew.New) break;

                            this.ObservableCollection.Remove(ProjectNew.New);
                            this.ObservableCollection.Insert(0, ProjectNew.New);
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
                    if (element.DataContext is ProjectBase item)
                    {
                        switch (item.Type)
                        {
                            case ProjectType.Project:
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

                if (e.ClickedItem is ProjectBase item)
                {
                    switch (item.Type)
                    {
                        case ProjectType.New: this.Click(ActionType.NewProject); break;
                        case ProjectType.Project: this.Click(ActionType.OpenProject, item); break;
                        case ProjectType.Folder: this.Click(ActionType.OpenFolder, item); break;
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
                        case Symbol.Add: this.Click(ActionType.NewProject); break;
                        case Symbol.Pictures: this.Click(ActionType.NewImage); break;
                        case Symbol.NewFolder: this.Click(ActionType.NewFolder); break;
                        case Symbol.Copy: this.Click(ActionType.DupliateShow); break;
                        case Symbol.Delete: this.Click(ActionType.DeleteShow); break;
                        case Symbol.MoveToFolder: this.Click(ActionType.SelectShow); break;
                        default: break;
                    }
                }
            };

            this.DupliateDocker.SecondaryButtonClick += (s, e) => this.Click(ActionType.DupliateHide);
            this.DupliateDocker.PrimaryButtonClick += async (s, e) =>
            {
                if (this.Disabler) return;

                await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), this.ListView.SelectedItems);
                this.Click(ActionType.DupliateHide);
            };

            this.DeleteDocker.SecondaryButtonClick += (s, e) => this.Click(ActionType.DeleteHide);
            this.DeleteDocker.PrimaryButtonClick += async (s, e) =>
            {
                if (this.Disabler) return;

                await this.ObservableCollection.DeleteAsync(this.ListView.SelectedItems);
                this.Click(ActionType.DeleteHide);
            };

            this.SelectDocker.SecondaryButtonClick += (s, e) => this.Click(ActionType.SelectHide);
            this.SelectDocker.PrimaryButtonClick += (s, e) =>
            {
                if (this.Disabler) return;

                this.ClipboardPath = this.Paths.GetPath();
                foreach (ProjectBase item in this.ListView.SelectedItems.Cast<ProjectBase>())
                {
                    switch (item.Type)
                    {
                        case ProjectType.Project:
                            this.ClipboardProjects.Add(item.Path);
                            break;
                    }
                }
                this.Click(ActionType.SelectToMove);
            };

            this.MoveDocker.SecondaryButtonClick += (s, e) => this.Click(ActionType.MoveHide);
            this.MoveDocker.PrimaryButtonClick += async (s, e) =>
            {
                if (this.Disabler) return;

                string path = this.Paths.GetPath();
                if (this.ClipboardPath != path)
                {
                    await this.ObservableCollection.Move(path, this.ClipboardProjects);
                }
                this.ClipboardProjects.Clear();
                this.Click(ActionType.MoveHide);
            };

            this.RenameCommand.Click += (s, e) => this.Click(ActionType.RenameProject, e);

            this.DupliateItem.Click += (s, e) => this.Click(ActionType.DupliateProject, this.RenameItem.CommandParameter);

            this.DeleteItem.Click += (s, e) => this.Click(ActionType.DeleteProject, this.RenameItem.CommandParameter);
            this.DeleteItem2.Click += (s, e) => this.Click(ActionType.DeleteProject, this.RenameItem.CommandParameter);

            this.MoveItem.Click += (s, e) => this.Click(ActionType.MoveProject, this.RenameItem.CommandParameter);

            this.LocalItem.Click += (s, e) => this.Click(ActionType.LocalFolder, this.RenameItem.CommandParameter);
            this.LocalItem2.Click += (s, e) => this.Click(ActionType.LocalFolder, this.RenameItem.CommandParameter);

            this.RenameDialog.IsPrimaryButtonEnabled = false;
            this.RenameTextBox.TextChanged += (s, e) => this.RenameDialog.IsPrimaryButtonEnabled = this.ObservableCollection.Match(this.RenameTextBox.Text);
        }

    }
}