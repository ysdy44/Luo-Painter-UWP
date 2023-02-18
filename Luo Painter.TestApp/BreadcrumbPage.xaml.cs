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
    public sealed partial class BreadcrumbPage : Page
    {

        IEnumerable<string> Items => this.Dictionary.Select(c => c.Path).OrderBy(c => c);
        readonly IList<Breadcrumb> Dictionary = new List<Breadcrumb>
        {
            new Breadcrumb("AAA", "AAA"),
            new Breadcrumb("BBB", "BBB"),
            new Breadcrumb("CCC", "CCC"),


            new Breadcrumb(@"AAA\111", "111"),
            new Breadcrumb(@"AAA\222", "222"),
            new Breadcrumb(@"AAA\333", "333"),

            new Breadcrumb(@"BBB\111", "111"),
            new Breadcrumb(@"BBB\222", "222"),
            new Breadcrumb(@"BBB\333", "333"),

            new Breadcrumb(@"CCC\111", "111"),
            new Breadcrumb(@"CCC\222", "222"),
            new Breadcrumb(@"CCC\333", "333"),


            new Breadcrumb(@"AAA\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"AAA\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"AAA\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"AAA\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"AAA\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"AAA\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"AAA\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"AAA\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"AAA\333\ⅢⅢⅢ", "ⅢⅢⅢ"),


            new Breadcrumb(@"BBB\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"BBB\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"BBB\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"BBB\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"BBB\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"BBB\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"BBB\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"BBB\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"BBB\333\ⅢⅢⅢ", "ⅢⅢⅢ"),


            new Breadcrumb(@"CCC\111\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"CCC\111\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"CCC\111\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"CCC\222\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"CCC\222\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"CCC\222\ⅢⅢⅢ", "ⅢⅢⅢ"),

            new Breadcrumb(@"CCC\333\ⅠⅠⅠ", "ⅠⅠⅠ"),
            new Breadcrumb(@"CCC\333\ⅡⅡⅡ", "ⅡⅡⅡ"),
            new Breadcrumb(@"CCC\333\ⅢⅢⅢ", "ⅢⅢⅢ"),
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

        readonly BreadcrumbObservableCollection Paths = new BreadcrumbObservableCollection();
        readonly ObservableCollection<Breadcrumb> Folders = new ObservableCollection<Breadcrumb>();

        public BreadcrumbPage()
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
                if (e.ClickedItem is Breadcrumb item)
                {
                    int removes = this.Paths.Navigate(item.Path);
                    if (removes is 0) return;

                    this.Load();
                }
            };
            this.GridView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Breadcrumb item)
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
                foreach (Breadcrumb item2 in this.Dictionary)
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
        //            this.Folders.Add(new Breadcrumb(item));
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
        //            this.Folders.Add(new Breadcrumb(item));
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