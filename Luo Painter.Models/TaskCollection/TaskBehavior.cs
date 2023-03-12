namespace Luo_Painter.Models
{
    public enum TaskBehavior : byte
    {
        Dead,

        WaitingWork,
        Working,
        WorkingBeforeDead,
    }
}