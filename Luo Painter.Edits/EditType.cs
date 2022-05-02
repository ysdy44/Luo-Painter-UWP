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
        Remove,

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
        Grow,
        Shrink,

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