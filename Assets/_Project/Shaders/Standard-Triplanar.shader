Shader "Custom/Standard-Triplanar" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			// float2 uv_MainTex;
            float3 localCoord;
			float3 localNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Scale;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		
        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = v.vertex.xyz;
            data.localNormal = v.normal.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			  // Blending factor of triplanar mapping
            float3 bf = normalize(abs(IN.localNormal));
            bf /= dot(bf, (float3)1);

            // Triplanar mapping
            float2 tx = IN.localCoord.yz * _Scale;
            float2 ty = IN.localCoord.zx * _Scale;
            float2 tz = IN.localCoord.xy * _Scale;

            // Base color
            half4 cx = tex2D(_MainTex, tx) * bf.x;
            half4 cy = tex2D(_MainTex, ty) * bf.y;
            half4 cz = tex2D(_MainTex, tz) * bf.z;
			half4 color = (cx + cy + cz) * _Color;

			fixed4 c =  color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
