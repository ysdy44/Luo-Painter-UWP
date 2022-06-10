using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public bool IsometricFillCircle(
            Color color,
            Vector2 position,
            Vector2 targetPosition,
            float pressure = 1f,
            float targetPressure = 1f,
            float size = 22f,
            float spacing = 0.25f,
            BitmapType type = BitmapType.Source,
            bool isCopy = false
            )
        {
            // Length
            float length = Vector2.Distance(position, targetPosition);

            float sizePressure = size * pressure;
            float distance = spacing * sizePressure; // distance is spacingSizePressure

            if (distance > length) return false;
            // 

            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                if (isCopy) ds.Blend = CanvasBlend.Copy;

                ds.FillCircle(position, sizePressure, color);

                while (distance < length)
                {
                    // Smooth
                    float smooth = distance / length;
                    float smoothR = 1 - smooth;

                    float sizePressureSmooth = size * (smooth * pressure + smoothR * targetPressure); // sizePressure
                    Vector2 positionSmooth = smooth * position + smoothR * targetPosition;

                    distance += spacing * sizePressureSmooth; // distance
                    //

                    ds.FillCircle(positionSmooth, sizePressureSmooth, color);
                }

                return true;
            }
        }


        public bool IsometricErasingDry(
            Vector2 position, Vector2 targetPosition,
            float pressure = 1f,
            float targetPressure = 1f,
            float size = 22f,
            float spacing = 0.25f
            ) => this.IsometricFillCircle(Colors.Transparent, position, targetPosition, pressure, targetPressure, size, spacing, BitmapType.Source, true);

        public bool IsometricErasingWet(
            Vector2 position, Vector2 targetPosition,
            float pressure = 1f,
            float targetPressure = 1f,
            float size = 22f,
            float spacing = 0.25f
            ) => this.IsometricFillCircle(Colors.White, position, targetPosition, pressure, targetPressure, size, spacing, BitmapType.Temp);


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public bool IsometricDrawShaderBrushEdgeHardness(
            byte[] shaderCode,
            Vector4 colorHdr,
            Vector2 position,
            Vector2 targetPosition,
            float pressure = 1f,
            float targetPressure = 1f,
            float size = 22f,
            float spacing = 0.25f,
            int hardness = 0,
            BitmapType type = BitmapType.Source
            )
        {
            // Length
            float length = Vector2.Distance(position, targetPosition);

            float sizePressure = size * pressure;
            float distance = spacing * sizePressure; // distance is spacingSizePressure

            if (distance > length) return false;
            // 

            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = 1f,
                        ["radius"] = sizePressure,
                        ["targetPosition"] = position,
                        ["color"] = colorHdr
                    }
                });

                while (distance < length)
                {
                    // Smooth
                    float smooth = distance / length;
                    float smoothR = 1 - smooth;

                    float sizePressureSmooth = size * (smooth * pressure + smoothR * targetPressure); // sizePressure
                    Vector2 positionSmooth = smooth * position + smoothR * targetPosition;

                    distance += spacing * sizePressureSmooth; // distance
                    //

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["pressure"] = 1f,
                            ["radius"] = sizePressureSmooth,
                            ["targetPosition"] = positionSmooth,
                            ["color"] = colorHdr
                        }
                    });
                }

                return true;
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public bool IsometricDrawShaderBrushEdgeHardnessWithTexture(
            byte[] shaderCode,
            Vector4 colorHdr,
            CanvasBitmap texture,
            bool rotate,
            Vector2 position,
            Vector2 targetPosition,
            float pressure = 1f,
            float targetPressure = 1f,
            float size = 22f,
            float spacing = 0.25f,
            int hardness = 0,
            BitmapType type = BitmapType.Source
            )
        {
            // Normalize
            Vector2 vector = targetPosition - position;

            if (double.IsNaN(vector.X)) return false;
            if (double.IsNaN(vector.Y)) return false;

            Vector2 normalization = Vector2.Normalize(vector);
            //

            // Length
            float length = vector.Length();

            float sizePressure = size * pressure;
            float distance = spacing * sizePressure; // distance is spacingSizePressure

            if (distance > length) return false;
            // 

            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = 1f,
                        ["radius"] = sizePressure,
                        ["targetPosition"] = position,
                        ["color"] = colorHdr
                    }
                });

                while (distance < length)
                {
                    // Smooth
                    float smooth = distance / length;
                    float smoothR = 1 - smooth;

                    float sizePressureSmooth = size * (smooth * pressure + smoothR * targetPressure); // sizePressure
                    Vector2 positionSmooth = smooth * position + smoothR * targetPosition;

                    distance += spacing * sizePressureSmooth; // distance
                    //

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Source1 = texture,
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["rotate"] = rotate,
                            ["normalization"] = normalization,
                            ["pressure"] = 1f,
                            ["radius"] = sizePressureSmooth,
                            ["targetPosition"] = positionSmooth,
                            ["color"] = colorHdr
                        }
                    });
                }

                return true;
            }
        }

    }
}