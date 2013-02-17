cbuffer PerFrameConstants : register(b0)
{ 
	column_major matrix Projection;
};

cbuffer PerObjectConstants : register(b1)
{
	column_major matrix World;
}

struct VS_INPUT
{
	float4 Position		: POSITION;
	float2 TexCoords	: TEXCOORD0;
	float4 Color		: COLOR0;
};

struct VS_OUTPUT
{
	float2 TexCoords	: TEXCOORD0;
	float4 Color		: COLOR0;
	float4 Position		: SV_Position;
};

VS_OUTPUT Main(VS_INPUT input)
{
	VS_OUTPUT output;

	float4 position = mul(World, input.Position);
	output.Position = mul(Projection, position);

	output.Color = input.Color;
	output.TexCoords = input.TexCoords;

	return output;
}