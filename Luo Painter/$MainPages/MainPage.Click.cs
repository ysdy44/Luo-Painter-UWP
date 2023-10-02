using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using System;
using System.Linq;
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

    public sealed partial class MainPage : Page
    {

        [MainPageToDrawPage(NavigationMode.Forward)]
        private async void Click(ActionType action, object obj = null)
        {
            if (this.Disabler) return;
            this.Disabler = true;

            switch (action)
            {
                case ActionType.OpenProject:
                    {
                        if (obj is Project project)
                        {
                            ProjectParameter parameter = await project.LoadAsync();
                            if (parameter is null)
                            {
                                await new MessageDialog(project.Path, App.Resource.GetString($"Tip_{TipType.NoFile}")).ShowAsync();
                                break;
                            }

                            base.Frame.Navigate(typeof(DrawPage), parameter);
                            return;
                        }
                    }
                    break;
                case ActionType.OpenFolder:
                    {
                        if (obj is ProjectBase project)
                        {
                            this.Paths.Add(new Breadcrumb(project.Path, project.Name));

                            this.Load();
                            this.UpdateBack();
                        }
                    }
                    break;

                case ActionType.NewProject:
                    if (ContentDialogExtensions.CanShow)
                    {
                        var screen = FileUtil.GetCurrentViewBounds();
                        this.SizePicker.ResezingWidth(System.Math.Clamp(screen.Width, 16, 8192));
                        this.SizePicker.ResezingHeight(System.Math.Clamp(screen.Height, 16, 8192));

                        ContentDialogResult result = await this.AddDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                System.Drawing.Size size = this.SizePicker.Size;

                                StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                Project project = new Project(item);
                                ProjectParameter parameter = await project.SaveAsync(item, size);

                                this.ObservableCollection.Insert(project);
                                base.Frame.Navigate(typeof(DrawPage), parameter);
                                return;
                            default:
                                break;
                        }
                    }
                    break;
                case ActionType.NewImage:
                    {
                        StorageFile file = (StorageFile)obj;
                        if (file is null) file = await FileUtil.PickSingleImageFileAsync(PickerLocationId.Desktop);
                        if (file is null) break;

                        using (IRandomAccessStream streamBitmap = await file.OpenAsync(FileAccessMode.Read))
                        using (Microsoft.Graphics.Canvas.CanvasBitmap bitmap = await Microsoft.Graphics.Canvas.CanvasBitmap.LoadAsync(Microsoft.Graphics.Canvas.CanvasDevice.GetSharedDevice(), streamBitmap))
                        using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView))
                        {
                            StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), file.DisplayName);
                            Project project = new Project(item);
                            ProjectParameter parameter = await project.SaveAsync(bitmap.SizeInPixels, bitmap.GetPixelBytes().AsBuffer(), thumbnail);

                            this.ObservableCollection.Insert(project);
                            base.Frame.Navigate(typeof(DrawPage), parameter);
                            return;
                        }
                    }
                case ActionType.NewFolder:
                    {
                        this.RenameTextBox.Text = this.ObservableCollection.Rename(this.New);
                        this.RenameTextBox.SelectAll();
                        this.RenameTextBox.Focus(FocusState.Keyboard);

                        this.RenameIcon.Symbol = Symbol.Folder;
                        this.RenameDialog.IsPrimaryButtonEnabled = true;

                        ContentDialogResult result = await this.RenameDialog.ShowInstance();
                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                string name = this.RenameTextBox.Text;
                                if (string.IsNullOrEmpty(name)) break;

                                await this.ObservableCollection.AddFolderAsync(this.Paths.GetPath(), name);
                                this.ListView.ScrollIntoView(this.ObservableCollection.Last());
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                case ActionType.DupliateShow:
                    {
                        this.ObservableCollection.Enable(ProjectType.Project);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DupliateDocker.IsShow = true;
                    }
                    break;
                case ActionType.DupliateHide:
                    this.ObservableCollection.Enable(ProjectType.All);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DupliateDocker.IsShow = false;
                    break;
                case ActionType.DeleteShow:
                    {
                        this.ObservableCollection.Enable(ProjectType.Project | ProjectType.Folder);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.DeleteDocker.IsShow = true;
                    }
                    break;
                case ActionType.DeleteHide:
                    this.ObservableCollection.Enable(ProjectType.All);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.DeleteDocker.IsShow = false;
                    break;
                case ActionType.SelectShow:
                    {
                        this.ObservableCollection.Enable(ProjectType.Project);
                        this.ListView.IsItemClickEnabled = false;
                        this.AppBarListView.IsItemClickEnabled = false;
                        this.SelectDocker.IsShow = true;
                    }
                    break;
                case ActionType.SelectHide:
                    this.ObservableCollection.Enable(ProjectType.All);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.SelectDocker.IsShow = false;
                    break;
                case ActionType.SelectToMove:
                    this.ObservableCollection.Enable(ProjectType.Folder);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.SelectDocker.IsShow = false;
                    this.MoveDocker.IsShow = true;
                    break;
                case ActionType.MoveHide:
                    this.ObservableCollection.Enable(ProjectType.All);
                    this.ListView.IsItemClickEnabled = true;
                    this.AppBarListView.IsItemClickEnabled = true;
                    this.MoveDocker.IsShow = false;
                    break;

                case ActionType.DupliateProject:
                    {
                        if (obj is Project projectFile)
                        {
                            await this.ObservableCollection.CopyAsync(this.Paths.GetPath(), projectFile);
                        }
                    }
                    break;
                case ActionType.DeleteProject:
                    {
                        if (obj is ProjectBase project)
                        {
                            await this.ObservableCollection.DeleteAsync(project);
                        }
                    }
                    break;
                case ActionType.MoveProject:
                    {
                        if (obj is ProjectBase project)
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

                case ActionType.RenameProject:
                    if (ContentDialogExtensions.CanShow)
                    {
                        if (obj is ProjectBase project)
                        {
                            this.RenameTextBox.Text = project.DisplayName;
                            this.RenameTextBox.SelectAll();
                            this.RenameTextBox.Focus(FocusState.Keyboard);

                            switch (project.Type)
                            {
                                case ProjectType.Project:
                                    this.RenameIcon.Symbol = Symbol.Document;
                                    this.RenameDialog.IsPrimaryButtonEnabled = false;
                                    break;
                                case ProjectType.Folder:
                                    this.RenameIcon.Symbol = Symbol.Folder;
                                    this.RenameDialog.IsPrimaryButtonEnabled = false;
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
                                        case ProjectType.Project:
                                            if (project is Project projectFile)
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
                case ActionType.LocalFolder:
                    {
                        if (obj is ProjectBase project)
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