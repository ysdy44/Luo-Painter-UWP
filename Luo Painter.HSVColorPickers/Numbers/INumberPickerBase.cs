namespace Luo_Painter.HSVColorPickers
{
    public interface INumberPickerBase
    {
        //@Delegate
        event NumberChangedHandler ValueChanging;
        event NumberChangedHandler ValueChanged;

        bool IsNegative { get; }
        string Absnumber { get; }

        double Minimum { get; }
        double Maximum { get; }

        string Unit { get; }

        // UI
        void Close();
        void Open();

        void Construct(INumberBase number);

        bool IsZero();
        bool InRange();
        double ToValue();
        string ToString();
    }
}