Shader "Custom/DisplacementShader"
{
    Properties
    {
       DisplacementTexture("DisplacementTexture_Anzeige", 2D) = "default"
       ColorTexture("Texture", 2D) = "default"
       Scale("DisplacementScale", Float) = 0.1
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
            sampler2D ColorTexture;
            float Scale;

            v2f vert(appdata v) {
                v2f o;

                //float2 uv = (v.tex.xy * 3) % float2(1, 1);
                float2 uv = v.tex.xy;

                float4 color = tex2Dlod( DisplacementTexture, float4(uv, 0, 0) );
                o.color = tex2Dlod( ColorTexture, float4(uv, 0, 0) );

                o.vertex = v.vertex;
                o.vertex.xyz += v.norm * color * Scale;
                o.vertex = UnityObjectToClipPos( o.vertex );
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
