Shader "Custom/DisplacementShader"
{
    Properties
    {
       Displacement("Displacement", Float) = 0
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
				float3 norm : NORMAL;
			};

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            float Displacement;

            v2f vert(appdata v) {
                v2f o;

                o.vertex = UnityObjectToClipPos( v.vertex );
                o.vertex.y += Displacement;

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {
                return fixed4(255, 0, 0, 255);
            }

            ENDCG
        }
    }
}
