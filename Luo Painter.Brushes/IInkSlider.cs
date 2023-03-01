namespace Luo_Painter.Brushes
{
    public interface IInkSlider
    {
        bool IsInkEnabled { get; set; }
        void ConstructInkSliderValue(InkPresenter presenter);
        void ConstructInkSlider(InkPresenter presenter);
    }
}