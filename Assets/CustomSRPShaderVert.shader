// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/CustomSRPShaderVert"
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
                fixed3 color : COLOR;
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

                fixed3 diffuse = _LightColor * saturate(dot(v.normal, _V4LightDir));
                
                fixed3 viewDir = normalize(_CameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                fixed3 reflectDir = -normalize(reflect(_V4LightDir, o.normal));
                fixed3 specular = pow(saturate(dot(reflectDir, viewDir)), _Gloss);

                o.color = diffuse + specular;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.color, 1);
            }
            ENDCG
        }
    }
}
