Shader "Custom/PortalView" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      
      struct Input {
          float4 screenPos;
      };
      
      sampler2D _MainTex;
      
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.screenPos.xy / IN.screenPos.w).rgb;
      }
      ENDCG
    } 
    Fallback "Diffuse"
}
