using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Represents a control based on a <see cref="Slider"/>,
    /// It has four events : 
    /// <see cref="ThumbSlider.ValueChangedStarted"/>, 
    /// <see cref="ThumbSlider.ValueChangedDelta"/>, 
    /// <see cref="ThumbSlider.ValueChangedCompleted"/> 
    /// and <see cref="ThumbSlider.ValueChangedUnfocused"/>.
    /// </summary>
    public sealed partial class ThumbSlider : Slider
    {
        //@Delegate
        /// <summary> Occurs when the value changed starts. </summary>
        public event EventHandler ValueChangedStarted;
        /// <summary> Occurs when value changed. </summary>
        public event RangeBaseValueChangedEventHandler ValueChangedDelta;
        /// <summary> Occurs when the value changed is complete. </summary>
        public event EventHandler ValueChangedCompleted;
        /// <summary> Occurs when the value changed when Unfocused. </summary>
        public event RangeBaseValueChangedEventHandler ValueChangedUnfocused;

        bool IsStarted = false;
        bool IsCompleted = true;

        VisualStateGroup CommonStates;
        VisualState Normal;
        VisualState PointerOver;
        VisualState Pressed;
        VisualState Disabled;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //VisualStateGroup
            if (this.CommonStates != null) this.CommonStates.CurrentStateChanged -= this.CommonStates_CurrentStateChanged;
            this.CommonStates = base.GetTemplateChild(nameof(CommonStates)) as VisualStateGroup;
            if (this.CommonStates != null) this.CommonStates.CurrentStateChanged += this.CommonStates_CurrentStateChanged;
            //VisualState
            this.Normal = base.GetTemplateChild(nameof(Normal)) as VisualState;
            this.PointerOver = base.GetTemplateChild(nameof(PointerOver)) as VisualState;
            this.Pressed = base.GetTemplateChild(nameof(Pressed)) as VisualState;
            this.Disabled = base.GetTemplateChild(nameof(Disabled)) as VisualState;

            base.ValueChanged -= this.ValueChangedByKeyboardCore;//Remove Delegate
            base.ValueChanged += this.ValueChangedByKeyboardCore;//Add Delegate
        }

        private void CommonStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.OldState != this.Pressed && e.NewState == this.Pressed)
            {
                this.IsStarted = true;
                this.ValueChangedStarted?.Invoke(sender, new EventArgs());//Delegate
                base.ValueChanged += this.ValueChangedDelta;//Add Delegate
            }
            else
            {
                this.IsStarted = false;
            }

            if (e.OldState == this.Pressed && e.NewState != this.Pressed)
            {
                this.IsCompleted = true;
                this.ValueChangedCompleted?.Invoke(sender, new EventArgs());//Delegate
                base.ValueChanged -= this.ValueChangedDelta;//Remove Delegate
            }
            else
            {
                this.IsCompleted = false;
            }
        }

        private void ValueChangedByKeyboardCore(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.IsStarted) return;
            if (base.FocusState == FocusState.Unfocused) return;

            this.ValueChangedUnfocused?.Invoke(sender, e);//Delegate
        }

    }
}