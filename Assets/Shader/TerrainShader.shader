Shader "Custom/TerrainShader"
{
    Properties
    {
        _DisplacementScale( "Displacement Scale", Range(0, 1000) ) = 1
        _DisplacementOffset( "Displacement Offset", Range(0, 1) ) = 0.5
        _Shininess( "Shininess", Range(1, 100) ) = 1.0
        _WaterLevel( "Height of Water Level", Range(0, 1) ) = 0.8

        _WaveAnimationSpeed( "Wave Animation Speed", Range(0.1, 10) ) = 1
        _WaterColor( "Color of the Water", Color ) = (0, 0.4, 0.7, 1)

        _AmbientReflectivity( "Ambient Reflectivity", Range(0, 1) ) = 0.5
        _DiffuseReflectivity( "Diffuse Reflectivity", Range(0, 1) ) = 0.5
        _SpecularReflectivity( "Specular Reflectivity", Range(0, 1) ) = 0.5

        _DisplacementTexture( "Displacement Texture", 2D ) = "default"
        _MoistureTexture( "Moisture Texture", 2D ) = "default"
        _ColorTexture( "Color Texture", 2D ) = "default"
        _NormalMapTexture1( "Normal Map Texture 1", 2D ) = "default"
        _NormalMapTexture2( "Normal Map Texture 2", 2D ) = "default"
    }
    SubShader
    {

        Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f {
                float4 vertex : SV_POSITION;                                    // Vertex in projected space
                float4 worldPosition : POSITIONT;                               // Vertex in world space
                float3 normal : NORMAL;                                         // Normal in world space
                float3 tangent : TANGENT0;                                      // Tangent in world space
                float3 bitangent : TANGENT1;                                    // Bitangent in world space
                float3 vectorToCamera : TEXCOORD0;                              // Direction from Vertex to Camera
                float2 terrainProperty : TEXCOORD1;                             // Terrain property (moisture, displacement)
                float2 uvCoord : TEXCOORD2;                                     // UV-Coordinates of Vertex (interpolated to fragment shader)
            };

            // Displacement Parameters
            sampler2D _DisplacementTexture;
            float4 _DisplacementTexture_ST;
            float _DisplacementScale;
            float _DisplacementOffset;
            float _WaterLevel;

            // Shading parameters
            sampler2D _MoistureTexture, _ColorTexture, _NormalMapTexture1, _NormalMapTexture2;
            float4 _MoistureTexture_ST, _ColorTexture_ST, _NormalMapTexture1_ST, _NormalMapTexture2_ST;
            float _AmbientReflectivity, _DiffuseReflectivity, _SpecularReflectivity;
            float _Shininess;
            float _WaveAnimationSpeed;
            fixed4 _WaterColor;

            v2f vert(appdata_tan v) {
                v2f o;

                // Textur-Koordinate
                float2 uv = v.texcoord.xy;
                o.uvCoord = uv;

                // Displacement Wert aus Textur
                fixed displacement = tex2Dlod( _DisplacementTexture, float4(TRANSFORM_TEX(uv, _DisplacementTexture), 0, 0) ).r;
                fixed moisture = tex2Dlod( _MoistureTexture, float4(TRANSFORM_TEX(uv, _MoistureTexture), 0, 0) ).r;

                // Moisture and displacement are used for accessing the color texture
                o.terrainProperty = float2( moisture, displacement );

                // If displacement is below the fluid threshold move it up to the treshold
                if( displacement < _WaterLevel ) {
                    displacement = _WaterLevel;
                }

                // Position in world coordinates
                o.worldPosition = mul( unity_ObjectToWorld, v.vertex );

                // Normal in world coordinates
                o.normal = normalize( UnityObjectToWorldNormal( v.normal ) );

                // Tangent in world coordinates
                o.tangent = normalize( UnityObjectToWorldDir( v.tangent ) );

                // Bitangent in world coordiantes (because other vectors are in world coordinates )
                o.bitangent = cross(o.normal, o.tangent);

                // Direction from vertex to camera
                o.vectorToCamera = normalize( WorldSpaceViewDir(v.vertex) );

                // Displacement
                // Use world position, where vertex is already scaled by model scale. Add the displacement to the scaled vertex position
                // (otherwise displacement would be scaled too: scale * (vertex.pos + displacement))
                o.vertex.w = 1;
                o.vertex.xyz = o.worldPosition + normalize( o.normal ) * displacement * _DisplacementScale * 2; // Multiply by ~2.0 to match unity transform units

                // Transform to projection space
                o.vertex = mul( UNITY_MATRIX_VP, o.vertex );

                return o;
            }

            fixed4 frag( v2f i ) : SV_Target {

                // Read and normalize normals from both water normal maps
                fixed3 val1 = tex2D( _NormalMapTexture1, i.uvCoord * float2(10, 10) + _WaveAnimationSpeed * _Time.xx ).rgb;
                float3 normal1 = normalize( val1 * 2.0 - 1.0 );

                fixed3 val2 = tex2D( _NormalMapTexture2, i.uvCoord * float2(10, 10) + _WaveAnimationSpeed * -_Time.xx ).rgb;
                float3 normal2 = normalize( val2 * 2.0 - 1.0 );

                // Matrix for converting tangent to normal space
                float3x3 Tangent2Normal = {
                            i.tangent.x,  i.bitangent.x,  i.normal.x, // row 1
                            i.tangent.y,  i.bitangent.y,  i.normal.y, // row 2
                            i.tangent.z,  i.bitangent.z,  i.normal.z  // row 3
                };

                // Transform (rotate) combined normal from both normal maps from tangent space to normal space
                float3 normal = mul( Tangent2Normal, normalize(normal1 + normal2) );

                // When the current fragment is not water dont use normal from normal map
                if( i.terrainProperty.y > _WaterLevel | _WaterLevel == 0) {
                    normal = i.normal;
                }

                fixed4 vAmbient = UNITY_LIGHTMODEL_AMBIENT;                                             // Ambient Light
                fixed4 vDiffuse = max(0, dot( _WorldSpaceLightPos0, normal )) * _LightColor0;           // Diffuse Light (Labmert)

                float3 reflectedLightVector = reflect( normalize(-_WorldSpaceLightPos0.xyz), normal );  // Specular Light (Phong)
                float x = max( 0, dot( reflectedLightVector, i.vectorToCamera ));
                fixed4 vSpecular = pow( x, _Shininess ) * _LightColor0;

                // If light shines on backside of vertex -> no specular reflection
                if( dot( _WorldSpaceLightPos0, normal ) < 0 ) {
                    vSpecular = fixed4(0, 0, 0, 0);
                }

                // Get color based on (moisture, displacement) from ColorTexture
                fixed4 color = tex2D( _ColorTexture, i.terrainProperty );

                // Set the color to water if beyond the water level
                if( i.terrainProperty.y < _WaterLevel & _WaterLevel != 0) {
                    color = _WaterColor;
                }

                // Specular (Reflection) on land is substantially lower than on water
                else {
                    vSpecular *= 0.0001;
                }

                // Calculated new light color based on ambient, diffuse and specular light and Keep Alpha
                color *= float4( _AmbientReflectivity * vAmbient + _DiffuseReflectivity * vDiffuse.rgb, color.a);
                color += _SpecularReflectivity * vSpecular;

                return color;
            }

            ENDCG
        }
    }
}
