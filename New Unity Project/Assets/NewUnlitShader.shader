
Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			#pragma multi_compile_fog
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : TEXCOORDO;
			};

			vertexOutput vert(float4 vertexPos : POSITION){
				vertexOutput output;

				output.pos = mul(mul(UNITY_MATRIX_P, UNITY_MATRIX_MV), vertexPos);
				output.col = vertexPos + float4(0.5, 0.5, 0.5, 0.0);
				//nomralize (-0.5 ~ 0.5 to 0.0 ~ 1.0)
				return output;
			}

			float4 frag(vertexOutput input) : COLOR 
			{
				return input.col; 
			}

			ENDCG
		}
	}
}
