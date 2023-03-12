using System;

namespace Luo_Painter.Models
{
    public sealed class ProjectNew : ProjectBase
    {
        //@Static
        public static readonly ProjectNew New = new ProjectNew();
        private ProjectNew() : base(ProjectType.New)
        {
            //@Debug
            // Order by DateCreated
            this.DateCreated = DateTimeOffset.MaxValue;
        }
    }
}