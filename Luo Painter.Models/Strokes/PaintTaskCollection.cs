using System;
using System.Collections.Generic;

namespace Luo_Painter.Models
{
    public enum TaskState : byte
    {
        Finished,

        Painting,
        Painted,
    }

    public enum TaskBehavior : byte
    {
        Dead,

        WaitingWork,
        Working,
        WorkingBeforeDead,
    }

    public sealed partial class TaskCollection : List<StrokeSegment>, IDisposable
    {
        public TaskState State { get; set; } = TaskState.Finished;
        public TaskBehavior GetBehavior()
        {
            if (base.Count is 0)
            {
                switch (this.State)
                {
                    case TaskState.Painting:
                        return TaskBehavior.WaitingWork;
                    default:
                        return TaskBehavior.Dead;
                }
            }
            else
            {
                switch (this.State)
                {
                    case TaskState.Painting:
                        return TaskBehavior.Working;
                    default:
                        return TaskBehavior.WorkingBeforeDead;
                }
            }
        }
    }
}