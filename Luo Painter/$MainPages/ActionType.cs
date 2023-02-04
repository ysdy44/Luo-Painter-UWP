namespace Luo_Painter
{
    internal enum ActionType : int
    {
        /// <summary> Open Project </summary>
        File,
        /// <summary> Open Folder </summary>
        Folder,

        /// <summary> Add Empty Project </summary>
        Add,
        /// <summary> Add Image Project </summary>
        Image,

        DupliateShow,
        DupliateHide,
        DeleteShow,
        DeleteHide,
        SelectShow,
        SelectHide,
        SelectToMove,
        MoveHide,

        /// <summary> Dupliate Project </summary>
        Dupliate,
        /// <summary> Delete Project or Folder </summary>
        Delete,
        /// <summary> Move Project </summary>
        Move,

        /// <summary> New Folder </summary>
        New,
        /// <summary> Rename Project </summary>
        Rename,
        /// <summary> Open Local Folder </summary>
        Local,
    }
}