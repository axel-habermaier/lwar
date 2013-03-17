in vec2 TexCoords;

layout(binding = 0) uniform sampler2D InputTexture;

out vec4 Output;

void main()
{
    Output = texture(InputTexture, TexCoords);
}

---

struct PS_INPUT
{
	float2 TexCoords : TEXCOORD0;
};

Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

float4 Main(PS_INPUT input) : SV_TARGET
{
    return InputTexture.Sample(InputSampler, input.TexCoords);
}