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
        Flatten,

        // Group
        Group,
        Ungroup,

        Release,

        // Select
        All,
        Deselect,
        Invert, 
        Pixel,

        Feather,
        Transform,

        // Combine
        Union,
        Exclude,
        Xor,
        Intersect,

        ExpandStroke,

        // Setup
        Crop,

        Stretch,

        FlipHorizontal,
        FlipVertical,

        LeftTurn,
        RightTurn,
        OverTurn,

    }
}