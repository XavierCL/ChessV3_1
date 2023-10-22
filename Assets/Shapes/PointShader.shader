Shader "Unlit/PointShader"
{
    Properties
    {
        _Color ("_Color", Color) = (.25, .5, .5, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centerOffset = (i.uv.xy - 0.5) * 2;
                float sqDist = dot(centerOffset, centerOffset);
                float dist = sqrt(sqDist);

                float delta = fwidth(dist);
                float alpha = 1 - smoothstep(1 - delta, 1 + delta, sqDist);

                return float4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}
