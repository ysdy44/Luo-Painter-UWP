namespace Luo_Painter.HSVColorPickers
{
    public enum HarmonyMode : byte
    {
        None,

        HasPoint1 = 16 | 4,
        HasPoint2 = 32 | 4,
        HasPoint3 = 64 | 4,

        HasPointToPoint1 = 16 | 8,
        HasPointToPoint2 = 32 | 8,
        HasPointToPoint3 = 64 | 8,
        HasPointToPoint4 = 128 | 8,

        Complementary = 1 | HasPoint1 | HasPointToPoint1,

        SplitComplementary = 1 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,
        Analogous = 2 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,

        Triadic = 3 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,
        Tetradic = 1 | HasPoint1 | HasPoint2 | HasPoint3 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3 | HasPointToPoint4,
    }
}