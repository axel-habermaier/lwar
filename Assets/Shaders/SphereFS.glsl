#version 330
#extension GL_ARB_shading_language_420pack : enable
#extension GL_ARB_separate_shader_objects : enable

in vec3 Normal;
out vec4 Output;

void main()
{
	Output = vec4(Normal, 1);
}