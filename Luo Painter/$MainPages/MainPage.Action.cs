using Luo_Painter.Elements;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
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
        SelectShow,
        SelectHide,
        SelectToMove,
        MoveHide,

        New,
        Rename,
        Local,
    }

    public sealed partial class MainPage : Page
    {

        private async void Action(ProjectAction action, Project item = null)
        {
            switch (action)
            {
                case ProjectAction.File:
                    if (item is null) break;

                    base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(item.Path));
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
                                StorageFolder item2 = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                this.ObservableCollection.Insert(item2);
                                base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(item2.Path, this.SizePicker.Size));
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Image:
                    {
                        StorageFile file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                        if (file is null) break;

                        StorageFolder item2 = await this.ObservableCollection.Create(this.Paths.GetPath(), file.DisplayName);
                        StorageFile file2 = await file.CopyAsync(item2);

                        this.ObservableCollection.Insert(item2);
                        base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(item2.Path, file2.Path));
                    }
                    break;

                case ProjectAction.DupliateShow:
                    this.ObservableCollection.Enable(StorageItemTypes.File);
                    this.ListView.IsItemClickEnabled = false;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.DupliateDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DupliateHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DupliateDocker.IsShow = false;
                    break;
                case ProjectAction.DeleteShow:
                    this.ObservableCollection.Enable(StorageItemTypes.File | StorageItemTypes.Folder);
                    this.ListView.IsItemClickEnabled = false;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.DeleteDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.DeleteHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DeleteDocker.IsShow = false;
                    break;
                case ProjectAction.SelectShow:
                    this.ObservableCollection.Enable(StorageItemTypes.File);
                    this.ListView.IsItemClickEnabled = false;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.SelectDocker.IsShow = true;

                    if (item is null) break;
                    this.ListView.SelectedItem = item;
                    break;
                case ProjectAction.SelectHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.SelectDocker.IsShow = false;
                    break;
                case ProjectAction.SelectToMove:
                    this.ObservableCollection.Enable(StorageItemTypes.Folder);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.SelectDocker.IsShow = false;
                    this.MoveDocker.IsShow = true;
                    break;
                case ProjectAction.MoveHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
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
                    {
                        if (item is null) break;
                        this.RenameTextBox.Text = item.DisplayName;
                        this.RenameTextBox.SelectAll();
                        this.RenameTextBox.Focus(FocusState.Keyboard);

                        ContentDialogResult result = await this.RenameDialog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                string name = this.RenameTextBox.Text;
                                if (string.IsNullOrEmpty(name)) break;

                                switch (item.Type)
                                {
                                    case StorageItemTypes.File:
                                        if (item is ProjectFile item2)
                                        {
                                            await item2.RenameAsync(name);
                                        }
                                        break;
                                    case StorageItemTypes.Folder:
                                        if (item is ProjectFolder item3)
                                        {
                                            await item3.RenameAsync(name);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Local:
                    {
                        if (item is null) break;
                        await Launcher.LaunchFolderPathAsync(item.Path);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}