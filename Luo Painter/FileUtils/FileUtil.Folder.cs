using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Luo_Painter
{
    public static partial class FileUtil
    {

        public static async Task<IRandomAccessStream> CreateStreamAsync(this StorageFolder storageFolder, string desiredName)
        {
            StorageFile file = await storageFolder.CreateFileAsync(desiredName, CreationCollisionOption.ReplaceExisting);
            return await file.OpenAsync(FileAccessMode.ReadWrite);
        }

        public static async Task<StorageFolder> GetFolderFromPathAsync(string path)
        {
            try
            {
                return await StorageFolder.GetFolderFromPathAsync(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}