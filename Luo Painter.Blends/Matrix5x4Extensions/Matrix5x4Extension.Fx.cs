using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Luo_Painter.Blends
{
    public static partial class Matrix5x4Extension
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 Threshold(float N = 0) // -1~1
        {
            return new Matrix5x4
            { 
                #pragma warning disable IDE0055
                //         R            G             B              A
                M11 = Red, M12 = Red, M13 = Red, M14 = 0, // Red
                M21 = Green, M22 = Green, M23 = Green, M24 = 0, // Green
                M31 = Blue, M32 = Blue, M33 = Blue, M34 = 0, // Blue
                M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
                M51 = -N, M52 = -N, M53 = -N, M54 = 0 // Offset
                #pragma warning restore IDE0055
            };
        }

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
        public static Matrix5x4 Exposure(float exposure = 1) => new Matrix5x4 // 0~2
        {             
            #pragma warning disable IDE0055
            //         R            G             B              A
            M11 = exposure, M12 = 0, M13 = 0, M14 = 0, // Red
            M21 = 0, M22 = exposure, M23 = 0, M24 = 0, // Green
            M31 = 0, M32 = 0, M33 = exposure, M34 = 0, // Blue
            M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
            M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
            #pragma warning restore IDE0055
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 Contrast(float contrast = 1) // 0~2
        {
            float offset = 0.5f - contrast / 2;
            return new Matrix5x4
            { 
                #pragma warning disable IDE0055
                //         R            G             B              A
                M11 = contrast, M12 = 0, M13 = 0, M14 = 0, // Red
                M21 = 0, M22 = contrast, M23 = 0, M24 = 0, // Green
                M31 = 0, M32 = 0, M33 = contrast, M34 = 0, // Blue
                M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
                M51 = offset, M52 = offset, M53 = offset, M54 = 0 // Offset
                #pragma warning restore IDE0055
            };
        }

    }
}