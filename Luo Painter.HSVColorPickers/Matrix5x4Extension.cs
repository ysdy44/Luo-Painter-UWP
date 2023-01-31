using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Luo_Painter.HSVColorPickers
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
    public static class Matrix5x4Extension
    {

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 ColorMatch(Vector4 sourceHdr, Vector4 destinationHdr) => new Matrix5x4
        {             
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = sourceHdr.X, M12 = 0, M13 = 0, M14 = 0, // Red
            M21 = 0, M22 = sourceHdr.Y, M23 = 0, M24 = 0, // Green
            M31 = 0, M32 = 0, M33 = sourceHdr.Z, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = sourceHdr.W, // Alpha
            M51 = destinationHdr.X, M52 = destinationHdr.Y, M53 = destinationHdr.Z, M54 = destinationHdr.W // Offset
            #pragma warning restore IDE0055
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 Brightness(float brightness = 0) => new Matrix5x4 // -1~1
        {             
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = 1, M12 = 0, M13 = 0, M14 = 0, // Red
            M21 = 0, M22 = 1, M23 = 0, M24 = 0, // Green
            M31 = 0, M32 = 0, M33 = 1, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = brightness, M52 = brightness, M53 = brightness, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 WhiteBlack(float white = 1, float black = 0) => new Matrix5x4
        {             
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = white, M12 = black, M13 = black, M14 = 0, // Red
            M21 = black, M22 = white, M23 = black, M24 = 0, // Green
            M31 = black, M32 = black, M33 = white, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 Exposure(float exposure = 1) => Matrix5x4Extension.WhiteBlack(exposure, 0); // 0~2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 Saturation(float saturation = 1) => Matrix5x4Extension.WhiteBlack((1 + saturation + saturation) / 3, (1 - saturation) / 3); // 0~2

    }
}