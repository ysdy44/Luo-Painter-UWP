using System;

namespace Luo_Painter.Models
{
    public interface ProjectBase
    {
        ProjectType Type { get; }
        bool IsEnabled { get; }
        string Path { get; }
        string Name { get; }
        string DisplayName { get; }
        DateTimeOffset DateCreated { get; }

        string Thumbnail { get; }

        void Enable(ProjectType types);
    }
}