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
    /// An collection of <see cref="Case"/> to help with XAML interop.
    /// </summary>
    public class CaseCollection : DependencyObjectCollection { }


    /// <summary>
    /// <see cref="Case"/> is the value container for the <see cref="SwitchPresenter"/>.
    /// </summary>
    [ContentProperty(Name = nameof(Content))]
    public class Case<T> : DependencyObject
        where T : Enum
    {
        /// <summary>
        /// Gets or sets the Content to display when this case is active.
        /// </summary>
        public object Content
        {
            get => (object)base.GetValue(ContentProperty);
            set => base.SetValue(ContentProperty, value);
        }
        /// <summary> Identifies the <see cref="Content"/> property. </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(Case<T>), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Value"/> that this case represents. If it matches the <see cref="SwitchPresenter.Value"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        public T Value
        {
            get => (T)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(T), typeof(Case<T>), new PropertyMetadata(default(T)));
    }


    /// <summary>
    /// The <see cref="SwitchPresenter"/> is a <see cref="ContentPresenter"/> which can allow a developer to mimic a <c>switch</c> statement within XAML.
    /// When provided a set of <see cref="Case"/>s and a <see cref="Value"/>, it will pick the matching <see cref="Case"/> with the corresponding <see cref="Case.Value"/>.
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
                foreach (Case<T> item in this.SwitchCases)
                {
                    if (item.Value.Equals(value))
                    {
                        base.Content = item.Content;
                        break;
                    }
                }
            }
        }
    }
}