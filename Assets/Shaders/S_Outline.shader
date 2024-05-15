Shader "Custom/Outline" 
{
    Properties {
        _MainTex ("Main texture", 2D) = "white" {}
        _OutlineWidth ("Outline width", Range(0,0.1)) = 0
        _OutlineColor ("Outline color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags 
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent" 
		}

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
				ZFail Replace
			}

			ColorMask 0
		}

        Pass {


			Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
 
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct v2f {
                float4 pos : SV_POSITION;
            };
 
            float _OutlineWidth;
            float4 _OutlineColor;
 
            float4 vert(appdata_base v) : SV_POSITION {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 normal =  UnityObjectToViewPos(v.normal);
                normal.x *= UNITY_MATRIX_P[0][0];
                normal.y *= UNITY_MATRIX_P[1][1];
                o.pos.xy += normal.xy * _OutlineWidth;
                return o.pos;
            }
 
            half4 frag(v2f i) : COLOR {
                return _OutlineColor;
            }
 
            ENDCG
        }
 
       /* CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
        }
 
        ENDCG*/
    }
    FallBack "Diffuse"
}
