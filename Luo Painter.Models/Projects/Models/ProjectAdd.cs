﻿using System;

namespace Luo_Painter.Models.Projects
{
    public sealed class ProjectAdd : Project
    {
        //@Static
        public static readonly ProjectAdd Add = new ProjectAdd();
        private ProjectAdd() : base(ProjectType.Add)
        {
            //@Debug
            // Order by DateCreated
            this.DateCreated = DateTimeOffset.MaxValue;
        }
    }
}