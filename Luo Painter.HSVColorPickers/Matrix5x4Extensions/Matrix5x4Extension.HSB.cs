using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Runtime.CompilerServices;

namespace Luo_Painter.Blends
{
    public static partial class Matrix5x4Extension
    {

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
        public static Matrix5x4 Saturation(float saturation = 1) // 0~2
        {
            float white = (1 + saturation + saturation) / 3;
            float black = (1 - saturation) / 3;

            return new Matrix5x4
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 HueRotate(float angle = 0) // 0~3
        {
            float x = Math.Clamp(Math.Abs(angle - 1.5f) - 0.5f, 0, 1); // ＼_／
            float y = 1 - Math.Clamp(Math.Abs(angle - 1), 0, 1); // ／＼_
            float z = 1 - Math.Clamp(Math.Abs(angle - 2), 0, 1); // _／＼

            return new Matrix5x4
            { 
                 #pragma warning disable IDE0055
                 //         R            G             B              A
                 M11 = x, M12 = z, M13 = y, M14 = 0, // Red
                 M21 = y, M22 = x, M23 = z, M24 = 0, // Green
                 M31 = z, M32 = y, M33 = x, M34 = 0, // Blue
                 M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
                 M51 = 0, M52 = 0, M53 = 0, M54 = 0 // Offset
                 #pragma warning restore IDE0055
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 HSB(float angle = 0, float saturation = 1, float brightness = 0) // 0~3 // 0~2 // -1~1
        {
            float x = Math.Clamp(Math.Abs(angle - 1.5f) - 0.5f, 0, 1); // ＼_／
            float y = 1 - Math.Clamp(Math.Abs(angle - 1), 0, 1); // ／＼_
            float z = 1 - Math.Clamp(Math.Abs(angle - 2), 0, 1); // _／＼

            float white = (1 + saturation + saturation) / 3;
            float black = (1 - saturation) / 3;

            float xs = x * white + (1 - x) * black;
            float ys = y * white + (1 - y) * black;
            float zs = z * white + (1 - z) * black;

            return new Matrix5x4
            { 
                 #pragma warning disable IDE0055
                 //         R            G             B              A
                 M11 = xs, M12 = zs, M13 = ys, M14 = 0, // Red
                 M21 = ys, M22 = xs, M23 = zs, M24 = 0, // Green
                 M31 = zs, M32 = ys, M33 = xs, M34 = 0, // Blue
                 M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
                 M51 = brightness, M52 = brightness, M53 = brightness, M54 = 0 // Offset
                 #pragma warning restore IDE0055
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5x4 HSB(float angle = 0, float saturation = 1, float brightness = 0, int split0 = 0, int split1 = 1, int split2 = 2, int split3 = 3) // 0~3 // 0~2 // -1~1
        {
            float x = Math.Clamp(Math.Abs(angle - 1.5f) - 0.5f, 0, 1); // ＼_／
            float y = 1 - Math.Clamp(Math.Abs(angle - 1), 0, 1); // ／＼_
            float z = 1 - Math.Clamp(Math.Abs(angle - 2), 0, 1); // _／＼

            float white = (1 + saturation + saturation) / 3;
            float black = (1 - saturation) / 3;

            float xs = x * white + (1 - x) * black;
            float ys = y * white + (1 - y) * black;
            float zs = z * white + (1 - z) * black;

            return new Matrix5x4
            { 
                 #pragma warning disable IDE0055
                 //         R            G             B              A
                 M11 = xs, M12 = zs, M13 = ys, M14 = 0, // Red
                 M21 = ys, M22 = xs, M23 = zs, M24 = 0, // Green
                 M31 = zs, M32 = ys, M33 = xs, M34 = 0, // Blue
                 M41 = 0, M42 = 0, M43 = 0, M44 = 1, // Alpha
                 M51 = brightness, M52 = brightness, M53 = brightness, M54 = 0 // Offset
                 #pragma warning restore IDE0055
            };
        }

    }
}