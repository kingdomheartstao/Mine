Shader "Unlit/Cutaways"
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
			Cull Off
			 
			CGPROGRAM

			 #pragma vertex vert  
				#pragma fragment frag 

			 struct vertexInput {
				float4 vertex : POSITION;
			 };
			 struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posInObj : TEXCOORD0;
			 };

			 vertexOutput vert(vertexInput input){
				vertexOutput output;

				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.posInObj = input.vertex;

				return output;
			 }

			 float4 frag(vertexOutput input) : COLOR{
				if (input.posInObj.y > 0.0){
					discard;
				}
				return float4(0.0, 1.0, 0.0, 1.0);
			 }

			ENDCG
		}
	}
}
