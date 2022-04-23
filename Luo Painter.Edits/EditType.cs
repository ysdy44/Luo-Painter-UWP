namespace Luo_Painter.Edits
{
    public enum EditType
    {
        None,

        // Edit
        Cut,
        Duplicate,
        Copy,
        Paste,

        Clear,

        Extract,
        Merge,

        // Group
        Group,
        Ungroup,

        Release,

        // Select
        All,
        Deselect,

        Invert,

        // Combine
        Union,
        Exclude,
        Xor,
        Intersect,

        ExpandStroke,
    }
}