struct PS_INPUT
{
	float2 TexCoords	: TEXCOORD0;
	float4 Color		: COLOR0;
};

Texture2D Tex : register(t0);

SamplerState TexSampler : register(s0)
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Main(PS_INPUT input) : SV_Target
{
	return Tex.Sample(TexSampler, input.TexCoords) * input.Color;
}