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

        #region GammaTransfer
        GTAASlider,
        GTEASlider,
        GTOASlider,

        GTARSlider,
        GTERSlider,
        GTORSlider,

        GTAGSlider,
        GTEGSlider,
        GTOGSlider,

        GTABSlider,
        GTEBSlider,
        GTOBSlider,
        #endregion

        #region Effect
        // Temperature
        TemperatureSlider,
        TintSlider,
        // HighlightsAndShadows
        ShadowsSlider,
        HighlightsSlider,
        ClaritySlider,
        BlurSlider,
        // DirectionalBlur
        DirectionalBlurSlider,
        DirectionalBlurAngleSlider,
        // Shadow
        ShadowAmountSlider,
        ShadowOpacitySlider,
        ShadowXSlider,
        ShadowYSlider,
        // EdgeDetection
        EdgeDetectionSlider,
        EdgeDetectionBlurAmountSlider,
        // Emboss
        EmbossSlider,
        EmbossAngleSlider,
        // DisplacementLiquefaction
        DisplacementLiquefactionSizeSlider,
        DisplacementLiquefactionPressureSlider,
        // RippleEffect
        FrequencySlider,
        PhaseSlider,
        AmplitudeSlider,
        // HSB
        HSBHueSlider,
        HSBSaturationSlider,
        HSBBrightnessSlider,
        // Lighting
        LightDistanceSlider,
        LightAmbientSlider,
        LightAngleSlider,
        #endregion

        #region Geometry
        // GeometryStar
        StarPointsSlider,
        StarInnerRadiusSlider,
        // GeometryCog
        CogCountSlider,
        CogInnerRadiusSlider,
        CogToothSlider,
        CogNotchSlider,
        // GeometryCookie
        CookieInnerRadiusSlider,
        CookieSweepAngleSlider,
        #endregion

        #region Pattern
        // PatternGrid
        GridStrokeWidthSlider,
        GridColumnSpanSlider,
        GridRowSpanSlider,
        // PatternSpotted
        SpottedRadiusSlider,
        SpottedSpanSlider,
        SpottedFadeSlider,
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