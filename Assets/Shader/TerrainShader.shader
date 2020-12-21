Shader "Custom/TerrainShader"
{
    Properties
    {
       DisplacementTexture("DisplacementTexture", 2D) = "default"
       ColorTexture("Texture", 2D) = "default"
       Scale( "DisplacementScale", Float ) = 0.1
       Albedo("Albedo", Float) = 1.0
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
                float3 norm : NORMAL;
            };

            sampler2D DisplacementTexture;
            sampler2D ColorTexture;
            float Scale;
            float Albedo;

            v2f vert(appdata v) {
                v2f o;

                // Textur-Koordinate
                float2 uv = v.tex.xy;

                // Displacement Wert aus Textur
                float4 color = tex2Dlod( DisplacementTexture, float4(uv, 0, 0) );

                // Farb Wert aus Textur
                o.color = tex2Dlod( ColorTexture, float4(uv, 0, 0) );

                // Displacement
                o.vertex = v.vertex;
                o.vertex.xyz += v.norm * color * Scale;
                o.vertex = UnityObjectToClipPos( o.vertex );

                // Normale in Weltkoordinaten
                o.norm = UnityObjectToWorldNormal( v.norm );

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {
                return i.color * Albedo * ( UNITY_LIGHTMODEL_AMBIENT + max(0, dot( _WorldSpaceLightPos0, i.norm ) ) );
            }

            ENDCG
        }
    }
}
