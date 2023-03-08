using Luo_Painter.Elements;
using Luo_Painter.Options;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter
{
    public enum ContextAppBarDevice
    {
        None,
        Phone,
        Pad,
        PC,
        Hub,
    }

    [ContentProperty(Name = nameof(Content))]
    internal class OptionCase : DependencyObject, ICase<OptionType>
    {
        public object Content
        {
            get => (object)base.GetValue(ContentProperty);
            set => base.SetValue(ContentProperty, value);
        }
        /// <summary> Identifies the <see cref="Content"/> property. </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(OptionCase), new PropertyMetadata(null));

        public OptionType Value
        {
            get => (OptionType)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(OptionType), typeof(OptionCase), new PropertyMetadata(default(OptionType)));

        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }
    }

    [ContentProperty(Name = nameof(SwitchCases))]
    internal sealed class OptionSwitchPresenter : SwitchPresenter<OptionType> { }

    public sealed partial class DrawPage
    {

        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);
        private bool IsAlt => this.IsKeyDown(VirtualKey.Menu);
        private bool IsSpace => this.IsKeyDown(VirtualKey.Space);

        private bool IsCenter => this.IsCtrl || this.TransformCenterButton.IsChecked is true;
        private bool IsRatio => this.IsShift || this.TransformRatioButton.IsChecked is true;
        private bool IsSnap => this.TransformSnapButton.IsChecked is true;


        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private bool VisibilityToBooleanConverter(Visibility value) => value is Visibility.Visible;
        private bool ReverseVisibilityToBooleanConverter(Visibility value) => value is Visibility.Collapsed;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        public double SizeConverter(double value) => this.SizeRange.ConvertXToY(value);
        private double FontSizeConverter(double value) => this.SizeConverter(value) / 4 + 1;
        private string SizeToStringConverter(double value) => string.Format("{0:F}", this.SizeConverter(value));
        private double OpacityConverter(double value) => value / 100;
        private string OpacityToStringConverter(double value) => $"{(int)value} %";

        public double DockLeftConverter(bool value) => value ? -90 : 0;
        public double DockRightConverter(bool value) => value ? 90 : 0;

        private Visibility SymmetryIndexToVisibilityConverter(int value)
        {
            switch (value)
            {
                case 0: return Visibility.Collapsed;
                case 1: return Visibility.Collapsed;
                default: return Visibility.Visible;
            }
        }


        #region DependencyProperty: Device


        // GradientStopSelector
        private double ReverseDevicePhoneToWidth740Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                case ContextAppBarDevice.Pad: return 500;
                default: return 740;
            }
        }

        // InkSlider
        private double ReverseDevicePhoneToWidth300Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                default: return 300;
            }
        }

        // Slider 
        private double ReverseDevicePhoneToWidth260Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return double.NaN;
                default: return 260;
            }
        }

        // Slider Slider 
        private double ReverseDevicePhoneAndPadToWidth260Converter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad: return double.NaN;
                default: return 260;
            }
        }
        private Orientation ReverseDevicePhoneAndPadToOrientationHorizontalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }

        // Slider Slider Slider Slider 
        private Orientation DevicePhoneToOrientationVerticalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }
        private Orientation DeviceHubToOrientationHorizontalConverter(ContextAppBarDevice value)
        {
            switch (value)
            {
                case ContextAppBarDevice.Phone:
                case ContextAppBarDevice.Pad:
                case ContextAppBarDevice.PC: return Orientation.Vertical;
                default: return Orientation.Horizontal;
            }
        }


        /// <summary> Gets or set the device for <see cref="DrawPage"/>. </summary>
        public ContextAppBarDevice Device
        {
            get => (ContextAppBarDevice)base.GetValue(DeviceProperty);
            set => base.SetValue(DeviceProperty, value);
        }
        /// <summary> Identifies the <see cref = "DrawPage.Device" /> dependency property. </summary>
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(ContextAppBarDevice), typeof(DrawPage), new PropertyMetadata(ContextAppBarDevice.None, (sender, e) =>
        {
            DrawPage control = (DrawPage)sender;

            if (e.NewValue is ContextAppBarDevice value)
            {
                switch (value)
                {
                    case ContextAppBarDevice.Phone:
                        VisualStateManager.GoToState(control, "Phone", false);
                        break;
                    case ContextAppBarDevice.Pad:
                        VisualStateManager.GoToState(control, "Pad", false);
                        break;
                    case ContextAppBarDevice.PC:
                        VisualStateManager.GoToState(control, "PC", false);
                        break;
                    case ContextAppBarDevice.Hub:
                        VisualStateManager.GoToState(control, "Hub", false);
                        break;
                    default:
                        break;
                }
            }
        }));


        #endregion

    }
}