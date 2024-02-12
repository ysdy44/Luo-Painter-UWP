namespace Luo_Painter.UI
{
    public enum NumberPickerMode
    {
        None = 0,

        Case0 = 0,
        Case1 = 1,
        Case2 = 2,
        Case3 = 3,

        #region Transform
        // Move
        MoveX,
        MoveY,
        // Transform
        TransformX,
        TransformY,

        TransformWidth,
        TransformHeight,

        TransformSkew,
        TransformRotate,
        // FreeTransform
        FreeTransform0,
        FreeTransform1,

        FreeTransform2,
        FreeTransform3,

        FreeTransform4,
        FreeTransform5,

        FreeTransform6,
        FreeTransform7,
        #endregion

        // Layer
        OpacitySlider = -1,
        // View
        RadianSlider = -2,
        ScaleSlider = -3,
        // FloodTolerance
        SelectionFloodToleranceSlider = -4,
    }
}