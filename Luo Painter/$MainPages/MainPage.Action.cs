using Luo_Painter.Elements;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        [MainPageToDrawPage(NavigationMode.Forward)]
        private async void Action(ProjectAction action, object obj = null)
        {
            if (this.Disabler) return;
            this.Disabler = true;

            switch (action)
            {
                case ProjectAction.File:
                    {
                        if (obj is Project project)
                        {
                            ProjectParameter parameter = await project.LoadAsync();
                            if (parameter is null)
                            {
                                await new MessageDialog("Folder not found.").ShowAsync();
                                break;
                            }

                            base.Frame.Navigate(typeof(DrawPage), parameter);
                        }
                    }
                    break;
                case ProjectAction.Folder:
                    {
                        if (obj is Project project)
                        {
                            this.Paths.Add(new Metadata(project.Path, project.Name));

                            this.Load();
                            this.UpdateBack();
                        }
                    }
                    break;

                case ProjectAction.Add:
                    {
                        ContentDialogResult result = await this.AddDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                System.Drawing.Size size = this.SizePicker.Size;

                                StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                ProjectFile project = new ProjectFile(item);
                                ProjectParameter parameter = await project.SaveAsync(item, size);

                                this.ObservableCollection.Insert(project);
                                base.Frame.Navigate(typeof(DrawPage), parameter);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ProjectAction.Image:
                    {
                        StorageFile file = (StorageFile)obj;
                        if (file is null) file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                        if (file is null) break;

                        using (IRandomAccessStream streamBitmap = await file.OpenAsync(FileAccessMode.Read))
                        using (Microsoft.Graphics.Canvas.CanvasBitmap bitmap = await Microsoft.Graphics.Canvas.CanvasBitmap.LoadAsync(Microsoft.Graphics.Canvas.CanvasDevice.GetSharedDevice(), streamBitmap))
                        using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView))
                        {
                            StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), file.DisplayName);
                            ProjectFile project = new ProjectFile(item);
                            ProjectParameter parameter = await project.SaveAsync(item, bitmap.SizeInPixels, bitmap.GetPixelBytes().AsBuffer(), thumbnail);

                            this.ObservableCollection.Insert(project);
                            base.Frame.Navigate(typeof(DrawPage), parameter);
                        }
                    }
                    break;

                case ProjectAction.DupliateShow:
                    {
                        this.ObservableCollection.Enable(StorageItemTypes.File);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DupliateDocker.IsShow = true;

                        if (obj is Project project)
                        {
                            this.ListView.SelectedItem = project;
                        }
                    }
                    break;
                case ProjectAction.DupliateHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DupliateDocker.IsShow = false;
                    break;
                case ProjectAction.DeleteShow:
                    {
                        this.ObservableCollection.Enable(StorageItemTypes.File | StorageItemTypes.Folder);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DeleteDocker.IsShow = true;

                        if (obj is Project project)
                        {
                            this.ListView.SelectedItem = project;
                        }
                    }
                    break;
                case ProjectAction.DeleteHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DeleteDocker.IsShow = false;
                    break;
                case ProjectAction.SelectShow:
                    {
                        this.ObservableCollection.Enable(StorageItemTypes.File);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.SelectDocker.IsShow = true;

                        if (obj is Project project)
                        {
                            this.ListView.SelectedItem = project;
                        }
                    }
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
                        if (obj is Project project)
                        {
                            this.RenameTextBox.Text = project.DisplayName;
                            this.RenameTextBox.SelectAll();
                            this.RenameTextBox.Focus(FocusState.Keyboard);

                            switch (project.Type)
                            {
                                case StorageItemTypes.File:
                                    this.RenameIcon.Symbol = Symbol.Document;
                                    break;
                                case StorageItemTypes.Folder:
                                    this.RenameIcon.Symbol = Symbol.Folder;
                                    break;
                                default:
                                    break;
                            }

                            ContentDialogResult result = await this.RenameDialog.ShowInstance();
                            switch (result)
                            {
                                case ContentDialogResult.Primary:
                                    string name = this.RenameTextBox.Text;
                                    if (string.IsNullOrEmpty(name)) break;

                                    switch (project.Type)
                                    {
                                        case StorageItemTypes.File:
                                            if (project is ProjectFile projectFile)
                                            {
                                                await projectFile.RenameAsync(name);
                                            }
                                            break;
                                        case StorageItemTypes.Folder:
                                            if (project is ProjectFolder projectFolder)
                                            {
                                                await projectFolder.RenameAsync(name);
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
                    }
                    break;
                case ProjectAction.Local:
                    {
                        if (obj is Project project)
                        {
                            await Launcher.LaunchFolderPathAsync(project.Path);
                        }
                    }
                    break;
                default:
                    break;
            }

            this.Disabler = false;
        }

    }
}