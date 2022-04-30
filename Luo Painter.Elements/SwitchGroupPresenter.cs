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
    /// An collection of <see cref="GroupCase{GroupType, Type}"/> to help with XAML interop.
    /// </summary>
    public class GroupCaseCollection : DependencyObjectCollection { }


    /// <summary>
    /// <see cref="GroupCase{GroupType, Type}"/> is the value container for the <see cref="SwitchGroupPresenter{GroupType, Type}"/>.
    /// </summary>
    [ContentProperty(Name = nameof(Content))]
    public class GroupCase<GroupType, Type> : DependencyObject
        where GroupType : Enum
        where Type : Enum
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
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(GroupCase<GroupType, Type>), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="GroupValue"/> that this case represents. If it matches the <see cref="SwitchGroupPresenter.GroupValue"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        public GroupType GroupValue
        {
            get => (GroupType)base.GetValue(GroupValueProperty);
            set => base.SetValue(GroupValueProperty, value);
        }
        /// <summary> Identifies the <see cref="GroupValue"/> property. </summary>
        public static readonly DependencyProperty GroupValueProperty = DependencyProperty.Register(nameof(GroupType), typeof(GroupType), typeof(GroupCase<GroupType, Type>), new PropertyMetadata(default(GroupType)));

        /// <summary>
        /// Gets or sets the <see cref="Value"/> that this case represents. If it matches the <see cref="SwitchGroupPresenter.Value"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        public Type Value
        {
            get => (Type)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Type), typeof(GroupCase<GroupType, Type>), new PropertyMetadata(default(Type)));
    }


    /// <summary>
    /// The <see cref="SwitchGroupPresenter{GroupType, Type}"/> is a <see cref="ContentPresenter"/> which can allow a developer to mimic a <c>switch</c> statement within XAML.
    /// When provided a set of <see cref="GroupCase{GroupType, Type}"/>s and a <see cref="Value"/>, it will pick the matching <see cref="GroupCase{GroupType, Type}"/> with the corresponding <see cref="GroupCase.Value"/>.
    /// </summary>
    [ContentProperty(Name = nameof(SwitchCases))]
    public partial class SwitchGroupPresenter<GroupType, Type> : ContentPresenter
        where GroupType : Enum
        where Type : Enum
    {
        public GroupCaseCollection SwitchCases { get; } = new GroupCaseCollection();

        public void EvaluateCases(GroupType groupValue, Type value)
        {
            if (this.SwitchCases == null || this.SwitchCases.Count == 0)
            {
                base.Content = null;
            }
            else
            {
                foreach (GroupCase<GroupType, Type> item in this.SwitchCases)
                {
                    if (item.GroupValue.Equals(groupValue))
                    {
                        if (item.Value.Equals(default(Type)) || item.Value.Equals(value))
                        {
                            if (base.Content != item.Content) base.Content = item.Content;
                            return;
                        }
                    }
                }
                base.Content = null;
            }
        }
    }
}