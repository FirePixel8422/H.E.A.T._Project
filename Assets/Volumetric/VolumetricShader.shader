Shader "Custom/VolumetricShader"
{
    Properties {
        _CameraColorTexture ("Camera Color Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.7,0.8,1,1)
        _Density ("Base Density", Float) = 0.5
        _Anisotropy ("Anisotropy (g)", Range(-0.9,0.9)) = 0.0
        _Scattering ("Scattering", Float) = 0.8
        _Extinction ("Extinction", Float) = 1.0
        _StepCount ("Raymarch Steps", Int) = 64
        _MaxDistance ("Max Distance", Float) = 100.0
        _FogHeight ("Fog Height (world) ", Float) = 0.0
        _FogFalloff ("Fog Falloff", Float) = 1.0
        _NoiseScale ("Noise Scale", Float) = 0.1
        _NoiseStrength ("Noise Strength", Float) = 0.25
        _TemporalJitter ("Temporal Jitter", Float) = 0.5
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            Name "FullscreenVolumetricFogPass"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _CameraColorTexture;
            sampler2D _CameraDepthTexture;

            float4 _FogColor;
            float _Density;
            float _Anisotropy;
            float _Scattering;
            float _Extinction;
            int _StepCount;
            float _MaxDistance;
            float _FogHeight;
            float _FogFalloff;
            float _NoiseScale;
            float _NoiseStrength;
            float _TemporalJitter;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes v) {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            float Hash(float2 p) {
                p = frac(p * 0.1031);
                p += dot(p, p.yx + 33.33);
                return frac((p.x + p.y) * p.x);
            }

            float Noise(float3 p) {
                float2 ip = floor(p.xy);
                float2 fp = frac(p.xy);
                float v00 = Hash(ip + float2(0,0) + p.z);
                float v10 = Hash(ip + float2(1,0) + p.z);
                float v01 = Hash(ip + float2(0,1) + p.z);
                float v11 = Hash(ip + float2(1,1) + p.z);
                float2 u = fp*fp*(3-2*fp);
                return lerp(lerp(v00,v10,u.x), lerp(v01,v11,u.x), u.y);
            }

            float PhaseHG(float cosTheta, float g) {
                float denom = 1 + g*g - 2*g*cosTheta;
                return (1 - g*g) / (4 * UNITY_PI * pow(denom, 1.5));
            }

            float4 frag(Varyings i) : SV_TARGET
            {
                float2 uv = i.uv;
                float3 viewDirVS = normalize(float3(uv*2-1, 1));
                float3 rayOriginVS = float3(0,0,0);

                // Sample depth and linearize to world units
                float sceneDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float linearSceneDepth = Linear01Depth(sceneDepth) * _ProjectionParams.z;
                float rayLength = min(_MaxDistance, linearSceneDepth / max(0.0001, dot(viewDirVS,float3(0,0,1))));

                int steps = max(1, _StepCount);
                float stepSize = rayLength / steps;
                float time = _Time.y;
                float jitter = frac(sin(dot(uv.xy * time, float2(12.9898,78.233))) * 43758.5453);
                float offset = (jitter-0.5) * _TemporalJitter * stepSize;

                float3 transmittanceAccum = float3(1,1,1);
                float3 scatteringAccum = float3(0,0,0);
                float t = offset;

                for(int s=0; s<steps; s++) {
                    float3 posVS = rayOriginVS + viewDirVS*t;
                    float3 posWS = mul(unity_CameraToWorld, float4(posVS,1)).xyz;

                    float density = _Density;
                    density *= saturate(exp(-(_FogHeight>0?_FogFalloff*(posWS.y-_FogHeight):0)));
                    density *= lerp(1-_NoiseStrength,1+_NoiseStrength, Noise(posWS*_NoiseScale + float3(0,0,time*0.02)));

                    float sigma_s = _Scattering * density;
                    float sigma_t = _Extinction * density;
                    float cosTheta = dot(normalize(viewDirVS), float3(0,0,1));
                    float phase = PhaseHG(cosTheta,_Anisotropy);
                    float3 inscatter = _FogColor.rgb * sigma_s * phase;

                    float3 dTrans = exp(-sigma_t * stepSize);
                    float3 contribution = transmittanceAccum * inscatter * (1 - dTrans);
                    scatteringAccum += contribution;
                    transmittanceAccum *= dTrans;
                    t += stepSize;
                    if(t>rayLength) break;
                }

                float3 sceneCol = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv).rgb;
                float3 final = sceneCol * transmittanceAccum + scatteringAccum;
                final = lerp(sceneCol, final, 1.0); // Blend fog over scene

                return float4(final,1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}