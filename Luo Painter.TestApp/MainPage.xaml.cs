using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class TypeGrouping : List<Type>, IList<Type>, IGrouping<string, Type>
    {
        public string Key { get; }
        public TypeGrouping(string key, IEnumerable<Type> collection) : base(collection) => this.Key = key;
        public override string ToString() => this.Key;
    }

    public sealed partial class MainPage : Page
    {

        //@Converter
        private bool BooleanNullableConverter(bool? value) => value == true;
        private string InitialConverter(Type value) => value.Name.First().ToString();

        //@Instance
        private readonly Lazy<ApplicationView> ViewLazy = new Lazy<ApplicationView>(() => ApplicationView.GetForCurrentView());
        private ApplicationView View => this.ViewLazy.Value;

        public MainPage()
        {
            this.InitializeComponent();
            this.Collection.Source = this.CreatePages(typeof(MainPage)).OrderBy(x => x.Name).GroupBy(this.InitialConverter).Select(c => new TypeGrouping(c.Key, c));
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
                Type type = typeInfo.AsType();
                if (type == assemblyType) continue;

                if (type.BaseType == typeof(Page))
                {
                    yield return type;
                }
            }
        }

    }
}