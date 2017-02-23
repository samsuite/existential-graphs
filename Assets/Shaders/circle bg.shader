﻿Shader "Cut Layer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
    SubShader
    {
        Tags { "RenderType"="CutLayer" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f {
                fixed4 position : SV_POSITION;
            };
 
            v2f vert() {
                v2f o;
                o.position = fixed4(0,0,0,0);
                return o;
            }
 
            fixed4 frag() : COLOR {
                return fixed4(1,0,0,1);
            }
            ENDCG
        }
    }
}