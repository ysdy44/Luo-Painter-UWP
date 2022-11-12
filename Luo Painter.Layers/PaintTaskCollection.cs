using System.Collections.Generic;

namespace Luo_Painter.Layers
{
    public enum PaintTaskState : byte
    {
        Finished,

        Painting,
        Painted,
    }

    public enum PaintTaskBehavior : byte
    {
        Dead,

        WaitingWork,
        Working,
        WorkingBeforeDead,
    }

    public sealed class PaintTaskCollection : List<StrokeSegment>
    {
        public PaintTaskState State { get; set; } = PaintTaskState.Finished;
        public PaintTaskBehavior GetBehavior()
        {
            if (base.Count is 0)
            {
                switch (this.State)
                {
                    case PaintTaskState.Painting:
                        return PaintTaskBehavior.WaitingWork;
                    default:
                        return PaintTaskBehavior.Dead;
                }
            }
            else
            {
                switch (this.State)
                {
                    case PaintTaskState.Painting:
                        return PaintTaskBehavior.Working;
                    default:
                        return PaintTaskBehavior.WorkingBeforeDead;
                }
            }
        }
    }
}