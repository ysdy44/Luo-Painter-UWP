using Luo_Painter.Elements;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
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

        Dupliate,
        Delete,
        Move,

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
                        if (obj is ProjectFile project)
                        {
                            ProjectParameter parameter = await project.LoadAsync();
                            if (parameter is null)
                            {
                                await new MessageDialog("Folder not found.").ShowAsync();
                                break;
                            }

                            base.Frame.Navigate(typeof(DrawPage), parameter);
                            return;
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
                    if (ContentDialogExtensions.CanShow)
                    {
                        ContentDialogResult result = await this.AddDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                System.Drawing.Size size = this.SizePicker.Size;

                                StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                ProjectFile project = new ProjectFile(item);
                                ProjectParameter parameter = await project.SaveAsync(size);

                                this.ObservableCollection.Insert(project);
                                base.Frame.Navigate(typeof(DrawPage), parameter);
                                return;
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
                            ProjectParameter parameter = await project.SaveAsync(bitmap.SizeInPixels, bitmap.GetPixelBytes().AsBuffer(), thumbnail);

                            this.ObservableCollection.Insert(project);
                            base.Frame.Navigate(typeof(DrawPage), parameter);
                            return;
                        }
                    }

                case ProjectAction.DupliateShow:
                    {
                        this.ObservableCollection.Enable(ProjectType.File);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DupliateDocker.IsShow = true;
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
                        this.ObservableCollection.Enable(ProjectType.File | ProjectType.Folder);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DeleteDocker.IsShow = true;
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
                        this.ObservableCollection.Enable(ProjectType.File);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.SelectDocker.IsShow = true;
                    }
                    break;
                case ProjectAction.SelectHide:
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.SelectDocker.IsShow = false;
                    break;
                case ProjectAction.SelectToMove:
                    this.ObservableCollection.Enable(ProjectType.Folder);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.SelectDocker.IsShow = false;
                    this.MoveDocker.IsShow = true;
                    break;
                case ProjectAction.MoveHide:
                    this.ObservableCollection.Enable(ProjectType.All);
                    this.ObservableCollection.Enable();
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.MoveDocker.IsShow = false;
                    break;

                case ProjectAction.Dupliate:
                    {
                        if (obj is ProjectFile projectFile)
                        {
                            await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), projectFile);
                        }
                    }
                    break;
                case ProjectAction.Delete:
                    {
                        if (obj is Project project)
                        {
                            await this.ObservableCollection.DeleteAsync(project);
                        }
                    }
                    break;
                case ProjectAction.Move:
                    {
                        if (obj is Project project)
                        {
                            this.ClipboardPath = this.Paths.GetPath();
                            this.ClipboardProjects.Add(project.Path);

                            //@Debug
                            //if (false)
                            //{
                            //    this.Action(ProjectAction.SelectToMove);
                            //}
                            //else
                            //{
                            this.ObservableCollection.Enable(ProjectType.Folder);
                            this.ListView.IsItemClickEnabled = true;
                            this.AppBarListView.IsItemClickEnabled = false;
                            this.SelectDocker.IsShow = false;
                            this.MoveDocker.IsShow = true;
                            //}
                        }
                    }
                    break;

                case ProjectAction.New:
                    if (ContentDialogExtensions.CanShow)
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
                    if (ContentDialogExtensions.CanShow)
                    {
                        if (obj is Project project)
                        {
                            this.RenameTextBox.Text = project.DisplayName;
                            this.RenameTextBox.SelectAll();
                            this.RenameTextBox.Focus(FocusState.Keyboard);

                            switch (project.Type)
                            {
                                case ProjectType.File:
                                    this.RenameIcon.Symbol = Symbol.Document;
                                    break;
                                case ProjectType.Folder:
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
                                        case ProjectType.File:
                                            if (project is ProjectFile projectFile)
                                            {
                                                await projectFile.RenameAsync(name);
                                            }
                                            break;
                                        case ProjectType.Folder:
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