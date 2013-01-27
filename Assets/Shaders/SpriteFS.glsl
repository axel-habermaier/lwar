#version 330
#extension GL_ARB_shading_language_420pack : enable
#extension GL_ARB_separate_shader_objects : enable

in vec2 TexCoords;
in vec4 Color;

layout(binding = 0) uniform sampler2D Tex;

out vec4 Output;

void main()
{
	Output = texture2D(Tex, TexCoords) * Color;
}