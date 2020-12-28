Shader "Custom/TerrainShader"
{
    Properties
    {
        _DisplacementScale("Displacement Scale", Range(0, 1000)) = 0.5
        _DisplacementOffset("Displacement Offset", Range(0, 1)) = 0.5
        _Shininess("Shininess", Range(1, 100)) = 1.0
        _FluidThreshold("Threshold for fluigd", Range(0, 1)) = 0.8

        _AmbientReflectivity( "Ambient Reflectivity", Range(0, 1) ) = 0.5
        _DiffuseReflectivity( "Diffuse Reflectivity", Range(0, 1) ) = 0.5
        _SpecularReflectivity( "Specular Reflectivity", Range(0, 1) ) = 0.5

        _LightPos( "Position of Point Light", Vector ) = (0, 0, 0)

        _DisplacementTexture("Displacement Texture", 2D) = "default"
        _MoistureTexture("Moisture Texture", 2D) = "default"
        _ColorTexture("Color Texture", 2D) = "default"
        _NormalMapTexture("Normal Map Texture", 2D) = "default"
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
                float4 tangent : TANGENT0;
			};

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 worldPosition : POSITIONT;
                float3 norm : NORMAL;
                float3 tangent : TANGENT0;
                float3 bitangent : TANGENT1;
                float3 vectorToCamera : TEXCOORD0;
                float2 texCoord : TEXCOORD1;
                float2 uvCoord :TEXCOORD2;
            };

            // Displacement Parameters
            sampler2D _DisplacementTexture;
            float _DisplacementScale;
            float _DisplacementOffset;
            float _FluidThreshold;

            // Shading parameters
            sampler2D _MoistureTexture, _ColorTexture, _NormalMapTexture;
            float4 _LightPos;
            float _AmbientReflectivity, _DiffuseReflectivity, _SpecularReflectivity;
            float _Shininess;

            v2f vert(appdata v) {
                v2f o;

                // Textur-Koordinate
                float2 uv = v.tex.xy;
                o.uvCoord = uv;

                // Displacement Wert aus Textur
                fixed displacement = tex2Dlod( _DisplacementTexture, float4(uv, 0, 0) ).x;
                fixed moisture = tex2Dlod( _MoistureTexture, float4(uv, 0, 0) ).x;

                // Moisute and displacement are used for the texture coordinates at the color texture
                o.texCoord = float2( moisture, displacement );

                // If displacement is below the fluid threshold move it up to the treshold
                if( displacement < _FluidThreshold ) {
                    displacement = _FluidThreshold;
                }

                // Displacement
                o.vertex = v.vertex;
                o.vertex.xyz += normalize( v.norm ) * ( displacement / 255 ) * _DisplacementScale;

                // Position in world coordinates
                o.worldPosition = mul( unity_ObjectToWorld, v.vertex );

                // Normal in world coordinates
                o.norm = normalize( UnityObjectToWorldNormal( v.norm ) );

                // Tangent in world coordinates
                o.tangent = normalize( UnityObjectToWorldDir( v.tangent ) );

                // Bitangent in world coordiantes (because other vectors are in world coordinates )
                o.bitangent = cross(o.norm, o.tangent);

                // Sichtvector zu Kamera in Weltkoordinaten
                o.vectorToCamera = normalize( WorldSpaceViewDir(o.vertex) );

                // Vertex in view space transformieren
                o.vertex = UnityObjectToClipPos( o.vertex );

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {
                // Read normal from normal map
                fixed3 normalMapValue = tex2D( _NormalMapTexture, i.uvCoord ).rgb;                      // Get rgb value from normal map
                float3 normalTangentSpace = normalize( normalMapValue * 2.0 - 1.0 );                    // convert the rgb values (0 to 1) to normal vector values (-1 to 1)

                // Matrix for converting tangent to normal space
                float3x3 Tangent2Normal = {
                            i.tangent.x,  i.bitangent.x,  i.norm.x, // row 1
                            i.tangent.y,  i.bitangent.y,  i.norm.y, // row 2
                            i.tangent.z,  i.bitangent.z,  i.norm.z  // row 3
                };

                // Transform (rotate) normal from tangent space to normal space
                float3 normal = mul( Tangent2Normal, normalTangentSpace );

                // not water
                if( i.texCoord.y > _FluidThreshold ) {
                    normal = i.norm;
                }

                fixed4 vAmbient = UNITY_LIGHTMODEL_AMBIENT;                                             // Ambient Light
                fixed4 vDiffuse = max(0, dot( _WorldSpaceLightPos0, normal )) * _LightColor0;           // Diffuse Light (Labmert)

                float3 reflectedLightVector = reflect( normalize(-_WorldSpaceLightPos0.xyz), normal );   // Specular Light (Phong)
                float x = max( 0, dot( reflectedLightVector, i.vectorToCamera ));
                fixed4 vSpecular = pow( x, _Shininess ) * _LightColor0;

                // If light shines on backside of vertex -> no specular reflection
                if( dot( _WorldSpaceLightPos0, normal ) < 0 ) {
                    vSpecular = fixed4(0, 0, 0, 0);
                }

                // Get color based on (moisture, displacement) from ColorTexture
                fixed4 color = tex2D( _ColorTexture, i.texCoord );

                // Set the color to water if beyond the water level
                if( i.texCoord.y <= _FluidThreshold ) {
                    color = fixed4(0, 0.4, 0.7, 1);
                }

                // Specular (Reflection) on land is substantially lower than on water
                else {
                    vSpecular *= 0.0001;
                }

                // Calculated new light color based on ambient, diffuse and specular light
                color *= ( _AmbientReflectivity * vAmbient + _DiffuseReflectivity * vDiffuse );
                color += _SpecularReflectivity * vSpecular;

                return color;
            }

            ENDCG
        }
    }
}
