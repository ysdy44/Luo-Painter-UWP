using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class AlignmentGridPage : Page
    {
        public AlignmentGridPage()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                bool result = this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);

                this.RebuildRun.Text = $"{result}";
                this.ColumnRun.Text = $"{this.AlignmentGrid.Column}";
                this.RowRun.Text = $"{this.AlignmentGrid.Row}";
                this.WidthRun.Text = $"{this.AlignmentGrid.Width} ({e.NewSize.Width})";
                this.HeightRun.Text = $"{this.AlignmentGrid.Height} ({e.NewSize.Height})";
            };
        }
    }
}