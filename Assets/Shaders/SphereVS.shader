layout(std140, binding = 0) uniform PerFrameConstants
{ 
	mat4 View;
	mat4 Projection;
	mat4 ViewProjection;
};

layout(std140, binding = 1) uniform PerObjectConstants
{ 
	mat4 Model;
};

layout(location = 0) in vec4 VertexPosition;
layout(location = 3) in vec3 VertexNormal;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec3 Normal;

void main()
{
	gl_Position = ViewProjection * Model * VertexPosition;
	Normal = VertexNormal;
}

---

cbuffer PerFrameConstants : register(b0)
{ 
	column_major matrix	View;
	column_major matrix Projection;
	column_major matrix	ViewProjection;
};

cbuffer PerObjectConstants : register(b1)
{
	column_major matrix World;
}

struct VS_INPUT
{
	float4 Position		: POSITION;
	float4 Normal		: NORMAL;
};

struct VS_OUTPUT
{
	float3 Normal		: NORMAL;
	float4 Position		: SV_Position;
};

VS_OUTPUT Main(VS_INPUT input)
{
	VS_OUTPUT output;

	float4 position = mul(World, input.Position);
	output.Position = mul(ViewProjection, position);
	output.Normal = input.Normal;

	return output;
}