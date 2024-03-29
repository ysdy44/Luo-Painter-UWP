﻿using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    public static class FrameworkElementExtensions
    {

        public static void Toggle(this SelectorItem selectorItem) => selectorItem.IsSelected = !selectorItem.IsSelected;
        /// <summary>
        /// Pokes the Z index of a target <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to poke the Z index for.</param>
        public static void PokeZIndex(this UIElement element)
        {
            int oldZIndex = Canvas.GetZIndex(element);

            Canvas.SetZIndex(element, oldZIndex + 1);
            Canvas.SetZIndex(element, oldZIndex);
        }


        public static T FindAncestor<T>(this DependencyObject reference)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            if (parent is null) return default;
            else if (parent is T result) return result;
            else return parent.FindAncestor<T>();
        }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject element)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);

                if (child is null) continue;
                else if (child is T result) yield return result;
                else
                {
                    foreach (T childOfChild in child.FindDescendants<T>())
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}