﻿// Copyright (C) 2018 Enkisoftware Limited - All rights reserved

// This source file is licensed as part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

Shader "Mercuna/NavOctreeTimeVaryingDebug"
{
	Properties
	{
		_SearchColor("SearchColor", Color) = (0,0.56,0,1)
		_CorridorColor("CorridorColor", Color) = (0,0,1,1)
		_LineWidth("LineWidth", Float) = 1.5
		_TimePeriod("TimePeriod", Float) = 10
	}
	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend One OneMinusSrcAlpha

		Pass
		{
			Cull Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag


			half4 _SearchColor, _CorridorColor;
			float _LineWidth, _TimePeriod;

			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float3  localPos : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(v.vertex);
				OUT.uv = v.texcoord;
				OUT.localPos = v.vertex.xyz;
				return OUT;
			}

			half4 frag(v2f IN) : COLOR
			{
				float t = clamp(1.5 * frac(_Time[1] / _TimePeriod), 0.0, 1.0);
				if (IN.uv[1] > t)
				{
					discard;
				}

				float3 scaledPos = IN.localPos / IN.uv[0];
				float3 gradX = ddx(scaledPos);
				float3 gradY = ddy(scaledPos);

				float3 grad = float3(length(float2(gradX.x, gradY.x)), length(float2(gradX.y, gradY.y)), length(float2(gradX.z, gradY.z)));

				float3 voxelPos = frac(scaledPos);
				float3 distToVoxelEdge = min(voxelPos, 1.0f - voxelPos);

				float3 a = _LineWidth * grad; // Scale by line width
				float3 b = grad + 0.0001; // Epsilon value to prevent inf on divide
				float3 c = distToVoxelEdge + 0.0001;
				float3 d = a - c;
				float3 e = d / b;
				float3 f = 0.1 / b; // Anti-aliasing

				float3 g = clamp(e, 0, 1) * clamp(f, 0, 1);
				float h = max(g.x, max(g.y, g.z));

				if (IN.uv[0] > 0.0f)
				{
					return _CorridorColor * h * 0.5f;
				}
				else
				{
					return _SearchColor * h * 0.5f;
				}
			}

			ENDCG

		}
	}
}