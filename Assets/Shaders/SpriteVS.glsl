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