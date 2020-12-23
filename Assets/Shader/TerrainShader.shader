Shader "Custom/TerrainShader"
{
    Properties
    {
        _Albedo ("Albedo", Range(0, 1)) = 1.0
        _DisplacementScale ("Displacement Scale", Range(0, 1)) = 0.5
        _DisplacementOffset ("Displacement Offset", Range(0, 1)) = 0.5
        _Shininess ("Shininess", Range(0, 1)) = 1.0

        _DisplacementTexture ("Displacement Texture", 2D) = "default"
        _ColorTexture ("Texture", 2D) = "default"
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

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

            // Displacement Parameters
            sampler2D _DisplacementTexture;
            float _DisplacementScale;
            float _DisplacementOffset;

            // Shading parameters
            sampler2D _ColorTexture;
            float _Albedo;
            float _Shininess;

            v2f vert(appdata v) {
                v2f o;

                // Textur-Koordinate
                float2 uv = v.tex.xy;

                // Displacement Wert aus Textur
                float4 color = tex2Dlod( _DisplacementTexture, float4(uv, 0, 0) );

                // Farb Wert aus Textur
                o.color = tex2Dlod( _ColorTexture, float4(uv, 0, 0) );

                // Displacement
                o.vertex = v.vertex;
                o.vertex.xyz += v.norm * color * _DisplacementScale;
                o.vertex = UnityObjectToClipPos( o.vertex );

                // Normale in Weltkoordinaten
                o.norm = UnityObjectToWorldNormal( v.norm );

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {
                float iAmbient = UNITY_LIGHTMODEL_AMBIENT;                                  // Ambient Light
                float iDiffuse = _Albedo * max(0, dot( _WorldSpaceLightPos0, i.norm ));     // Diffuse Light (Labmert)
                float iSpecular = 0.0f;                                                     // Specular Light (Phong)

                float h = normalize( _WorldSpaceCameraPos + _WorldSpaceLightPos0 );
                float x = max( 0, dot( h, i.norm ));
                iSpecular = pow( x, _Shininess ) * (( _Shininess + 8 ) / 8 * 3.141 ) * _LightColor0;

                return i.color * ( iAmbient + iDiffuse + iSpecular );
            }

            ENDCG
        }
    }
}
