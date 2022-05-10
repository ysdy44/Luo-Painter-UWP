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
    /// An collection of <see cref="IGroupCase{GroupType, Type}"/> to help with XAML interop.
    /// </summary>
    public sealed class GroupCaseCollection : DependencyObjectCollection { }

    /// <summary>
    /// <see cref="IGroupCase{GroupType, Type}"/> is the value container for the <see cref="SwitchGroupPresenter{GroupType, Type}"/>.
    /// </summary>
    public interface IGroupCase<GroupType, Type>
        where GroupType : Enum
        where Type : Enum
    {
        /// <summary>
        /// Gets the Content to display when this case is active.
        /// </summary>
        object Content { get; }

        /// <summary>
        /// Gets the <see cref="GroupValue"/> that this case represents. If it matches the <see cref="SwitchGroupPresenter.GroupValue"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        GroupType GroupValue { get; }

        /// <summary>
        /// Gets the <see cref="Value"/> that this case represents. If it matches the <see cref="SwitchGroupPresenter.Value"/> this case's <see cref="Content"/> will be displayed in the presenter.
        /// </summary>
        Type Value { get; }

        /// <summary> The current tool becomes the active case. </summary>
        void OnNavigatedTo();

        /// <summary> The current page does not become an active case. </summary>
        void OnNavigatedFrom();
    }

    /// <summary>
    /// The <see cref="SwitchGroupPresenter{GroupType, Type}"/> is a <see cref="ContentPresenter"/> which can allow a developer to mimic a <c>switch</c> statement within XAML.
    /// When provided a set of <see cref="IGroupCase{GroupType, Type}"/>s and a <see cref="Value"/>, it will pick the matching <see cref="IGroupCase{GroupType, Type}"/> with the corresponding <see cref="IGroupCase.Value"/>.
    /// </summary>
    [ContentProperty(Name = nameof(SwitchCases))]
    public class SwitchGroupPresenter<GroupType, Type> : ContentPresenter
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
                foreach (IGroupCase<GroupType, Type> item in this.SwitchCases)
                {
                    if (item.GroupValue.Equals(groupValue))
                    {
                        if (item.Value.Equals(default(Type)) || item.Value.Equals(value))
                        {
                            base.Content = item.Content;
                            return;
                        }
                    }
                }
                base.Content = null;
            }
        }
    }
}