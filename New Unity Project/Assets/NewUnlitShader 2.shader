Shader "Unlit/NewUnlitShader 2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
      _Point ("a point in world space", Vector) = (0., 0., 0., 1.0)
      _DistanceNear ("threshold distance", Float) = 5.0
      _ColorNear ("color near to point", Color) = (0.0, 1.0, 0.0, 1.0)
      _ColorFar ("color far from point", Color) = (0.3, 0.3, 0.3, 1.0)
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

			struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
         };

         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output; 
 
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.position_in_world_space = 
               mul(_Object2World, input.vertex);
               // transformation of input.vertex from object 
               // coordinates to world coordinates;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
             float dist = distance(input.position_in_world_space, 
               float4(0.0, 0.0, 0.0, 1.0));
               // computes the distance between the fragment position 
               // and the origin (the 4th coordinate should always be 
               // 1 for points).
            
            if (dist < 5.0)
            {
               return float4(0.0, 1.0, 0.0, 1.0); 
                  // color near origin
            }
            else
            {
               return float4(0.1, 0.1, 0.1, 1.0); 
                  // color far from origin
            }
         }
 



			ENDCG
		}
	}
}
