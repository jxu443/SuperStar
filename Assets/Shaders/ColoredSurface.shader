Shader "Custom/ColoredSurface"
{
    Properties
    {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        CGPROGRAM
        
        //standard lighting and full support for shadows.
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0
        struct Input {
			float3 worldPos;
		}; 

        float _Smoothness;
        
        // inout: indicates that it's both passed to the function and used for the result of the function.
        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface) {
            //surface.Smoothness = 0.5; // default is 0
            surface.Smoothness = _Smoothness; // make smoothness appear in GUI, combine with Properties 

            surface.Albedo.x = saturate((input.worldPos.x + 21) * 0.05 + 0.5); // three components
            surface.Albedo.y = saturate((input.worldPos.y + 5) * 0.05 + 0.5);
            surface.Albedo.z = saturate((input.worldPos.z - 62) * 0.05 + 0.5);
        }



		ENDCG // a hybrid of CG and HLSL, two shader languages. 
        
    }
    
    FallBack "Diffuse" //add a fallback to the standard diffuse shader
}

