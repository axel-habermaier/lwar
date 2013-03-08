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
	float3 TexCoords0 : TEXCOORD0;
    float3 TexCoords1 : TEXCOORD1;
};

TextureCube CubeMap : register(t0);
SamplerState CubeMapSampler : register(s0);

Texture2D HeatTexture : register(t1);
SamplerState HeatSampler : register(s1);

float4 Main(PS_INPUT input) : SV_Target
{
	float sample1 = CubeMap.Sample(CubeMapSampler, input.TexCoords0).r;
    float sample2 = CubeMap.Sample(CubeMapSampler, input.TexCoords1).r;

    float result = sample1 + sample2;
    float delta = 1;
    if (result > delta)
        return HeatTexture.Sample(HeatSampler, float2(result-delta, 0));
    else
        return float4(0, 0, 0, 0);
}
