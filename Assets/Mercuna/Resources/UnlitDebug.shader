// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

Shader "Mercuna/UnlitDebug"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Fog{ Mode off }
		Tags{ "Queue" = "Transparent" }
		Pass
		{
			Color[_Color]
		}
	}
}
