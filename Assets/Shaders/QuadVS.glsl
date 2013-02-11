#version 330
#extension GL_ARB_shading_language_420pack : enable
#extension GL_ARB_separate_shader_objects : enable

layout(std140, binding = 0) uniform PerFrameConstants
{ 
	mat4 View;
	mat4 Projection;
	mat4 ViewProjection;
};

layout(std140, binding = 1) uniform PerObjectConstants
{ 
	mat4 World;
};

layout(location = 0) in vec4 VertexPosition;
layout(location = 2) in vec2 VertexTexCoords;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec2 TexCoords;

void main()
{
	gl_Position = ViewProjection * World * VertexPosition;
	TexCoords = VertexTexCoords;
}