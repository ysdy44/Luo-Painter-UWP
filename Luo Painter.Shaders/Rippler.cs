namespace Luo_Painter.Shaders
{
    /// <summary>
    /// Date of <see cref="ShaderType.RippleEffect"/>.
    /// <para> 
    /// https://github.com/microsoft/Windows-universal-samples/blob/main/Samples/D2DCustomEffects/cpp/PixelShader/CustomPixelShaderRenderer.cpp
    /// </para>
    /// </summary>
    public struct Rippler
    {
        public static Rippler Zero = new Rippler
        {
            Frequency = 140.0f,
            Phase = 0.0f,
            Amplitude = 60.0f,
            Spread = 0.01f,
        };

        public float Frequency;
        public float Phase;
        public float Amplitude;
        public float Spread;

        public Rippler(float time) // 0 ~ 20
        {
            // Reduce the frequency over time to make each individual wave spread out.
            this.Frequency = 140.0f - time * 7.5f;
            // Change the phase over time to make each individual wave travel away from the center.
            this.Phase = -time * 5.0f;
            // Reduce the amplitude over time to make the waves decay in intensity.
            this.Amplitude = 60.0f - time * 3.75f;
            // Increase the spread over time to make the visible area of the waves spread out.
            this.Spread = 0.01f + time / 40.0f;
        }

        public override string ToString() => $"Frequency:{this.Frequency},Phase:{this.Phase},Amplitude:{this.Amplitude},Spread:{this.Spread}";
    }
}