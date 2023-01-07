using System.Collections.Generic;
using Windows.UI.Core;

namespace Luo_Painter.Elements
{
    public enum CursorMode
    {
        None = 0,
        OneWay = 1,
        OneTime = 2,
    }

    public sealed class CoreCursorDictionary
    {
        public CoreCursor this[CoreCursorType key] => this.Cursors[key];
        private readonly IReadOnlyDictionary<CoreCursorType, CoreCursor> Cursors = new Dictionary<CoreCursorType, CoreCursor>
        {
            [CoreCursorType.Arrow] = new CoreCursor(CoreCursorType.Arrow, 0),
            [CoreCursorType.Cross] = new CoreCursor(CoreCursorType.Cross, 0),
            [CoreCursorType.Custom] = null,

            [CoreCursorType.Hand] = new CoreCursor(CoreCursorType.Hand, 0),
            [CoreCursorType.Help] = new CoreCursor(CoreCursorType.Help, 0),
            [CoreCursorType.IBeam] = new CoreCursor(CoreCursorType.IBeam, 0),

            [CoreCursorType.SizeAll] = new CoreCursor(CoreCursorType.SizeAll, 0),
            [CoreCursorType.SizeNortheastSouthwest] = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0),
            [CoreCursorType.SizeNorthSouth] = new CoreCursor(CoreCursorType.SizeNorthSouth, 0),
            [CoreCursorType.SizeNorthwestSoutheast] = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 0),
            [CoreCursorType.SizeWestEast] = new CoreCursor(CoreCursorType.SizeWestEast, 0),

            [CoreCursorType.UniversalNo] = new CoreCursor(CoreCursorType.UniversalNo, 0),
            [CoreCursorType.UpArrow] = new CoreCursor(CoreCursorType.UpArrow, 0),
            [CoreCursorType.Wait] = new CoreCursor(CoreCursorType.Wait, 0),
            [CoreCursorType.Pin] = new CoreCursor(CoreCursorType.Pin, 0),
            [CoreCursorType.Person] = new CoreCursor(CoreCursorType.Person, 0),
        };
    }
}