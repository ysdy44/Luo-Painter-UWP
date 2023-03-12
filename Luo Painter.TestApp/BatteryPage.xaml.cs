using System;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Power;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class BatteryCollection : ObservableCollection<BatteryReport>
    {
        /// <summary>
        /// <see cref="Battery.ReportUpdated"/>
        /// </summary>
        public event TypedEventHandler<Battery, object> ReportUpdated
        {
            remove => Battery.AggregateBattery.ReportUpdated -= value;
            add => Battery.AggregateBattery.ReportUpdated += value;
        }

        public BatteryCollection()
        {
            this.Aggregate();
            this.Individual();
        }  
        
        /// <summary>
        /// Request Aggregate Battery Report
        /// </summary>
        public void Aggregate()
        {
            Battery battery = Battery.AggregateBattery;
            string id = battery.DeviceId;
            base.Add(battery.GetReport());
        }

        /// <summary>
        /// Request Individual Battery Reports
        /// </summary>
        public async void Individual()
        {
            foreach (DeviceInformation device in await DeviceInformation.FindAllAsync(Battery.GetDeviceSelector()))
            {
                try
                {
                    string id = device.Id;
                    Battery battery = await Battery.FromIdAsync(id);
                    base.Add(battery.GetReport());
                }
                catch { }
            }
        }
    }

    public sealed partial class BatteryPage : Page
    {
        public BatteryPage()
        {
            this.InitializeComponent();
        }
    }
}