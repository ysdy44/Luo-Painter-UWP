﻿using Luo_Painter.Edits;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class SetupButton : StackPanel
    {
        //@Delegate
        public event EventHandler<EditType> ItemClick
        {
            remove => this.EditTypeCommand.Click -= value;
            add => this.EditTypeCommand.Click += value;
        }

        //@Construct
        public SetupButton()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}