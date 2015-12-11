Shader "Unlit/Transparency2"
{
	SubShader
	{
		Tags { "Queue"="Transparent" }

		Pass //COLOR INSIDE,MIXED WITH OUTSIDE
		{
			Cull Front

			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			float4 vert (float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
			
			float4 frag (void) : COLOR
			{
				return float4(1.0, 0.0, 0.0, 0.3);
			}
			ENDCG
		}

		Pass //While block the face inside,only can see outside transparency 
		{
			Cull Back

			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			float4 vert(float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}

			float4 frag(void) : COLOR
			{
				return float4(0.0, 1.0, 0.0, 0.3);
			}

			ENDCG
		}
	}
}
