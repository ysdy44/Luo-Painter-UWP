﻿namespace Luo_Painter.Models.Historys
{
    public class SetupHistory : IHistory
    {
        public HistoryMode Mode => HistoryMode.Setup;
        public HistoryPropertyMode PropertyMode => HistoryPropertyMode.None;
        public HistoryPropertyMode PropertyType => HistoryPropertyMode.None;

        public readonly System.Drawing.Size UndoParameter;
        public readonly System.Drawing.Size RedoParameter;

        public SetupHistory(int undoWidth, int undoHeight, int redoWidth, int redoHeight)
        {
            this.UndoParameter = new System.Drawing.Size(undoWidth, undoHeight);
            this.RedoParameter = new System.Drawing.Size(redoWidth, redoHeight);
        }

        public void Dispose()
        {
        }
    }
}