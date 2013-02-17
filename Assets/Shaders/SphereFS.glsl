in vec3 Normal;
out vec4 Output;

layout(binding = 0) uniform samplerCube CubeMap;

void main()
{
	vec3 color = texture(CubeMap, Normal).xyz;
	Output = vec4(color, 1);
}