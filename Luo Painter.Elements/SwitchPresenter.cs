// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// An collection of <see cref="ICase{T}"/> to help with XAML interop.
    /// </summary>
    public class CaseCollection : DependencyObjectCollection { }


    /// <summary>
    /// <see cref="ICase{T}"/> is the value container for the <see cref="SwitchPresenter{T}"/>.
    /// </summary>
    public interface ICase<T>
        where T : Enum
    {
        /// <summary>
        /// Gets or sets the Content to display when this case is active.
        /// </summary>
        object Content { get; }

        /// <summary>
        /// Gets or sets the <see cref="Value"/> that this case represents. If it matches the <see cref="SwitchPresenter.Value"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        T Value { get; }

        /// <summary> The current tool becomes the active case. </summary>
        void OnNavigatedTo();

        /// <summary> The current page does not become an active case. </summary>
        void OnNavigatedFrom();
    }


    /// <summary>
    /// The <see cref="SwitchPresenter{T}"/> is a <see cref="ContentPresenter"/> which can allow a developer to mimic a <c>switch</c> statement within XAML.
    /// When provided a set of <see cref="ICase{T}"/>s and a <see cref="Value"/>, it will pick the matching <see cref="ICase{T}"/> with the corresponding <see cref="ICase.Value"/>.
    /// </summary>
    [ContentProperty(Name = nameof(SwitchCases))]
    public partial class SwitchPresenter<T> : ContentPresenter
        where T : Enum
    {
        public CaseCollection SwitchCases { get; } = new CaseCollection();


        /// <summary>
        /// Gets or sets a value indicating the value to compare all cases against. When this value is bound to and changes, the presenter will automatically evaluate cases and select the new appropriate content from the switch.
        /// </summary>     
        public T Value
        {
            get => (T)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Type), typeof(T), typeof(SwitchPresenter<T>), new PropertyMetadata(default(T), (sender, e) =>
        {
            SwitchPresenter<T> control = (SwitchPresenter<T>)sender;

            if (e.NewValue is T value)
            {
                control.EvaluateCases(value);
            }
        }));

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.EvaluateCases(this.Value);
        }

        private void EvaluateCases(T value)
        {
            if (this.SwitchCases == null || this.SwitchCases.Count == 0)
            {
                base.Content = null;
            }
            else
            {
                foreach (ICase<T> item in this.SwitchCases)
                {
                    if (item.Value.Equals(value))
                    {
                        base.Content = item.Content;
                        return;
                    }
                }
                base.Content = null;
            }
        }
    }
}