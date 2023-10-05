using System.Threading.Tasks;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class ClickEyedropper : Eyedropper
    {
        //@Task
        private TaskCompletionSource<Color> TaskSource;
        public ClickEyedropper() : base()
        {
            this.InitializeComponent();
            this.Content.PointerMoved += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.Move(e.GetCurrentPoint(this).Position);
            };

            this.Content.Tapped += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.TrySetResult();
                this.Dispose();
            };
            this.Content.PointerReleased += (s, e) =>
            {
                if (this.Pixels is null) return;

                this.TrySetResult();
                this.Dispose();
            };
        }
        public void TrySetResult()
        {
            if (this.TaskSource != null && this.TaskSource.Task.IsCanceled == false)
            {
                this.TaskSource.TrySetResult(this.Color);
            }
        }
        public async Task<Color> OpenAsync()
        {
            this.TaskSource = new TaskCompletionSource<Color>();

            Color resultcolor = await this.TaskSource.Task;
            this.TaskSource = null;
            return resultcolor;
        }
        public void Dispose()
        {
            this.TaskSource?.TrySetCanceled();
            this.TaskSource = null;
        }
    }
}