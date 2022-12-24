using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents a generic selector, 
    /// it notifies <see cref="UIElement"/>s on the <see cref="Canvas"/>
    /// when the <see cref="ObservableCollection{T}.CollectionChanged"/>.
    /// </summary>
    /// <typeparam name="TKey"> The key of item. </typeparam>
    /// <typeparam name="TValue"> The value of item. </typeparam>
    public abstract class TSelector<TKey, TValue> : Canvas
        where TValue : UIElement
    {

        //@Abstract
        /// <summary>
        /// Gets the left of item by its key.
        /// </summary>
        /// <param name="key"> The key of item. </param>
        /// <returns> The product left.</returns>
        public abstract double GetItemLeft(TKey key);
        /// <summary>
        /// Gets the top of item by its key.
        /// </summary>
        /// <param name="key"> The key of item. </param>
        /// <returns> The product top.</returns>
        public abstract double GetItemTop(TKey key);

        /// <summary>
        /// Create value of item by its key.
        /// </summary>
        /// <param name="key"> The key of item. </param>
        /// <returns> The product item.</returns>
        protected abstract TValue CreateItem(TKey key);
        /// <summary>
        /// Add handle for item
        /// </summary>
        /// <param name="value"> The value of item. </param>
        protected abstract void AddItemHandle(TValue value);
        /// <summary>
        /// Remove handle for item.
        /// </summary>
        /// <param name="value"> The value of item. </param>
        protected abstract void RemoveItemHandle(TValue value);

        /// <summary> Gets the count of <see cref = "TSelector{TKey, TValue}" />'s items. </summary>
        public int Count => this.Items.Count;

        /// <summary> Gets the items of <see cref = "TSelector{TKey, TValue}" />. </summary>
        protected readonly IDictionary<TKey, TValue> Items = new Dictionary<TKey, TValue>();
        private INotifyCollectionChanged ItemSourceNotify;

        #region DependencyProperty


        /// <summary> Gets or sets style of <see cref = "TSelector{TKey, TValue}" />'s item. </summary>
        public Style ItemStyle
        {
            get => (Style)base.GetValue(ItemStyleProperty);
            set => base.SetValue(ItemStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "TSelector{TKey, TValue}.ItemStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register(nameof(ItemStyle), typeof(Style), typeof(TSelector<TKey, TValue>), new PropertyMetadata(null));


        /// <summary> Gets or sets template of <see cref = "TSelector{TKey, TValue}" />'s item. </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)base.GetValue(ItemTemplateProperty);
            set => base.SetValue(ItemTemplateProperty, value);
        }
        /// <summary> Identifies the <see cref = "TSelector{TKey, TValue}.ItemTemplate" /> dependency property. </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(TSelector<TKey, TValue>), new PropertyMetadata(null));


        /// <summary> Gets or sets the source of <see cref = "TSelector{TKey, TValue}" /> 's items. </summary>
        public object ItemSource
        {
            get => (object)base.GetValue(ItemSourceProperty);
            set => base.SetValue(ItemSourceProperty, value);
        }
        /// <summary> Identifies the <see cref = "TSelector{TKey, TValue}.ItemSource" /> dependency property. </summary>
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof(ItemSource), typeof(object), typeof(TSelector<TKey, TValue>), new PropertyMetadata(false, (sender, e) =>
        {
            TSelector<TKey, TValue> control = (TSelector<TKey, TValue>)sender;

            if (e.NewValue is object value)
            {
                if (control.ItemSourceNotify != null)
                {
                    control.ItemSourceNotify.CollectionChanged -= control.ItemSourceNotify_CollectionChanged;
                    if (control.ItemSourceNotify is IEnumerable<TKey> items)
                    {
                        foreach (var item in control.Items)
                        {
                            control.RemoveItemHandle(item.Value);
                        }
                        control.Children.Clear();
                    }
                }
                control.ItemSourceNotify = value as INotifyCollectionChanged;
                if (control.ItemSourceNotify != null)
                {
                    control.ItemSourceNotify.CollectionChanged += control.ItemSourceNotify_CollectionChanged;
                    if (control.ItemSourceNotify is IEnumerable<TKey> items)
                    {
                        foreach (TKey item in items)
                        {
                            control.Children.Add(control.Create(item));
                        }
                    }
                }
            }
        }));


        #endregion

        //@Construct
        public TSelector()
        {
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                foreach (var item in this.Items)
                {
                    Canvas.SetLeft(item.Value, this.GetItemLeft(item.Key));
                    Canvas.SetTop(item.Value, this.GetItemTop(item.Key));
                }
            };
        }

        private TValue Create(TKey itemAdd)
        {
            if (this.Items.ContainsKey(itemAdd))
            {
                return this.Items[itemAdd];
            }

            TValue button = this.CreateItem(itemAdd);
            Canvas.SetLeft(button, this.GetItemLeft(itemAdd));
            Canvas.SetTop(button, this.GetItemTop(itemAdd));
            this.AddItemHandle(button);
            this.Items.Add(itemAdd, button);
            return button;
        }

        private void ItemSourceNotify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems[0] is TKey itemAdd)
                    {
                        int index = e.NewStartingIndex;
                        if (this.Items.ContainsKey(itemAdd)) break;
                        base.Children.Insert(index, this.Create(itemAdd));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        int index = e.OldStartingIndex;
                    }
                    if (e.NewItems[0] is TKey itemMove)
                    {
                        int index = e.NewStartingIndex;
                        base.Children.Insert(index, this.Items[itemMove]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems[0] is TKey itemRemove)
                    {
                        int index = e.OldStartingIndex;
                        TValue item = base.Children[index] as TValue;
                        this.RemoveItemHandle(item);
                        base.Children.RemoveAt(index);
                        this.Items.Remove(itemRemove);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems[0] is TKey itemReplace2)
                    {
                        int index = e.OldStartingIndex;
                        TValue item = base.Children[index] as TValue;
                        this.RemoveItemHandle(item);
                        base.Children.RemoveAt(index);
                        this.Items.Remove(itemReplace2);
                    }
                    if (e.NewItems[0] is TKey itemReplace)
                    {
                        int index = e.NewStartingIndex;
                        this.Children.Insert(index, this.Create(itemReplace));
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (UIElement item in base.Children)
                    {
                        this.RemoveItemHandle(item as TValue);
                    }
                    base.Children.Clear();
                    this.Items.Clear();
                    break;

                default:
                    break;
            };
        }

    }
}