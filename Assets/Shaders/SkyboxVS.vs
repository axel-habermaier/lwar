layout(std140, binding = 0) uniform PerFrameConstants
{ 
	mat4 View;
	mat4 Projection;
	mat4 ViewProjection;
	//vec2 ViewportSize;
};

layout(location = 0) in vec4 VertexPosition;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec3 TexCoords;

void main()
{
	mat4 rotationMatrix = View;
	rotationMatrix[3][0] = 0; 
	rotationMatrix[3][1] = 0;
	rotationMatrix[3][2] = 0;

	vec4 position = rotationMatrix * VertexPosition;
	position = Projection * position;
	position.z += 1;

	gl_Position = position; 
	TexCoords = VertexPosition.xyz;
}

---

cbuffer PerFrameConstants : register(b0)
{ 
	column_major matrix	View;
	column_major matrix Projection;
	column_major matrix	ViewProjection;
	float2 ViewportSize;
};

struct VS_INPUT
{
	float4 Position		: POSITION;
};

struct VS_OUTPUT
{
	float3 TexCoords	: TEXCOORD0;
	float4 Position		: SV_Position;
};

VS_OUTPUT Main(VS_INPUT input)
{
	VS_OUTPUT output;

	float4x4 rotationMatrix = View;
	rotationMatrix[0][3] = 0; 
	rotationMatrix[1][3] = 0;
	rotationMatrix[2][3] = 0;

	float4 position = mul(rotationMatrix, input.Position);
	position = mul(Projection, position);
	position.z += 1;

	output.Position = position; 
	output.TexCoords = input.Position.xyz;

	return output;
}