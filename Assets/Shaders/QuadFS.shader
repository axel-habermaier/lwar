in vec2 TexCoords;

layout(binding = 0) uniform sampler2D Tex;

out vec4 Output;

void main()
{
	Output = texture(Tex, TexCoords);
}

---

struct PS_INPUT
{
	float2 TexCoords	: TEXCOORD0;
};

Texture2D Tex : register(t0);
SamplerState TexSampler : register(s0);

float4 Main(PS_INPUT input) : SV_Target
{
	return Tex.Sample(TexSampler, input.TexCoords);
}