using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents a history stack that contains methods such as undo redo.
    /// </summary>
    public sealed class Historian<T> : INotifyCollectionChanged
        where T : IDisposable
    {

        //@Delegate
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            remove => this.Items.CollectionChanged -= value;
            add => this.Items.CollectionChanged += value;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether items in the history can undo.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (this.Index < 0) return false;
                if (this.Index >= this.Items.Count) return false;
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether items in the history can redo.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                if (this.Index + 1 < 0) return false;
                if (this.Index + 1 >= this.Items.Count) return false;
                return true;
            }
        }

        /// <summary>
        /// Gets the index of currently item.
        /// </summary>
        public int Index { get; private set; } = -1;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Historian{T}"/>.
        /// </summary>
        public int Count => this.Items.Count;

        /// <summary>
        /// Gets the limit of count. 
        /// </summary>
        public readonly int Limit;

        /// <summary>
        /// Gets the source of items. 
        /// </summary>
        public object Source => this.Items;

        readonly ObservableCollection<T> Items = new ObservableCollection<T>();

        //@Construct
        /// <summary>
        /// Initializes a Historian. 
        /// </summary> 
        /// <param name="limit"> The limit of count. </param>
        public Historian(int limit = 20)
        {
            this.Limit = limit;
        }


        /// <summary>
        /// Inserts an <see cref="T"/> at the top of the <see cref="Historian{T}"/>.
        /// </summary>
        /// <param name="history">
        /// The object to push onto the <see cref="Historian{T}"/>. 
        /// The value can be null for reference types.
        /// </param>
        /// <returns> The count of all removed items. </returns>
        public int Push(T history)
        {
            int removes = 0;
            {
                if (this.Items.Count > 0)
                {
                    while (this.Index + 1 < this.Items.Count)
                    {
                        int i = this.Items.Count - 1;

                        T remove = this.Items[i];
                        remove.Dispose();
                        this.Items.Remove(remove);

                        removes++;
                    }
                }

                if (this.Items.Count >= this.Limit)
                {
                    this.Items.RemoveAt(0);
                    this.Items.Add(history);
                }
                else
                {
                    this.Index++;
                    this.Items.Add(history);
                }
            }
            return removes;
        }

        /// <summary>
        /// Undo.
        /// </summary>
        /// <param name="actionUndo"> The action for undo. </param>
        /// <returns> **True** if successful; otherwise, **False**. </returns>
        public bool Undo(Func<T, bool> actionUndo)
        {
            if (this.CanUndo == false) return false;

            if (this.Items[this.Index] is T history)
            {
                this.Index--;
                return actionUndo(history);
            }

            return false;
        }

        /// <summary>
        /// Redo.
        /// </summary>
        /// <param name="actionRedo"> The action for redo. </param>
        /// <returns> **True** if successful; otherwise, **False**. </returns>
        public bool Redo(Func<T, bool> actionRedo)
        {
            if (this.CanRedo == false) return false;

            if (this.Items[this.Index + 1] is T history)
            {
                this.Index++;
                return actionRedo(history);
            }

            return false;
        }

        /// <summary>
        /// Sets the index by calling <see cref="Historian{T}.Undo(Func{T, bool})"/> and <see cref="Historian{T}.Redo(Func{T, bool})"/>.
        /// </summary>
        /// <param name="index"> The target index. </param>
        /// <param name="actionUndo"> The action for undo. </param>
        /// <param name="actionRedo"> The action for redo. </param>
        /// <returns> The offset of index and target index. </returns>
        public int SetIndex(int index, Func<T, bool> actionUndo, Func<T, bool> actionRedo)
        {
            if (index == this.Index) return 0;
            else if (index < this.Index)
            {
                int offset = 0;
                for (int i = this.Index; i > -1; i--)
                {
                    if (index == this.Index) break;
                    else if (this.Items[i] is T history)
                    {
                        this.Index--;
                        offset--;
                        actionUndo(history);
                    }
                    else break;
                }
                return offset;
            }
            else if (index > this.Index)
            {
                int offset = 0;
                for (int i = this.Index; i + 1 < this.Items.Count; i++)
                {
                    if (index == this.Index) break;
                    else if (this.Items[i + 1] is T history)
                    {
                        this.Index++;
                        offset++;
                        actionRedo(history);
                    }
                    else break;
                }
                return offset;
            }
            else return 0;
        }

        /// <summary>
        /// Removes all items from the <see cref="Historian{T}"/>.
        /// </summary>
        public void Clear()
        {
            this.Index = -1;
            this.Items.Clear();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire System.Collections.ObjectModel.Collection`1.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="Historian{T}"/>. The value can
        /// be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire System.Collections.ObjectModel.Collection`1,
        /// if found; otherwise, -1.
        /// </returns>
        public int IndexOf(T item) => this.Items.IndexOf(item);

    }
}