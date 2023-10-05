Shader "Shader Graphs/CopiedLiquidEffect"
    {
        Properties
        {
            _WobbleX("Wobble X", Float) = 1
            _WobbleZ("Wobble Z", Float) = 1
            _FillAmount("FillAmount", Vector) = (0, 0, 0, 0)
            _WaveTimeScale("WaveTimeScale", Float) = 1
            [HDR]_TopColor("Top Color", Color) = (0, 0.6946828, 0.764151, 1)
            [HDR]_BottomColor("Tint", Color) = (0, 0.8207547, 0.2961154, 1)
            [NoScaleOffset]_Main_Texture("Main Texture", 2D) = "white" {}
            _Foam_Smoothness("Foam Smoothness", Float) = 0
            [HDR]_FoamColor("Foam/EdgeColor", Color) = (0.2850213, 0.990566, 0.7492778, 1)
            _FoamWidth("FoamWidth", Range(0, 0.5)) = 0.04
            _Frequency("Frequency", Float) = 0
            _Amplitude("Amplitude", Float) = 0
            _Rim_Power("Rim Power", Float) = 2
            _Rim_Color("Rim Color", Color) = (0.693879, 1, 0.3254717, 1)
            [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
            [HideInInspector]_QueueControl("_QueueControl", Float) = -1
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Transparent"
                "DisableBatching"="False"
                "ShaderGraphShader"="true"
                "ShaderGraphTargetId"="UniversalUnlitSubTarget"
            }
            Pass
            {
                Name "Universal Forward"
                Tags
                {
                    // LightMode: <None>
                }

                Stencil
                {
                    Ref 2
                    Comp Always
                    Pass Replace
                }
            
            // Render State
            Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite On
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma instancing_options renderinglayer
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma shader_feature _ _SAMPLE_GI
                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_CULLFACE
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
                #define _FOG_FRAGMENT 1
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float3 normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 WorldSpaceNormal;
                     float3 WorldSpaceViewDirection;
                     float3 TimeParameters;
                     float FaceSign;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS : INTERP0;
                     float3 normalWS : INTERP1;
                     float3 FillPosition : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.positionWS.xyz = input.positionWS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.positionWS.xyz;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
                {
                    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
                
                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }
                
                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _IsFrontFace_b52e377029d41a8da4a44eb0140332c4_Out_0 = max(0, IN.FaceSign.x);
                    float4 _Property_b915d8ff3c8b4fdd8a28ea2bb53d4b23_Out_0 = _Rim_Color;
                    float _Property_ee5225c865384e4b97cc8f4504776c33_Out_0 = _Rim_Power;
                    float _FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3;
                    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_ee5225c865384e4b97cc8f4504776c33_Out_0, _FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3);
                    float4 _Multiply_e93ee2660e1740349480100cb1de57d4_Out_2;
                    Unity_Multiply_float4_float4(_Property_b915d8ff3c8b4fdd8a28ea2bb53d4b23_Out_0, (_FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3.xxxx), _Multiply_e93ee2660e1740349480100cb1de57d4_Out_2);
                    float4 _Property_7b2c672cd6f828859498090ad905feb0_Out_0 = IsGammaSpace() ? LinearToSRGB(_BottomColor) : _BottomColor;
                    UnityTexture2D _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0 = UnityBuildTexture2DStructNoScale(_Main_Texture);
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float4 _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.tex, _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.samplerstate, _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.GetTransformedUV((_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2.xx)) );
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_R_4 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.r;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_G_5 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.g;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_B_6 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.b;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_A_7 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.a;
                    float4 _Multiply_9b91869eff90466c84edf08522a68218_Out_2;
                    Unity_Multiply_float4_float4(_Property_7b2c672cd6f828859498090ad905feb0_Out_0, _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0, _Multiply_9b91869eff90466c84edf08522a68218_Out_2);
                    float _Property_09c5cae741fb492b9c992077efa84150_Out_0 = _FoamWidth;
                    float _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2;
                    Unity_Subtract_float(0.5, _Property_09c5cae741fb492b9c992077efa84150_Out_0, _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2);
                    float _Property_dd8293fa026441f4b849a2e1ef949eda_Out_0 = _Foam_Smoothness;
                    float _Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2;
                    Unity_Subtract_float(_Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2, _Property_dd8293fa026441f4b849a2e1ef949eda_Out_0, _Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2);
                    float _Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3;
                    Unity_Smoothstep_float(_Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2, _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, _Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    float _Multiply_46032e91067848b890e2b39695495f81_Out_2;
                    Unity_Multiply_float_float(_Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2, _Multiply_46032e91067848b890e2b39695495f81_Out_2);
                    float _OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1;
                    Unity_OneMinus_float(_Multiply_46032e91067848b890e2b39695495f81_Out_2, _OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1);
                    float4 _Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_9b91869eff90466c84edf08522a68218_Out_2, (_OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1.xxxx), _Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2);
                    float4 _Property_5ce189042e3150838460315eee2740d7_Out_0 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
                    float4 _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2;
                    Unity_Multiply_float4_float4(_Property_5ce189042e3150838460315eee2740d7_Out_0, (_Multiply_46032e91067848b890e2b39695495f81_Out_2.xxxx), _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2);
                    float4 _Add_622364ec8ca048cdb68440fe432c9dba_Out_2;
                    Unity_Add_float4(_Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2, _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2, _Add_622364ec8ca048cdb68440fe432c9dba_Out_2);
                    float4 _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2;
                    Unity_Add_float4(_Multiply_e93ee2660e1740349480100cb1de57d4_Out_2, _Add_622364ec8ca048cdb68440fe432c9dba_Out_2, _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2);
                    float4 _Property_7dd7346a90857a87b977fb392835ec49_Out_0 = IsGammaSpace() ? LinearToSRGB(_TopColor) : _TopColor;
                    float4 _Property_768b088e50814b13a2487f3febe90463_Out_0 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
                    float _Property_a8df2ceb30c34c5da2bc38a2da7cf048_Out_0 = _FoamWidth;
                    float _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2;
                    Unity_Multiply_float_float(0.1, _Property_a8df2ceb30c34c5da2bc38a2da7cf048_Out_0, _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2);
                    float _Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2;
                    Unity_Subtract_float(0.5, _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2, _Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2);
                    float _Step_b1a8d63bdf21496881e0261af22c6caf_Out_2;
                    Unity_Step_float(_Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, _Step_b1a8d63bdf21496881e0261af22c6caf_Out_2);
                    float _Multiply_96211e30c6bd44799e77f139054907d9_Out_2;
                    Unity_Multiply_float_float(_Step_b1a8d63bdf21496881e0261af22c6caf_Out_2, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2, _Multiply_96211e30c6bd44799e77f139054907d9_Out_2);
                    float4 _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2;
                    Unity_Multiply_float4_float4(_Property_768b088e50814b13a2487f3febe90463_Out_0, (_Multiply_96211e30c6bd44799e77f139054907d9_Out_2.xxxx), _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2);
                    float4 _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2;
                    Unity_Add_float4(_Property_7dd7346a90857a87b977fb392835ec49_Out_0, _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2, _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2);
                    float4 _Branch_44c0960736dc9e8799bfae01ef32184f_Out_3;
                    Unity_Branch_float4(_IsFrontFace_b52e377029d41a8da4a44eb0140332c4_Out_0, _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2, _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2, _Branch_44c0960736dc9e8799bfae01ef32184f_Out_3);
                    surface.BaseColor = (_Branch_44c0960736dc9e8799bfae01ef32184f_Out_3.xyz);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                    float3 unnormalizedNormalWS = input.normalWS;
                    const float renormFactor = 1.0 / length(unnormalizedNormalWS);
                
                
                    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
                
                
                    output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                    BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
                ColorMask R
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "DepthNormalsOnly"
                Tags
                {
                    "LightMode" = "DepthNormalsOnly"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
                #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS : INTERP0;
                     float3 FillPosition : INTERP1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
                ColorMask 0
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS : INTERP0;
                     float3 FillPosition : INTERP1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "SceneSelectionPass"
                Tags
                {
                    "LightMode" = "SceneSelectionPass"
                }
            
            // Render State
            Cull Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENESELECTIONPASS 1
                #define ALPHA_CLIP_THRESHOLD 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "ScenePickingPass"
                Tags
                {
                    "LightMode" = "Picking"
                }
            
            // Render State
            Cull Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 4.5
                #pragma exclude_renderers gles gles3 glcore
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENEPICKINGPASS 1
                #define ALPHA_CLIP_THRESHOLD 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Transparent"
                "DisableBatching"="False"
                "ShaderGraphShader"="true"
                "ShaderGraphTargetId"="UniversalUnlitSubTarget"
            }
            Pass
            {
                Name "Universal Forward"
                Tags
                {
                    // LightMode: <None>
                }
            
            // Render State
            Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite On
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile_fog
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma instancing_options renderinglayer
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma shader_feature _ _SAMPLE_GI
                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_CULLFACE
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
                #define _FOG_FRAGMENT 1
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float3 normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 WorldSpaceNormal;
                     float3 WorldSpaceViewDirection;
                     float3 TimeParameters;
                     float FaceSign;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS : INTERP0;
                     float3 normalWS : INTERP1;
                     float3 FillPosition : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.positionWS.xyz = input.positionWS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.positionWS.xyz;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
                {
                    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Subtract_float(float A, float B, out float Out)
                {
                    Out = A - B;
                }
                
                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
                
                void Unity_OneMinus_float(float In, out float Out)
                {
                    Out = 1 - In;
                }
                
                void Unity_Add_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
                {
                    Out = Predicate ? True : False;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _IsFrontFace_b52e377029d41a8da4a44eb0140332c4_Out_0 = max(0, IN.FaceSign.x);
                    float4 _Property_b915d8ff3c8b4fdd8a28ea2bb53d4b23_Out_0 = _Rim_Color;
                    float _Property_ee5225c865384e4b97cc8f4504776c33_Out_0 = _Rim_Power;
                    float _FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3;
                    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_ee5225c865384e4b97cc8f4504776c33_Out_0, _FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3);
                    float4 _Multiply_e93ee2660e1740349480100cb1de57d4_Out_2;
                    Unity_Multiply_float4_float4(_Property_b915d8ff3c8b4fdd8a28ea2bb53d4b23_Out_0, (_FresnelEffect_ccbdb69a91c647ab964b246d0b1cd6c3_Out_3.xxxx), _Multiply_e93ee2660e1740349480100cb1de57d4_Out_2);
                    float4 _Property_7b2c672cd6f828859498090ad905feb0_Out_0 = IsGammaSpace() ? LinearToSRGB(_BottomColor) : _BottomColor;
                    UnityTexture2D _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0 = UnityBuildTexture2DStructNoScale(_Main_Texture);
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float4 _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.tex, _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.samplerstate, _Property_b27c7f83131d4a80898f419e3d9bc708_Out_0.GetTransformedUV((_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2.xx)) );
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_R_4 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.r;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_G_5 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.g;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_B_6 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.b;
                    float _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_A_7 = _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0.a;
                    float4 _Multiply_9b91869eff90466c84edf08522a68218_Out_2;
                    Unity_Multiply_float4_float4(_Property_7b2c672cd6f828859498090ad905feb0_Out_0, _SampleTexture2D_426210e9070e4e8aaa30009dbad4a33c_RGBA_0, _Multiply_9b91869eff90466c84edf08522a68218_Out_2);
                    float _Property_09c5cae741fb492b9c992077efa84150_Out_0 = _FoamWidth;
                    float _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2;
                    Unity_Subtract_float(0.5, _Property_09c5cae741fb492b9c992077efa84150_Out_0, _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2);
                    float _Property_dd8293fa026441f4b849a2e1ef949eda_Out_0 = _Foam_Smoothness;
                    float _Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2;
                    Unity_Subtract_float(_Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2, _Property_dd8293fa026441f4b849a2e1ef949eda_Out_0, _Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2);
                    float _Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3;
                    Unity_Smoothstep_float(_Subtract_8653e30dc4ba48b0a131a0e3455425f5_Out_2, _Subtract_390d3c8373564f1fb4a39104ac960a11_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, _Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    float _Multiply_46032e91067848b890e2b39695495f81_Out_2;
                    Unity_Multiply_float_float(_Smoothstep_c8b1ee587f8a4f09b3a6d84df42c0e2b_Out_3, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2, _Multiply_46032e91067848b890e2b39695495f81_Out_2);
                    float _OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1;
                    Unity_OneMinus_float(_Multiply_46032e91067848b890e2b39695495f81_Out_2, _OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1);
                    float4 _Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2;
                    Unity_Multiply_float4_float4(_Multiply_9b91869eff90466c84edf08522a68218_Out_2, (_OneMinus_9cea9afdfef543458e9015c0d9afcee9_Out_1.xxxx), _Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2);
                    float4 _Property_5ce189042e3150838460315eee2740d7_Out_0 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
                    float4 _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2;
                    Unity_Multiply_float4_float4(_Property_5ce189042e3150838460315eee2740d7_Out_0, (_Multiply_46032e91067848b890e2b39695495f81_Out_2.xxxx), _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2);
                    float4 _Add_622364ec8ca048cdb68440fe432c9dba_Out_2;
                    Unity_Add_float4(_Multiply_1dadcf19efe742aab1105348c1e0cce3_Out_2, _Multiply_e499e65e06824abc8b534813505dd5d7_Out_2, _Add_622364ec8ca048cdb68440fe432c9dba_Out_2);
                    float4 _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2;
                    Unity_Add_float4(_Multiply_e93ee2660e1740349480100cb1de57d4_Out_2, _Add_622364ec8ca048cdb68440fe432c9dba_Out_2, _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2);
                    float4 _Property_7dd7346a90857a87b977fb392835ec49_Out_0 = IsGammaSpace() ? LinearToSRGB(_TopColor) : _TopColor;
                    float4 _Property_768b088e50814b13a2487f3febe90463_Out_0 = IsGammaSpace() ? LinearToSRGB(_FoamColor) : _FoamColor;
                    float _Property_a8df2ceb30c34c5da2bc38a2da7cf048_Out_0 = _FoamWidth;
                    float _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2;
                    Unity_Multiply_float_float(0.1, _Property_a8df2ceb30c34c5da2bc38a2da7cf048_Out_0, _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2);
                    float _Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2;
                    Unity_Subtract_float(0.5, _Multiply_a0adbf2bbf304e779145b22a6f67b716_Out_2, _Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2);
                    float _Step_b1a8d63bdf21496881e0261af22c6caf_Out_2;
                    Unity_Step_float(_Subtract_f4237cef516f4c9584096fbf493ab0fe_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, _Step_b1a8d63bdf21496881e0261af22c6caf_Out_2);
                    float _Multiply_96211e30c6bd44799e77f139054907d9_Out_2;
                    Unity_Multiply_float_float(_Step_b1a8d63bdf21496881e0261af22c6caf_Out_2, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2, _Multiply_96211e30c6bd44799e77f139054907d9_Out_2);
                    float4 _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2;
                    Unity_Multiply_float4_float4(_Property_768b088e50814b13a2487f3febe90463_Out_0, (_Multiply_96211e30c6bd44799e77f139054907d9_Out_2.xxxx), _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2);
                    float4 _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2;
                    Unity_Add_float4(_Property_7dd7346a90857a87b977fb392835ec49_Out_0, _Multiply_19c254135f8e4ec090a014aa28e06e3e_Out_2, _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2);
                    float4 _Branch_44c0960736dc9e8799bfae01ef32184f_Out_3;
                    Unity_Branch_float4(_IsFrontFace_b52e377029d41a8da4a44eb0140332c4_Out_0, _Add_8ee30018fbf1483cbbb43864e0831bf6_Out_2, _Add_e1bc4d6556554aeabc7ed61b983a28b6_Out_2, _Branch_44c0960736dc9e8799bfae01ef32184f_Out_3);
                    surface.BaseColor = (_Branch_44c0960736dc9e8799bfae01ef32184f_Out_3.xyz);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                    float3 unnormalizedNormalWS = input.normalWS;
                    const float renormFactor = 1.0 / length(unnormalizedNormalWS);
                
                
                    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
                
                
                    output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                    BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "DepthOnly"
                Tags
                {
                    "LightMode" = "DepthOnly"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
                ColorMask R
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "DepthNormalsOnly"
                Tags
                {
                    "LightMode" = "DepthNormalsOnly"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv1 : TEXCOORD1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                     float4 tangentWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 tangentWS : INTERP0;
                     float3 normalWS : INTERP1;
                     float3 FillPosition : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.tangentWS.xyzw = input.tangentWS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.tangentWS = input.tangentWS.xyzw;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
            
            // Render State
            Cull Off
                ZTest LEqual
                ZWrite On
                ColorMask 0
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS : INTERP0;
                     float3 FillPosition : INTERP1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.normalWS.xyz = input.normalWS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.normalWS = input.normalWS.xyz;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "SceneSelectionPass"
                Tags
                {
                    "LightMode" = "SceneSelectionPass"
                }
            
            // Render State
            Cull Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENESELECTIONPASS 1
                #define ALPHA_CLIP_THRESHOLD 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "ScenePickingPass"
                Tags
                {
                    "LightMode" = "Picking"
                }
            
            // Render State
            Cull Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #pragma target 3.5 DOTS_INSTANCING_ON
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENEPICKINGPASS 1
                #define ALPHA_CLIP_THRESHOLD 1
                #define _ALPHATEST_ON 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                     float3 FillPosition;
                };
                struct SurfaceDescriptionInputs
                {
                     float3 TimeParameters;
                     float3 FillPosition;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                     float3 WorldSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 FillPosition : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.FillPosition.xyz = input.FillPosition;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.FillPosition = input.FillPosition.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float _WobbleX;
                float _WobbleZ;
                float4 _TopColor;
                float4 _BottomColor;
                float4 _FoamColor;
                float3 _FillAmount;
                float4 _Main_Texture_TexelSize;
                float _FoamWidth;
                float _Frequency;
                float _Amplitude;
                float _Rim_Power;
                float4 _Rim_Color;
                float _Foam_Smoothness;
                float _WaveTimeScale;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_Main_Texture);
                SAMPLER(sampler_Main_Texture);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Subtract_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A - B;
                }
                
                void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
                {
                    Rotation = radians(Rotation);
                
                    float s = sin(Rotation);
                    float c = cos(Rotation);
                    float one_minus_c = 1.0 - c;
                
                    Axis = normalize(Axis);
                
                    float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                              one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                              one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                            };
                
                    Out = mul(rot_mat,  In);
                }
                
                void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float3(float3 A, float3 B, out float3 Out)
                {
                    Out = A + B;
                }
                
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }
                
                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }
                
                void Unity_Step_float(float Edge, float In, out float Out)
                {
                    Out = step(Edge, In);
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                    float3 FillPosition;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0 = _WobbleZ;
                    float _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0 = _WobbleX;
                    float3 _Property_6dc235de29024a5baf3314fc962f75f2_Out_0 = _FillAmount;
                    float3 _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2;
                    Unity_Subtract_float3(IN.WorldSpacePosition, SHADERGRAPH_OBJECT_POSITION, _Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2);
                    float3 _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2;
                    Unity_Subtract_float3(_Subtract_5152ed3ce574499e887a3ada42fd96ab_Out_2, _Property_6dc235de29024a5baf3314fc962f75f2_Out_0, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2);
                    float3 _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (1, 0, 0), _Property_5a55e7a9e7cf31819615b283a53723f0_Out_0, _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3);
                    float3 _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_ea8645dfad306e808f074a65b0985a59_Out_3, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2);
                    float3 _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3;
                    Unity_Rotate_About_Axis_Degrees_float(_Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, float3 (0, 1, 0), _Property_35372246b6c6ea8da44f990b9b7861e7_Out_0, _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3);
                    float3 _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2;
                    Unity_Multiply_float3_float3(float3(1, 1, 1), _RotateAboutAxis_6fbce07e3e6b4889900703c021779fa3_Out_3, _Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2);
                    float3 _Add_653001ab3d246f8498fb80372fa669c6_Out_2;
                    Unity_Add_float3(_Multiply_6b9d786c35d4ef8cadea75341899ea8b_Out_2, _Multiply_a3df392fd99401829fd2e22b4d5ce62a_Out_2, _Add_653001ab3d246f8498fb80372fa669c6_Out_2);
                    float3 _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    Unity_Add_float3(_Add_653001ab3d246f8498fb80372fa669c6_Out_2, _Subtract_406a7cd9fbc74102b8d4d6e225a6db4c_Out_2, _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2);
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    description.FillPosition = _Add_e86cd2c0a06f0f8a9e51d942ffe32294_Out_2;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            output.FillPosition = input.FillPosition;
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                    float AlphaClipThreshold;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_R_1 = IN.FillPosition[0];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_G_2 = IN.FillPosition[1];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_B_3 = IN.FillPosition[2];
                    float _Split_5d5a6388fedfab88bcf0e75524e857ca_A_4 = 0;
                    float _Property_6687eeed7cb14e2da3d023daa87d802b_Out_0 = _Amplitude;
                    float _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2;
                    Unity_Multiply_float_float(_Property_6687eeed7cb14e2da3d023daa87d802b_Out_0, 2, _Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2);
                    float _Property_554c817137394bb0a2e3e7637d6704e7_Out_0 = _WaveTimeScale;
                    float _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_554c817137394bb0a2e3e7637d6704e7_Out_0, _Multiply_ece28e32e607438a9144e35a9cd91316_Out_2);
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1 = IN.FillPosition[0];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_G_2 = IN.FillPosition[1];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3 = IN.FillPosition[2];
                    float _Split_dfb41dfdc68f43d0b5f9bad32945e464_A_4 = 0;
                    float _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0 = _Frequency;
                    float _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2;
                    Unity_Multiply_float_float(_Split_dfb41dfdc68f43d0b5f9bad32945e464_R_1, _Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2);
                    float _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2;
                    Unity_Multiply_float_float(_Property_49ebcdf42d6b41c9a6bc0ec1956b816f_Out_0, _Split_dfb41dfdc68f43d0b5f9bad32945e464_B_3, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2);
                    float _Add_3f232ec2488b4fec868579862a2102fa_Out_2;
                    Unity_Add_float(_Multiply_981ec7f2b89a4f33bf3c44081dd3b7f3_Out_2, _Multiply_50aa97dca2e44a8f939639220f74fbbc_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2);
                    float _Add_21a49893793745efa0259bf2a7692173_Out_2;
                    Unity_Add_float(_Multiply_ece28e32e607438a9144e35a9cd91316_Out_2, _Add_3f232ec2488b4fec868579862a2102fa_Out_2, _Add_21a49893793745efa0259bf2a7692173_Out_2);
                    float _Sine_eba6c5550d0140f1b7262d951d465391_Out_1;
                    Unity_Sine_float(_Add_21a49893793745efa0259bf2a7692173_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1);
                    float _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2;
                    Unity_Multiply_float_float(_Multiply_3d1071b326d5447eb34cdac9a2efbf4e_Out_2, _Sine_eba6c5550d0140f1b7262d951d465391_Out_1, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2);
                    float _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2;
                    Unity_Add_float(_Split_5d5a6388fedfab88bcf0e75524e857ca_B_3, _Multiply_6b91f7e43f4043308ff61de011fc92c2_Out_2, _Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2);
                    float _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    Unity_Step_float(_Add_062766ebf75e4bbcbfb80f7bdfc13e78_Out_2, 1, _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2);
                    surface.Alpha = _Step_96e0982632ee1c8cad5b757fbe46107b_Out_2;
                    surface.AlphaClipThreshold = 0.01;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                    output.WorldSpacePosition =                         TransformObjectToWorld(input.positionOS);
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    output.FillPosition = input.FillPosition;
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
        }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
        CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
        FallBack "Hidden/Shader Graph/FallbackError"
    }