namespace Luo_Painter.Models
{
    public enum ActionType
    {
        /// <summary> Open Project </summary>
        OpenProject,
        /// <summary> Open Folder </summary>
        OpenFolder,

        /// <summary> New Empty Project </summary>
        NewProject,
        /// <summary> New Image Project </summary>
        NewImage,
        /// <summary> New Folder </summary>
        NewFolder,

        DupliateShow,
        DupliateHide,
        DeleteShow,
        DeleteHide,
        SelectShow,
        SelectHide,
        SelectToMove,
        MoveHide,

        /// <summary> Dupliate Project </summary>
        DupliateProject,
        /// <summary> Delete Project or Folder </summary>
        DeleteProject,
        /// <summary> Move Project </summary>
        MoveProject,

        /// <summary> Rename Project </summary>
        RenameProject,
        /// <summary> Open Local Folder </summary>
        LocalFolder,
    }
}