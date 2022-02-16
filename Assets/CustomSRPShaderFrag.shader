// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/CustomSRPShaderFrag"
{
    Properties
    {
        _Gloss("Gloss", Range(0, 10)) = 80
    }

    SubShader
    {
        Tags { "LightMode" = "CustomTag" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
            };

            //定义光照方向
            half4 _V4LightDir;
            fixed4 _LightColor;

            half4 _CameraPos;

            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 diffuse = saturate(dot(i.normal, _V4LightDir)) * _LightColor;

                fixed3 reflectDir = -normalize(reflect(_V4LightDir, i.normal));
                fixed3 viewDir = normalize(_CameraPos.xyz - i.worldPos.xyz);
                fixed3 specular = pow(saturate(dot(reflectDir, viewDir)), _Gloss);

                return fixed4(diffuse + specular, 1);
            }
            ENDCG
        }
    }
}
