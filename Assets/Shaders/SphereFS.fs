in vec3 Normal;
out vec4 Output;

layout(binding = 0) uniform samplerCube CubeMap;

void main()
{
	vec3 color = texture(CubeMap, Normal).xyz;
	Output = vec4(color, 1);
}

---

struct PS_INPUT
{
	float3 Normal : NORMAL;
};

TextureCube CubeMap : register(t0);
SamplerState CubeMapSampler : register(s0);

float4 Main(PS_INPUT input) : SV_Target
{
	return CubeMap.Sample(CubeMapSampler, input.Normal);
}