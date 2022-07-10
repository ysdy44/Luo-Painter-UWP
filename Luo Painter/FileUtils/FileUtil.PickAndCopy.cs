using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace Luo_Painter
{
    public static partial class FileUtil
    {

        /// <summary>
        /// The file picker is displayed so that the user can select a file.
        /// </summary>
        /// <param name="location"> The destination locationId. </param>
        /// <returns> The product file. </returns>
        public static async Task<StorageFile> PickSingleFileAsync(PickerLocationId location, string fileChoices = ".Jpeg", string suggestedFileName = "Untitled")
        {
            // FileSavePicker
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = suggestedFileName,
                FileTypeChoices =
                {
                    {"DB", new[] { fileChoices } }
                }
            };


            // PickSaveFileAsync
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file is null) return null;

            return file;
        }


        /// <summary>
        /// The file picker is displayed so that the user can select a folder.
        /// </summary>
        /// <param name="location"> The destination locationId. </param>
        /// <returns> The product folder. </returns>
        public async static Task<StorageFolder> PickSingleFolderAsync(PickerLocationId location)
        {
            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    "*"
                }
            };

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder is null) return null;

            // Application now has read/write access to all contents in the picked folder
            // (including other sub-folder contents)
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            return folder;
        }


        /// <summary>
        /// Copy file to the temporary folder, and return the copy.
        /// </summary>
        /// <param name="file"> The destination file. </param>
        /// <returns> The copied file. </returns>
        public async static Task<StorageFile> CopySingleImageFileAsync(StorageFile file)
        {
            if (file is null) return null;

            StorageFile copyFile = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name, NameCollisionOption.ReplaceExisting);
            return copyFile;
        }

        /// <summary>
        /// The file picker is displayed so that the user can select a file.
        /// </summary>
        /// <param name="location"> The destination locationId. </param>
        /// <returns> The product file. </returns>
        public async static Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                     ".jpg", ".jpeg", ".jpe",
                     ".png",
                     ".gif",
                     ".bmp", ".dib",
                     ".tif", ".tiff",
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        /// <summary>
        /// The files picker is displayed so that the user can select a files.
        /// </summary>
        /// <param name="location"> The destination locationId. </param>
        /// <returns> The product files. </returns>
        public async static Task<IReadOnlyList<StorageFile>> PickMultipleImageFilesAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                     ".jpg", ".jpeg", ".jpe",
                     ".png",
                     ".gif",
                     ".bmp", ".dib",
                     ".tif", ".tiff",
                }
            };

            // File
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            return files;
        }


        /// <summary>
        /// Filter files that are not pictures.
        /// Then copy to the temporary folder, and return the copy.
        /// </summary>
        /// <param name="item"> The destination item. </param>
        /// <returns> The product file. </returns>
        public async static Task<StorageFile> CopySingleImageFileAsync(IStorageItem item)
        {
            if (item is null) return null;

            if (item is StorageFile file)
            {
                switch (file.FileType.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".jpe":
                    case ".png":
                    case ".gif":
                    case ".bmp":
                    case ".dib":
                    case ".tif":
                    case ".tiff":
                        StorageFile copyFile = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        return copyFile;
                }
            }
            return null;
        }

    }
}