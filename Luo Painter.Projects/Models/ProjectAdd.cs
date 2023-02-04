using System;

namespace Luo_Painter.Projects.Models
{
    public sealed class ProjectNone : Project
    {
        //@Static
        public static readonly ProjectNone Add = new ProjectNone();
        private ProjectNone() : base(ProjectType.Add)
        {
            //@Debug
            // Order by DateCreated
            this.DateCreated = DateTimeOffset.MaxValue;
        }
    }
}