using Luo_Painter.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MetadataPage : Page
    {

        IEnumerable<string> Items => this.Dictionary.Select(c => c.Path).OrderBy(c => c);
        readonly IList<Metadata> Dictionary = new List<Metadata>
        {
            new Metadata("AAA", "AAA"),
            new Metadata("BBB", "BBB"),
            new Metadata("CCC", "CCC"),


            new Metadata(@"AAA\111", "111"),
            new Metadata(@"AAA\222", "222"),
            new Metadata(@"AAA\333", "333"),

            new Metadata(@"BBB\111", "111"),
            new Metadata(@"BBB\222", "222"),
            new Metadata(@"BBB\333", "333"),

            new Metadata(@"CCC\111", "111"),
            new Metadata(@"CCC\222", "222"),
            new Metadata(@"CCC\333", "333"),


            new Metadata(@"AAA\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"AAA\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"AAA\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"AAA\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"AAA\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"AAA\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"AAA\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"AAA\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"AAA\333\ⅢⅢⅢ", "ⅢⅢⅢ"),


            new Metadata(@"BBB\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"BBB\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"BBB\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"BBB\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"BBB\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"BBB\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"BBB\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"BBB\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"BBB\333\ⅢⅢⅢ", "ⅢⅢⅢ"),


            new Metadata(@"CCC\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"CCC\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"CCC\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"CCC\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"CCC\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"CCC\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Metadata(@"CCC\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Metadata(@"CCC\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Metadata(@"CCC\333\ⅢⅢⅢ", "ⅢⅢⅢ"),
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
        readonly ObservableCollection<Metadata> Folders = new ObservableCollection<Metadata>();

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
                if (e.ClickedItem is Metadata item)
                {
                    int removes = this.Paths.Navigate(item.Path);
                    if (removes is 0) return;

                    this.Load();
                }
            };
            this.GridView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Metadata item)
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
                foreach (Metadata item2 in this.Dictionary)
                {
                    if (item2.Path == item)
                    {
                        this.Folders.Add(item2);
                    }
                }
            }
        }

        //private async Task<bool> Load()
        //{
        //    string path = this.Paths.GetPath();
        //
        //    if (path is null)
        //    {
        //        this.Folders.Clear();
        //        foreach (StorageFolder item in await ApplicationData.Current.LocalFolder.GetFoldersAsync())
        //        {
        //            this.Folders.Add(new Metadata(item));
        //        }
        //        return true;
        //    }
        //
        //    try
        //    {
        //        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
        //        if (folder is null) return false;
        //
        //        this.Folders.Clear();
        //        foreach (StorageFolder item in await folder.GetFoldersAsync())
        //        {
        //            this.Folders.Add(new Metadata(item));
        //        }
        //        return true;
        //    }
        //    catch (ArgumentException)
        //    {
        //        return false;
        //    }
        //}

    }
}