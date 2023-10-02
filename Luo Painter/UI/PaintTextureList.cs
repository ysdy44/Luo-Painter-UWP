using System.Collections.Generic;

namespace Luo_Painter.Controls
{
    public sealed class PaintTextureList : List<PaintTexture>
    {
        public int IndexOf(string path)
        {
            if (string.IsNullOrEmpty(path)) return -1;

            for (int i = 0; i < base.Count; i++)
            {
                PaintTexture item = base[i];
                if (item.Path == path)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}