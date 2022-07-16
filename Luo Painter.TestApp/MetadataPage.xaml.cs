using Luo_Painter.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MetadataPage : Page
    {

        IEnumerable<string> Items => this.Dictionary.Keys.OrderBy(c => c);
        readonly IDictionary<string, string> Dictionary = new Dictionary<string, string>
        {
            ["AAA"] = "AAA",
            ["BBB"] = "BBB",
            ["CCC"] = "CCC",


            [@"AAA\111"] = "111",
            [@"AAA\222"] = "222",
            [@"AAA\333"] = "333",

            [@"BBB\111"] = "111",
            [@"BBB\222"] = "222",
            [@"BBB\333"] = "333",

            [@"CCC\111"] = "111",
            [@"CCC\222"] = "222",
            [@"CCC\333"] = "333",


            [@"AAA\111\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"AAA\111\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"AAA\111\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"AAA\222\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"AAA\222\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"AAA\222\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"AAA\333\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"AAA\333\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"AAA\333\ⅢⅢⅢ"] = "ⅢⅢⅢ",


            [@"BBB\111\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"BBB\111\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"BBB\111\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"BBB\222\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"BBB\222\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"BBB\222\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"BBB\333\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"BBB\333\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"BBB\333\ⅢⅢⅢ"] = "ⅢⅢⅢ",


            [@"CCC\111\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"CCC\111\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"CCC\111\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"CCC\222\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"CCC\222\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"CCC\222\ⅢⅢⅢ"] = "ⅢⅢⅢ",

            [@"CCC\333\ⅠⅠⅠ"] = "ⅠⅠⅠ",
            [@"CCC\333\ⅡⅡⅡ"] = "ⅡⅡⅡ",
            [@"CCC\333\ⅢⅢⅢ"] = "ⅢⅢⅢ",
        };
        readonly IDictionary<string, string[]> Nodes = new Dictionary<string, string[]>
        {
            ["Root"] = new string[]
            {
                "AAA",
                "BBB",
                "CCC",
            },


            ["AAA"] = new string[]
            {
                @"AAA\111",
                @"AAA\222",
                @"AAA\333",
            },
            ["BBB"] = new string[]
            {
                @"BBB\111",
                @"BBB\222",
                @"BBB\333",
            },
            ["CCC"] = new string[]
            {
                @"CCC\111",
                @"CCC\222",
                @"CCC\333",
            },


            [@"AAA\111"] = new string[]
            {
                @"AAA\111\ⅠⅠⅠ",
                @"AAA\111\ⅡⅡⅡ",
                @"AAA\111\ⅢⅢⅢ",
            },
            [@"AAA\222"] = new string[]
            {
                @"AAA\222\ⅠⅠⅠ",
                @"AAA\222\ⅡⅡⅡ",
                @"AAA\222\ⅢⅢⅢ",
            },
            [@"AAA\333"] = new string[]
            {
                @"AAA\333\ⅠⅠⅠ",
                @"AAA\333\ⅡⅡⅡ",
                @"AAA\333\ⅢⅢⅢ",
            },


            [@"BBB\111"] = new string[]
            {
                @"BBB\111\ⅠⅠⅠ",
                @"BBB\111\ⅡⅡⅡ",
                @"BBB\111\ⅢⅢⅢ",
            },
            [@"BBB\222"] = new string[]
            {
                @"BBB\222\ⅠⅠⅠ",
                @"BBB\222\ⅡⅡⅡ",
                @"BBB\222\ⅢⅢⅢ",
            },
            [@"BBB\333"] = new string[]
            {
                @"BBB\333\ⅠⅠⅠ",
                @"BBB\333\ⅡⅡⅡ",
                @"BBB\333\ⅢⅢⅢ",
            },


            [@"CCC\111"] = new string[]
            {
                @"CCC\111\ⅠⅠⅠ",
                @"CCC\111\ⅡⅡⅡ",
                @"CCC\111\ⅢⅢⅢ",
            },
            [@"CCC\222"] = new string[]
            {
                @"CCC\222\ⅠⅠⅠ",
                @"CCC\222\ⅡⅡⅡ",
                @"CCC\222\ⅢⅢⅢ",
            },
            [@"CCC\333"] = new string[]
            {
                @"CCC\333\ⅠⅠⅠ",
                @"CCC\333\ⅡⅡⅡ",
                @"CCC\333\ⅢⅢⅢ",
            },
        };

        readonly MetadataObservableCollection Paths = new MetadataObservableCollection();
        readonly ObservableCollection<string> Folders = new ObservableCollection<string>();

        public MetadataPage()
        {
            this.InitializeComponent();
            this.Load();

            this.BackButton.Click += (s, e) =>
            {
                if (this.Paths.GoBack())
                {
                    this.Load();
                }
            };
            this.HomeButton.Click += (s, e) =>
            {
                if (this.Paths.GoHome())
                {
                    this.Load();
                }
            };

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is string item)
                {
                    int removes = this.Paths.Navigate(item);
                    if (removes is 0) return;

                    this.Load();
                }
            };
            this.GridView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is string item)
                {
                    this.Paths.Add(item);
                    this.Load();
                }
            };
        }

        private void Load()
        {
            string path = this.Paths.GetPath();

            string[] folder =
                (path is null) ?
                this.Nodes["Root"] :
                this.Nodes.ContainsKey(path) ? this.Nodes[path] : null;

            this.Folders.Clear();

            if (folder is null) return;
            foreach (string item in folder)
            {
                this.Folders.Add(this.Dictionary[item]);
            }
        }

        //private async void Load()
        //{
        //    string path = this.Paths.GetPath();
        //
        //    StorageFolder folder =
        //        (path is null) ?
        //        ApplicationData.Current.LocalFolder :
        //        await StorageFolder.GetFolderFromPathAsync(
        //            System.IO.Path.Combine(
        //                ApplicationData.Current.LocalFolder.Path,
        //                System.IO.Path.Combine(path)));
        //
        //    this.Folders.Clear();
        //
        //   if (folder is null) return;
        //    foreach (StorageFolder item in await folder.GetFoldersAsync())
        //    {
        //        this.Folders.Add(item.DisplayName);
        //    }
        //}

    }
}