Shader "Simple Toon/SToon Default"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Colorize)][Space(5)]  //colorize
		_Color ("Color", COLOR) = (1,1,1,1)

		[HideInInspector] _ColIntense ("Intensity", Range(0,3)) = 1
        [HideInInspector] _ColBright ("Brightness", Range(-1,1)) = 0
		_AmbientCol ("Ambient", Range(0,1)) = 0

        [Header(Detail)][Space(5)]  //detail
        [Toggle] _Segmented ("Segmented", Float) = 1
        _Steps ("Steps", Range(1,25)) = 3
        _StpSmooth ("Smoothness", Range(0,1)) = 0
        _Offset ("Lit Offset", Range(-1,1.1)) = 0

        [Header(Light)][Space(5)]  //light
        [Toggle] _Clipped ("Clipped", Float) = 0
        _MinLight ("Min Light", Range(0,1)) = 0
        _MaxLight ("Max Light", Range(0,1)) = 1
        _Lumin ("Luminocity", Range(0,2)) = 0

        [Header(Shine)][Space(5)]  //shine
		[HDR] _ShnColor ("Color", COLOR) = (1,1,0,1)
        [Toggle] _ShnOverlap ("Overlap", Float) = 0

        _ShnIntense ("Intensity", Range(0,1)) = 0
        _ShnRange ("Range", Range(0,1)) = 0.15
        _ShnSmooth ("Smoothness", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "UniversalMaterialType" = "Lit"
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _ShnColor;
                float _ColIntense;
                float _ColBright;
                float _AmbientCol;
                float _Segmented;
                float _Steps;
                float _StpSmooth;
                float _Offset;
                float _Clipped;
                float _MinLight;
                float _MaxLight;
                float _Lumin;
                float _ShnOverlap;
                float _ShnIntense;
                float _ShnRange;
                float _ShnSmooth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float STClamp01(float value)
            {
                return saturate(value);
            }

            float STRev(float value)
            {
                return 1.0 - value;
            }

            float STPosZ(float value)
            {
                return value >= 0.0 ? 1.0 : 0.0;
            }

            float STInvLerp01(float from, float to, float value)
            {
                if (from == to)
                    return 0.0;
                return STClamp01((value - from) / (to - from));
            }

            float STSmoothStep(float from, float to, float value, float equalValue)
            {
                if (from == to)
                    return equalValue;
                return smoothstep(from, to, value);
            }

            float STSmoothLerp(float from, float to, float value)
            {
                float shaped = -(2.0 / ((value + 0.34) * 4.7)) + 1.3;
                return lerp(from, to, saturate(shaped));
            }

            float4 STColorBlend(float4 baseColor, float4 targetColor, float blend)
            {
                return lerp(baseColor, targetColor, blend);
            }

            float STToon(float ndotl, float atten)
            {
                float maxLight = max(_MinLight, _MaxLight);
                float steps = _Segmented > 0.5 ? max(_Steps, 1.0) : 1.0;
                float stepSmooth = _Segmented > 0.5 ? _StpSmooth : 1.0;
                float offset = clamp(_Offset, -1.0, 1.0);
                float delta = maxLight - _MinLight;

                float intense = STClamp01((ndotl + offset) / (1.0 + offset));
                float stepSize = 1.0 / floor(steps);
                int litNum = (int)ceil(intense / stepSize);
                float lit = litNum * stepSize;

                float reduce = litNum == 1 ? STClamp01(1.0 - ((_Offset - 1.0) / 0.1)) : 1.0;
                float smoothStart = lit - stepSize;
                float smoothEnd = smoothStart + stepSize * stepSmooth;
                float smoothLerp = STInvLerp01(smoothEnd, smoothStart, intense);
                float smoothStep = STSmoothStep(smoothEnd, smoothStart, intense, 0.0);
                float smoothValue = STSmoothLerp(smoothStep, smoothLerp, stepSmooth);
                float smoothed = STClamp01(lit - smoothValue * reduce * stepSize);

                float attenInv = clamp(atten, 0.0, 1.0);
                float dimLit = smoothed * attenInv;
                float lumLight = maxLight + _Lumin;
                float lumDelta = lumLight - _MinLight;

                float clipped = clamp(_MinLight + (STClamp01(dimLit - _MinLight) / max(delta, 0.0001)) * lumDelta, _MinLight, lumLight);
                float relative = _MinLight + lumDelta * dimLit;
                return (_Clipped > 0.5) ? clipped : relative;
            }

            void STPostShine(inout float4 color, float ndotl, float atten)
            {
                float pos = abs(ndotl - 1.0);
                float len = _ShnRange * 2.0;
                float smoothEnd = len * (1.0 - _ShnSmooth);
                float shine = STPosZ(len - pos);
                float smooth = STSmoothStep(len, smoothEnd, pos, 1.0);
                float dim = 1.0 - STRev(atten) * STRev(_ShnOverlap);
                color = STColorBlend(color, _ShnColor, _ShnIntense * shine * smooth * dim);
            }

            float4 STShade(float3 normalWS, float3 viewDirWS, float2 uv, float3 lightDir, float3 lightColor, float atten)
            {
                float ndotl = dot(normalWS, lightDir);
                float toon = STToon(ndotl, atten);
                float4 shade = float4(0.0, 0.0, 0.0, 1.0);
                float4 lit = STColorBlend(_Color, float4(lightColor, 1.0), _AmbientCol);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * lit * _ColIntense + _ColBright;
                float4 color = STColorBlend(shade, tex, toon);
                STPostShine(color, ndotl, atten);
                color.a = 1.0;
                return color;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positions.positionCS;
                output.positionWS = positions.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                float mainAtten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                float4 color = 0.0;
                if (dot(mainLight.color, 1.0) > 0.0)
                    color = STShade(normalWS, viewDirWS, input.uv, normalize(mainLight.direction), mainLight.color, mainAtten);

                #ifdef _ADDITIONAL_LIGHTS
                uint lightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < lightCount; lightIndex++)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    float atten = light.shadowAttenuation * light.distanceAttenuation;
                    float4 addColor = STShade(normalWS, viewDirWS, input.uv, normalize(light.direction), light.color, atten);
                    color = max(color, addColor);
                }
                #endif

                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        Pass
        {
            Name "DirectLight"
            LOD 80

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #include "STCore.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                LIGHTING_COORDS(0,1)
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _MaxLight = max(_MinLight, _MaxLight);
                _Steps = _Segmented ? _Steps : 1;
                _StpSmooth = _Segmented ? _StpSmooth : 1;

				_DarkColor = fixed4(0,0,0,1);
				_MaxAtten = 1.0;

				float3 normal = normalize(i.worldNormal);
				float3 light_dir = normalize(_WorldSpaceLightPos0.xyz);
				float3 view_dir = normalize(i.viewDir);
				float3 halfVec = normalize(light_dir + view_dir);
				float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));

                float NdotL = dot(normal, light_dir);
				float NdotH = dot(normal, halfVec);
				float VdotN = dot(view_dir, normal);
				float FdotV = dot(forward, -view_dir);

                fixed atten = SHADOW_ATTENUATION(i);
                float toon = Toon(NdotL, atten);

				fixed4 shadecol = _DarkColor;
				fixed4 litcol = ColorBlend(_Color, _LightColor0, _AmbientCol);
				fixed4 texcol = tex2D(_MainTex, i.uv) * litcol * _ColIntense + _ColBright;

				float4 blendCol = ColorBlend(shadecol, texcol, toon);
				float4 postCol = PostEffects(blendCol, toon, atten, NdotL, NdotH, VdotN, FdotV);

				postCol.a = 1.;
				return _LightColor0.a > 0 ? postCol : 0;
            }

            ENDCG
        }

        Tags { "RenderType" = "Opaque" "LightMode" = "ForwardAdd" }
        Pass
        {
            Name "SpotLight"
            BlendOp Max
            LOD 100

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #include "STCore.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                LIGHTING_COORDS(0,1)
                float2 uv : TEXCOORDSS;
                float4 pos : SV_POSITION;
                float3 worldPos : WORLD;
                half3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir = WorldSpaceViewDir(v.vertex);

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				_MaxLight = max(_MinLight, _MaxLight);
                _Steps = _Segmented ? _Steps : 1;
                _StpSmooth = _Segmented ? _StpSmooth : 1;

				_DarkColor = fixed4(0,0,0,1);
				_MaxAtten = 1.0;

				float3 normal = normalize(i.worldNormal);
				float3 light_dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				float3 view_dir = normalize(i.viewDir);
				float3 halfVec = normalize(light_dir + view_dir);
				float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));

				float NdotL = dot(normal, light_dir);
				float NdotH = dot(normal, halfVec);
				float VdotN = dot(view_dir, normal);
				float FdotV = dot(forward, -view_dir);

                float atten = LIGHT_ATTENUATION(i);
                float toon = Toon(NdotL, atten);

				fixed4 shadecol = _DarkColor;
				fixed4 litcol = ColorBlend(_Color, _LightColor0, _AmbientCol);
				fixed4 texcol = tex2D(_MainTex, i.uv) * litcol * _ColIntense + _ColBright;

				float4 blendCol = ColorBlend(shadecol, texcol, toon);
				float4 postCol = PostEffects(blendCol, toon, atten, NdotL, NdotH, VdotN, FdotV);

				postCol.a = 1.;
				return postCol;
            }

            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
