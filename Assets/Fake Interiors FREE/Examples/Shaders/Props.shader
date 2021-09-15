// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Misc/Props"
{
	Properties
	{
		_Brick_A("Brick_A", 2D) = "white" {}
		_Brick_N("Brick_N", 2D) = "bump" {}
		_Brick_R("Brick_R", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Brick_N;
		uniform float4 _Brick_N_ST;
		uniform sampler2D _Brick_A;
		uniform float4 _Brick_A_ST;
		uniform sampler2D _Brick_R;
		uniform float4 _Brick_R_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Brick_N = i.uv_texcoord * _Brick_N_ST.xy + _Brick_N_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Brick_N, uv_Brick_N ) );
			float2 uv_Brick_A = i.uv_texcoord * _Brick_A_ST.xy + _Brick_A_ST.zw;
			o.Albedo = tex2D( _Brick_A, uv_Brick_A ).rgb;
			float2 uv_Brick_R = i.uv_texcoord * _Brick_R_ST.xy + _Brick_R_ST.zw;
			o.Smoothness = tex2D( _Brick_R, uv_Brick_R ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEFakeInspector"
}
/*ASEBEGIN
Version=13502
1956;-26;979;796;1310.442;778.6963;2.037682;True;False
Node;AmplifyShaderEditor.SamplerNode;72;-305.4977,284.1984;Float;True;Property;_Brick_R;Brick_R;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;69;-313.4977,-113.8016;Float;True;Property;_Brick_A;Brick_A;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;70;-312.4977,84.19843;Float;True;Property;_Brick_N;Brick_N;1;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;229,-93;Float;False;True;2;Float;ASEFakeInspector;0;0;Standard;Misc/Props;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;69;0
WireConnection;0;1;70;0
WireConnection;0;4;72;0
ASEEND*/
//CHKSM=F5802C09C7A63B81A9B635E5C658CDB62A85F653