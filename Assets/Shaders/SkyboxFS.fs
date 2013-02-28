in vec3 TexCoords;

layout(binding = 0) uniform samplerCube CubeMap;

out vec4 Output;

void main()
{
	Output = texture(CubeMap, TexCoords);
}

---

struct PS_INPUT
{
	float3 TexCoords	: TEXCOORD0;
};

TextureCube CubeMap : register(t0);
SamplerState CubeMapSampler : register(s0);

float4 Main(PS_INPUT input) : SV_Target
{
	return CubeMap.Sample(CubeMapSampler, input.TexCoords);
}