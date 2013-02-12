#ps_4_0

struct PS_INPUT
{
	float3 Normal : NORMAL;
};

float4 Main(PS_INPUT input) : SV_Target
{
	return float4(input.Normal, 1);
}