Shader "Unlit/NewUnlitShader 3"
{
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			float4 vert (float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
			
			float4 frag (void) : COLOR
			{
				return float4(0.0, 1.0, 0.0, 0.3);
			}
			ENDCG
		}
	}
}
