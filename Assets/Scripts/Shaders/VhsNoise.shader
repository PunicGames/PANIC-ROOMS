Shader "Hidden/VhsNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 screenPosition : TEXCOORD1;
        };

        TEXTURE2D(_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;

        SamplerState sampler_point_clamp;

        uniform float _Intensity;
        uniform float2 _BlockCount;
        uniform float2 _BlockSize;
        uniform float2 _HalfBlockSize;


        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            OUT.screenPosition = ComputeScreenPos(OUT.positionHCS);
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            #define PI 3.14159265359
            #define TAPESPEED 3.0

            float rand (float2 co)
			{
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

            float3 tex2D(float2 p)
            {
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, p).xyz;
                if ( 0.5 < abs(p.x - 0.5))
                {
					col = float3(0.1,0.1,0.1);
				}

                return col;
            }

            float2 grad(int2 z )
            {
                int n = z.x + z.y * 11111;
                
                n = (n<<13) ^ n;
                n = (n*(n*n*15731+789221)+1376312589)>>16;

                return float2(cos(float(n)),sin(float(n)));
            }

            float pnoise(float2  p)
            {
                int2 i = int2(floor(p));
                float2 f = frac(p);

                float2 u = f * f * (3.0 - 2.0 * f);

                return  lerp( lerp( dot( grad( i+int2(0,0) ), f-float2(0.0,0.0) ), 
                             dot( grad( i+int2(1,0) ), f-float2(1.0,0.0) ), u.x),
                             lerp( dot( grad( i+int2(0,1) ), f-float2(0.0,1.0) ), 
                             dot( grad( i+int2(1,1) ), f-float2(1.0,1.0) ), u.x), u.y);
            }

            float perlinNoise (float2 p, float2 uv)
            {
                float f = 0.0;
                uv *= 8.0;

                // Definir la matriz de transformación
                float2x2 m = float2x2(1.6, 1.2, -1.2, 1.6);

                
                f += 0.5000 * pnoise(uv); 
                uv = mul(m, uv);               

                f += 0.2500 * pnoise(uv);
                uv = mul(m, uv);

                f += 0.1250 * pnoise(uv);
                uv = mul(m, uv);

                f += 0.0625 * pnoise(uv);
                uv = mul(m, uv);
    
                return 0.5 + 0.5 * f;
            }


            float hash (float2 v)
            {
                return frac( sin( dot(v, float2( 89.44, 19.36 ) ) ) * 22189.22 );
            }

            float iHash( float2 v, float2 r)
            {
                
                float h00 = hash( floor( v * r + float2(0.0, 0.0)) / r );
                float h10 = hash( floor( v * r + float2(1.0, 0.0)) / r );
                float h01 = hash( floor( v * r + float2(0.0, 1.0)) / r );
                float h11 = hash( floor( v * r + float2(1.0, 1.0)) / r );

                float2 f = frac( v * r );
                float2 ip = smoothstep(float2(0.0, 0.0), float2(1.0, 1.0), frac(v * r));
                float ix0 = lerp(h00, h10, ip.x);
                float ix1 = lerp(h01, h11, ip.x);
                return lerp(ix0, ix1, ip.y);
            }

            float rnd(float2 noise)
            {
                // Añadir más irregularidad usando números primos y cambios en las frecuencias y fases
                float a = frac(sin(dot(noise.xy, float2(12.9898, 78.233))) * 43758.5453123);
                float b = frac(sin(dot(noise.xy, float2(0.1, 24.433))) * 1234.564567);

                // Usar una combinación no lineal de a y b para disminuir la periodicidad
                return frac(a * b * 95.233);
            }

            float noise (float2 p)
			{
				float sum = 0.0;

                for (int i = 1; i < 9; i++)
				{
                    float aux = pow(2.0, float(i));
                    float value = 2.0 * aux;
					sum += iHash(p + float2(i,i), float2 (value, value) / aux);
				}

                return sum;
			}

            half4 frag(Varyings input) : SV_Target
            {
                float2 iResolution = float2(16,9);
                float iTime = _Time.y;

                // Normalizing pixel coordinates
                float2 fragCoord = input.screenPosition.xy;
                float2 fragPos = fragCoord / iResolution;
                float2 fPos = fragPos * 2.0 - 1.0;
                fPos.x *= iResolution.x / iResolution.y;

                float dist = length(fPos);

                // Texture sampling adjusted for aspect ratio
                float2 uv = input.uv;
                float2 uvn = input.uv;

                // Implement your noise, rand, perlinNoise functions here, assume they are implemented

                float randomOffset = noise(float2(iTime * 0.5, 0)) * 0.5 - 0.25;
                float pNoise = perlinNoise(fragPos, uv);

                float randSPD = rand(float2(0.0, fragPos.y + iTime));
                float tcPhase = clamp( ( sin( uvn.y * 4.0 + randomOffset + (iTime + randomOffset * 0.005f)) - 0.98 ) * noise( float2( iTime, iTime ) ), 0.0, 0.01 ) * 7.5 * randomOffset;
                float tcNoise = max( noise( float2( uvn.y * 25.0, iTime * 100.0 ) ) - 0.5, 0.0);
                uvn.x = uvn.x - (tcNoise * tcPhase * pNoise) * 0.5;
    
                float3 col = float3(0.0,0.0,0.0);

                // switching noise
                float snPhase = smoothstep( 0.03, 0.0, uvn.y );
                //uvn.y += snPhase * 0.7;
                //uvn.x += snPhase * ( ( noise( float2( uv.y * 100.0, iTime * 10.0 ) ) - 0.5 ) * 0.2 );
                float3 c = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, input.uv).xyz;

                col = tex2D(uvn);
                col *= 1.0 - tcPhase;

                col = (c + col) * 0.5;
                // ac beat
                //col *= 1.0 + clamp( noise( float2( 0.0, uv.y + sin(iTime * 0.5f + randomOffset) * 0.2) ) * 0.6 - 0.25, 0.0, 0.1 );
                //col *= 1.0 - vignette;
                
                float rNoise = rnd(input.uv * frac(abs(sin(_Time.y * 0.005)))) * _Intensity; 
                float gNoise = rnd((float2(0.01,0.0) + input.uv) * frac(abs(sin(_Time.y * 0.005)))) * _Intensity;
                float bNoise = rnd((float2(0.02,0.0) + input.uv) * frac(abs(sin(_Time.y * 0.005)))) * _Intensity; 
                
                float2 pos = input.uv;
                pos -= 0.5;
                float vignette = length(pos) - 0.45;
                vignette = 1.0 - smoothstep(0.0, 0.2, vignette);
    
	            return float4( col.x + rNoise, col.y + gNoise, col.z + bNoise, 1.0 ) * vignette;
            }
            ENDHLSL
        } 

        
    }
}