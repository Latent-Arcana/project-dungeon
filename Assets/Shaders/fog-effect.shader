Shader "Custom/FogEffectWithPerlinNoiseFalloff"
{
    Properties
    {
        _NoiseScale ("Noise Scale", Float) = 1.0
        _Speed ("Animation Speed", Float) = 1.0
        _PixelSize ("Pixel Size", Float) = 10.0 // Controls the size of the "pixels"
        _SlowTimeFactor ("Slow Time Factor", Float) = 0.01 // Slow down time progression
        _DelayFactor ("Pixel Delay Factor", Float) = 0.5 // Controls how much the delay differs between pixels
        _FogDensity ("Fog Density", Float) = 0.5 // Controls the overall density of the fog
        _FogOpacity ("Fog Opacity", Float) = 0.6 // Controls the opacity of the fog effect
        _FalloffFactor ("Falloff Factor", Float) = 2.0 // Controls how quickly the fog fades
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _NoiseScale;
            float _Speed;
            float _PixelSize;
            float _SlowTimeFactor;
            float _DelayFactor;
            float _FogDensity;
            float _FogOpacity;
            float _FalloffFactor;

            // Simple Perlin noise function (2D) with time offset based on UV coordinates
            float perlinNoise(float2 uv, float timeOffset)
            {
                // Add the time offset to the calculation based on UV to create a delay effect
                float timeSlow = (_Time.y + timeOffset) * _SlowTimeFactor; // Slow time evolution with offset
                return (sin(uv.x * 12.9898 + uv.y * 78.233 + timeSlow * _Speed) * 43758.5453);
            }

            // Map the Perlin noise value to a fog-like color and opacity with falloff
            fixed4 mapNoiseToFog(float noiseValue, float falloff)
            {
                // Smooth out the noise to create a softer, more fog-like appearance
                float fogValue = (noiseValue - floor(noiseValue)) * 0.5 + 0.5; // Normalize to [0, 1]

                // Apply the falloff factor to simulate how the fog becomes denser or lighter
                fogValue = pow(fogValue, falloff); // Exponential falloff to control the fog's strength

                // Control the density of the fog (the darker it gets, the more dense the fog)
                fogValue = fogValue * _FogDensity;

                // Introduce opacity to make the fog transparent
                float opacity = lerp(0.0, 1.0, fogValue * _FogOpacity);

                // Adjust the RGB channels for a dynamic fog color effect
                return fixed4(fogValue, (fogValue * 0.8) - .1, (fogValue * 0.5) - .1, opacity); // Gradual transition to darker shades
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Pixelate the UV coordinates by scaling them and rounding them to grid steps
                float2 pixelatedUV = floor(i.uv * _PixelSize) / _PixelSize;

                // Calculate a time offset based on the pixel's UV coordinates for a smooth delay
                float timeOffset = dot(pixelatedUV, float2(12.9898, 78.233)); // Create a unique time offset based on UV coordinates

                // Generate Perlin noise value based on pixelated UV coordinates, time, and delay
                float noiseValue = perlinNoise(pixelatedUV * _NoiseScale, timeOffset);  // Scale UV for larger/smaller noise patterns
                noiseValue = (noiseValue - floor(noiseValue)) * 0.5 + 0.5;  // Normalize to [0, 1]

                // Apply falloff and map the noise value to a fog-like effect with smooth color transitions
                return mapNoiseToFog(noiseValue, _FalloffFactor);
            }
            ENDCG
        }
    }
}
