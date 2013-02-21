layout(std140, binding = 0) uniform PerFrameConstants
{ 
	mat4 Projection;
};

layout(std140, binding = 1) uniform PerObjectConstants
{ 
	mat4 World;
};

layout(location = 0) in vec4 VertexPosition;
layout(location = 1) in vec4 VertexColor;
layout(location = 2) in vec2 VertexTexCoords;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec2 TexCoords;
out vec4 Color;

void main()
{
	gl_Position = Projection * World * VertexPosition;
	TexCoords = VertexTexCoords;

	Color = VertexColor;
} 

---

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