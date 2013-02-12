#version 330
#extension GL_ARB_shading_language_420pack : enable
#extension GL_ARB_separate_shader_objects : enable

in vec3 Normal;
out vec4 Output;

layout(binding = 0) uniform samplerCube CubeMap;

void main()
{
	vec3 color = texture(CubeMap, Normal).xyz;
	Output = vec4(color, 1);
}