in vec3 TexCoords0;
in vec3 TexCoords1;

layout(location = 0) out vec4 Output;

layout(binding = 0) uniform samplerCube CubeMap;
layout(binding = 1) uniform sampler2D HeatTexture;

void main()
{
	float sample1 = texture(CubeMap, TexCoords0).r;
    float sample2 = texture(CubeMap, TexCoords1).r;

    float result = sample1 + sample2;
    float blend = result / 2;

    vec4 color = texture(HeatTexture, vec2(blend, 0));
    Output = vec4(color.rgb * blend, result / 4);
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

    float4 color = HeatTexture.Sample(HeatSampler, float2(result / 2, 0));

    float blend = result / 2;
    return float4(color.rgb * blend, result / 4);
}
