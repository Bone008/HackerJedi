// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32298,y:32640,varname:node_3138,prsc:2|custl-6034-OUT;n:type:ShaderForge.SFN_ScreenPos,id:6338,x:31505,y:32754,varname:node_6338,prsc:2,sctp:0;n:type:ShaderForge.SFN_Tex2d,id:9652,x:31976,y:32754,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_6628,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4676-OUT;n:type:ShaderForge.SFN_OneMinus,id:2498,x:31661,y:32754,varname:node_2498,prsc:2|IN-6338-UVOUT;n:type:ShaderForge.SFN_RemapRange,id:4676,x:31820,y:32754,varname:node_4676,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0.5|IN-2498-OUT;n:type:ShaderForge.SFN_Tex2d,id:7465,x:31661,y:32948,ptovrint:False,ptlb:DistorsionPattern,ptin:_DistorsionPattern,varname:node_7465,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8148-OUT;n:type:ShaderForge.SFN_TexCoord,id:9280,x:30432,y:32875,varname:node_9280,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:4253,x:30885,y:32983,ptovrint:False,ptlb:DistorsionPatternSpeedX,ptin:_DistorsionPatternSpeedX,varname:node_4253,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:6271,x:30885,y:33070,ptovrint:False,ptlb:DistorsionPatternSpeedY,ptin:_DistorsionPatternSpeedY,varname:node_6271,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Time,id:9673,x:30964,y:32844,varname:node_9673,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5341,x:31195,y:32899,varname:node_5341,prsc:2|A-9673-T,B-4253-OUT;n:type:ShaderForge.SFN_Multiply,id:4522,x:31195,y:33021,varname:node_4522,prsc:2|A-9673-T,B-6271-OUT;n:type:ShaderForge.SFN_Add,id:4813,x:31351,y:32899,varname:node_4813,prsc:2|A-4510-R,B-5341-OUT;n:type:ShaderForge.SFN_Add,id:2149,x:31351,y:33021,varname:node_2149,prsc:2|A-4510-G,B-4522-OUT;n:type:ShaderForge.SFN_Append,id:8148,x:31505,y:32948,varname:node_8148,prsc:2|A-4813-OUT,B-2149-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2452,x:30432,y:33035,ptovrint:False,ptlb:DistorsionPatternTiling,ptin:_DistorsionPatternTiling,varname:node_2452,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8154,x:30591,y:32946,varname:node_8154,prsc:2|A-9280-UVOUT,B-2452-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4510,x:30748,y:32946,varname:node_4510,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8154-OUT;n:type:ShaderForge.SFN_Color,id:4778,x:31661,y:33125,ptovrint:False,ptlb:DistorsionPatternColor,ptin:_DistorsionPatternColor,varname:node_4778,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:7164,x:31820,y:33044,varname:node_7164,prsc:2|A-7465-R,B-4778-RGB;n:type:ShaderForge.SFN_Blend,id:6034,x:32136,y:32880,varname:node_6034,prsc:2,blmd:6,clmp:False|SRC-9652-RGB,DST-6442-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6442,x:31976,y:33004,ptovrint:False,ptlb:EnableDistorsionPattern,ptin:_EnableDistorsionPattern,varname:node_6442,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7755-OUT,B-7164-OUT;n:type:ShaderForge.SFN_Vector1,id:7755,x:31820,y:32986,varname:node_7755,prsc:2,v1:0;proporder:9652-6442-7465-2452-4253-6271-4778;pass:END;sub:END;*/

Shader "Gater/UV Remap" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _EnableDistorsionPattern ("EnableDistorsionPattern", Float ) = 0
        _DistorsionPattern ("DistorsionPattern", 2D) = "white" {}
        _DistorsionPatternTiling ("DistorsionPatternTiling", Float ) = 1
        _DistorsionPatternSpeedX ("DistorsionPatternSpeedX", Range(-10, 10)) = 0
        _DistorsionPatternSpeedY ("DistorsionPatternSpeedY", Range(-10, 10)) = 0
        _DistorsionPatternColor ("DistorsionPatternColor", Color) = (1,0,0,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _DistorsionPattern; uniform float4 _DistorsionPattern_ST;
            uniform float _DistorsionPatternSpeedX;
            uniform float _DistorsionPatternSpeedY;
            uniform float _DistorsionPatternTiling;
            uniform float4 _DistorsionPatternColor;
            uniform fixed _EnableDistorsionPattern;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
////// Lighting:
                float2 node_4676 = ((1.0 - i.screenPos.rg)*-0.5+1.0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4676, _MainTex));
                float2 node_4510 = (i.uv0*_DistorsionPatternTiling).rg;
                float4 node_9673 = _Time + _TimeEditor;
                float2 node_8148 = float2((node_4510.r+(node_9673.g*_DistorsionPatternSpeedX)),(node_4510.g+(node_9673.g*_DistorsionPatternSpeedY)));
                float4 _DistorsionPattern_var = tex2D(_DistorsionPattern,TRANSFORM_TEX(node_8148, _DistorsionPattern));
                float3 finalColor = (1.0-(1.0-_MainTex_var.rgb)*(1.0-lerp( 0.0, (_DistorsionPattern_var.r*_DistorsionPatternColor.rgb), _EnableDistorsionPattern )));
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
