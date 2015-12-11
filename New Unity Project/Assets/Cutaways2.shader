Shader "Unlit/Cutaways2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
         Cull Front // cull only front faces
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posInObjectCoords : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.posInObjectCoords = input.vertex; 
 
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
            if (input.posInObjectCoords.y > 0.0) 
            {
               discard; // drop the fragment if y coordinate > 0
            }
            return float4(1.0, 0.0, 0.0, 1.0); // red
         }
 
         ENDCG  
      }

		Pass
		{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 posInObj : TEXCOORDO;
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
