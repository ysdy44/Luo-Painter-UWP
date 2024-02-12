namespace Luo_Painter.UI
{
    public enum NumberPickerMode
    {
        None,

        // Layer
        OpacitySlider,

        // View
        RadianSlider,
        ScaleSlider,
        // FloodTolerance
        SelectionFloodToleranceSlider,

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
    }
}