layout(location = 0) in vec4 VertexPosition;
layout(location = 5) in vec2 VertexTexCoords;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec2 TexCoords;

void main()
{
	gl_Position = vec4(VertexPosition.x * -1, VertexPosition.z, 1, 1);
	TexCoords = vec2(1 - VertexTexCoords.x, 1 - VertexTexCoords.y);
}

---

struct VS_INPUT
{
	float4 Position		: POSITION;
	float2 TexCoords	: TEXCOORD0;
};

struct VS_OUTPUT
{
	float2 TexCoords	: TEXCOORD0;
	float4 Position		: SV_Position;
};

VS_OUTPUT Main(VS_INPUT input)
{
	VS_OUTPUT output;

	output.Position = float4(input.Position.x * -1, input.Position.z, 1, 1);
	output.TexCoords = float2(1 - input.TexCoords.x, input.TexCoords.y);

	return output;
}