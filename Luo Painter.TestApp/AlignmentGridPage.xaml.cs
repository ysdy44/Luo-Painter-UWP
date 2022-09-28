using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class AlignmentGridPage : Page
    {
        public AlignmentGridPage()
        {
            this.InitializeComponent();
            this.Canvas1.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                bool result = this.AlignmentGrid1.RebuildWithInterpolation(e.NewSize);

                this.RectangleGeometry1.Rect = new Rect(default, e.NewSize);

                this.RebuildRun1.Text = $"{result}";
                this.ColumnRun1.Text = $"{this.AlignmentGrid1.Column}";
                this.RowRun1.Text = $"{this.AlignmentGrid1.Row}";
                this.WidthRun1.Text = $"{this.AlignmentGrid1.Width} ({e.NewSize.Width})";
                this.HeightRun1.Text = $"{this.AlignmentGrid1.Height} ({e.NewSize.Height})";
            };
            this.Canvas2.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                bool result = this.AlignmentGrid2.RebuildWithInterpolation(e.NewSize);
            
                this.RectangleGeometry2.Rect = new Rect(default, e.NewSize);

                this.RebuildRun2.Text = $"{result}";
                this.ColumnRun2.Text = $"{this.AlignmentGrid2.Column}";
                this.RowRun2.Text = $"{this.AlignmentGrid2.Row}";
                this.WidthRun2.Text = $"{this.AlignmentGrid2.Width} ({e.NewSize.Width})";
                this.HeightRun2.Text = $"{this.AlignmentGrid2.Height} ({e.NewSize.Height})";
            };
        }
    }
}