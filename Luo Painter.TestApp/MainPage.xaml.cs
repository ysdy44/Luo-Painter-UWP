using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MainPage : Page
    {

        //@Converter
        private bool BooleanNullableConverter(bool? value) => value == true;

        //@Instance
        private readonly Lazy<ApplicationView> ViewLazy = new Lazy<ApplicationView>(() => ApplicationView.GetForCurrentView());
        private ApplicationView View => this.ViewLazy.Value;

        public MainPage()
        {
            this.InitializeComponent();
            collection.Source = this.CreatePages(typeof(MainPage)).GroupBy(c => c.Name.Substring(0, 1)).OrderBy(x => x.Key);
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Type item)
                {
                    this.View.Title = item.Name;
                    this.Frame.Navigate(item);
                }
            };
        }

        private IEnumerable<Type> CreatePages(Type assemblyType)
        {
            Assembly assembly = assemblyType.GetTypeInfo().Assembly;
            IEnumerable<TypeInfo> typeInfos = assembly.DefinedTypes;

            foreach (TypeInfo typeInfo in typeInfos)
            {
                string name = typeInfo.Name; // $"{Main}Page"
                string fullName = typeInfo.FullName; // $"Luo_Painter.TestApp.{Main}Page"

                if (name == "MainPage") continue;
                if (name.EndsWith("Page"))
                {
                    Type type = typeInfo.AsType();
                    yield return type;
                    // yield return Activator.CreateInstance(type);
                }
            }
        }

    }
}