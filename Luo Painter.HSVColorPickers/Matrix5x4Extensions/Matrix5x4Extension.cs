using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.Blends
{
    /// <summary>
    /// Provides extended methods for <see cref="Matrix5x4"/> to build <see cref="ColorMatrixEffect"/>.
    /// <para/>
    /// <see cref="EffectChannelSelect.Red"/> = 
    /// (S.R * M11) + (S.G * M21) + (S.B * M31) + (S.A * M41) + M51; 
    /// <para/>
    /// <see cref="EffectChannelSelect.Green"/> = 
    /// (S.R * M12) + (S.G * M22) + (S.B * M32) + (S.A * M42) + M52; 
    /// <para/>
    /// <see cref="EffectChannelSelect.Blue"/> = 
    /// (S.R * M13) + (S.G * M23) + (S.B * M33) + (S.A * M43) + M53; 
    /// <para/>
    /// <see cref="EffectChannelSelect.Alpha"/> = 
    /// (S.R * M14) + (S.G * M24) + (S.B * M34) + (S.A * M44) + M54; 
    /// </summary>
    public static partial class Matrix5x4Extension
    {

        const float Gray = 1f / 3f;
        const float Red = 0.3086f;
        const float Green = 0.6094f;
        const float Blue = 0.0820f;

        public static readonly Matrix5x4 Identity = new Matrix5x4
        { 
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = 1, M12 = 0, M13 = 0, M14 = 0, // Red
            M21 = 0, M22 = 1, M23 = 0, M24 = 0, // Green
            M31 = 0, M32 = 0, M33 = 1, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        public static readonly Matrix5x4 Invert = new Matrix5x4
        { 
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = -1, M12 = 0, M13 = 0, M14 = 0, // Red
            M21 = 0, M22 = -1, M23 = 0, M24 = 0, // Green
            M31 = 0, M32 = 0, M33 = -1, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 1, M52 = 1, M53 = 1, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        public static readonly Matrix5x4 Grayscale = new Matrix5x4
        { 
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = Gray, M12 = Gray, M13 = Gray, M14 = 0, // Red
            M21 = Gray, M22 = Gray, M23 = Gray, M24 = 0, // Green
            M31 = Gray, M32 = Gray, M33 = Gray, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        public static readonly Matrix5x4 Decolor = new Matrix5x4
        { 
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = Red, M12 = Red, M13 = Red, M14 = 0, // Red
            M21 = Green, M22 = Green, M23 = Green, M24 = 0, // Green
            M31 = Blue, M32 = Blue, M33 = Blue, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

    }
}