Shader "Custom/DisplacementShader"
{
    Properties
    {
       DisplacementTexture("DisplacementTexture", 2D) = "default"
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

            // Input Struct for Vertex
			struct appdata
			{
				float4 vertex : POSITION;
				float2 tex : TEXCOORD0;
                float4 norm : NORMAL;
			};

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D DisplacementTexture;

            v2f vert(appdata v) {
                v2f o;

                float4 color = tex2Dlod( DisplacementTexture, float4(v.tex.x, v.tex.y, 0, 0) );

                o.vertex = v.vertex;
                o.vertex.y += v.norm * length(color) * 0.1;
                o.vertex = UnityObjectToClipPos( o.vertex );
                o.color = color;
                //o.vertex.y += Displacement;

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {
                return i.color;
            }

            ENDCG
        }
    }
}
