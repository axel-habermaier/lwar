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