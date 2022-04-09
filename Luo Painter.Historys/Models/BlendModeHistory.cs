using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.Historys.Models
{
    public class BlendModeHistory : IHistory<BlendEffectMode?>
    {
        public HistoryType Type => HistoryType.BlendMode;
        public string Id { set; get; }
        public BlendEffectMode? UndoParameter { set; get; }
        public BlendEffectMode? RedoParameter { set; get; }
        public void Dispose()
        {
        }
    }
}