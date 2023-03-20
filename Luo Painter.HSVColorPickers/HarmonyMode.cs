namespace Luo_Painter.HSVColorPickers
{
    public enum HarmonyMode
    {
        None,

        HasPoint1 = 8,
        HasPoint2 = 16,
        HasPoint3 = 32,

        HasPointToPoint1 = HasPoint1,
        HasPointToPoint2 = HasPoint2,
        HasPointToPoint3 = HasPoint2,
        HasPointToPoint4 = HasPoint3,

        Complementary = 1 | HasPoint1 | HasPointToPoint1,

        SplitComplementary = 1 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,
        Analogous = 2 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,

        Triadic = 3 | HasPoint1 | HasPoint2 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3,
        Tetradic = 1 | HasPoint1 | HasPoint2 | HasPoint3 | HasPointToPoint1 | HasPointToPoint2 | HasPointToPoint3 | HasPointToPoint4,
    }
}