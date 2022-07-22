using Luo_Painter.Elements;
using Luo_Painter.Projects;
using Luo_Painter.Projects.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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

        private async void Action(ProjectAction action, Project project = null)
        {
            switch (action)
            {
                case ProjectAction.File:
                    {
                        if (project is null) return;

                        StorageFolder item = await StorageFolder.GetFolderFromPathAsync(project.Path);
                        if (item is null) return;

                        string docProject = null;
                        string docLayers = null;
                        IDictionary<string, IBuffer> bitmaps = new Dictionary<string, IBuffer>();

                        foreach (StorageFile item2 in await item.GetFilesAsync())
                        {
                            string id = item2.Name;
                            if (string.IsNullOrEmpty(id)) continue;

                            switch (id)
                            {
                                case "Thumbnail.png":
                                    break;
                                case "Project.xml":
                                    docProject = item2.Path;
                                    break;
                                case "Layers.xml":
                                    docLayers = item2.Path;
                                    break;
                                default:
                                    bitmaps.Add(id, await FileIO.ReadBufferAsync(item2));
                                    break;
                            }
                        }

                        if (docProject is null) break;
                        if (docLayers is null) break;

                        XDocument docProject2 = XDocument.Load(docProject);
                        if (docProject2.Root.Element("Width") is XElement width2)
                        {
                            if (docProject2.Root.Element("Height") is XElement height2)
                            {
                                if (int.TryParse(width2.Value, out int width))
                                {
                                    if (int.TryParse(height2.Value, out int height))
                                    {
                                        base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(project.Path, project.Name, width, height, docProject, docLayers, bitmaps));
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ProjectAction.Folder:
                    if (project is null) break;
                    this.Paths.Add(new Metadata(project.Path, project.Name));

                    this.Load();
                    this.UpdateBack();
                    break;

                case ProjectAction.Add:
                    {
                        ContentDialogResult result = await this.AddDialog.ShowInstance();

                        switch (result)
                        {
                            case ContentDialogResult.Primary:
                                StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), this.Untitled);
                                this.ObservableCollection.Insert(item);
                                base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(item.Path, item.Name, this.SizePicker.Size));
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

                        ImageProperties prop = await file.Properties.GetImagePropertiesAsync();
                        if (prop is null) break;

                        StorageFolder item = await this.ObservableCollection.Create(this.Paths.GetPath(), file.DisplayName);
                        this.ObservableCollection.Insert(item);

                        base.Frame.Navigate(typeof(DrawPage), new ProjectParameter(item.Path, item.Name, prop.Width, prop.Height, await FileIO.ReadBufferAsync(file)));
                    }
                    break;

                case ProjectAction.DupliateShow:
                    this.ObservableCollection.Enable(StorageItemTypes.File);
                    this.ListView.IsItemClickEnabled = false;
                    this.AppBarListView.IsItemClickEnabled = false;
                    this.DupliateDocker.IsShow = true;

                    if (project is null) break;
                    this.ListView.SelectedItem = project;
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

                    if (project is null) break;
                    this.ListView.SelectedItem = project;
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

                    if (project is null) break;
                    this.ListView.SelectedItem = project;
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
                        if (project is null) break;
                        this.RenameTextBox.Text = project.DisplayName;
                        this.RenameTextBox.SelectAll();
                        this.RenameTextBox.Focus(FocusState.Keyboard);

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
                    break;
                case ProjectAction.Local:
                    {
                        if (project is null) break;
                        await Launcher.LaunchFolderPathAsync(project.Path);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}