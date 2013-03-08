uniform sampler2D image;

out vec4 FragmentColor;

uniform float offset[3] = float[]( 0.0, 1.3846153846, 3.2307692308 );
uniform float weight[3] = float[]( 0.2270270270, 0.3162162162, 0.0702702703 );

void main(void)
{
    FragmentColor = texture2D( image, vec2(gl_FragCoord)/1024.0 ) * weight[0];
    for (int i=1; i<3; i++) {
        FragmentColor +=
            texture2D( image, ( vec2(gl_FragCoord)+vec2(0.0, offset[i]) )/1024.0 )
                * weight[i];
        FragmentColor +=
            texture2D( image, ( vec2(gl_FragCoord)-vec2(0.0, offset[i]) )/1024.0 )
                * weight[i];
    }
}

---

cbuffer Data : register(b3)
{ 
	int Size;
    int Mipmap;
};

struct PS_INPUT
{
	float2 TexCoords : TEXCOORD0;
};

Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

static const float offsets[] = { 0.0, 1.3846153846, 3.2307692308 };
static const float weights[] = { 0.2270270270, 0.3162162162, 0.0702702703 };

float4 Main(PS_INPUT input) : SV_TARGET
{
    float4 color = InputTexture.SampleLevel(InputSampler, input.TexCoords, Mipmap) * weights[0];
    for (int i = 1; i < 3; ++i)
    {
        float2 offset = float2(0, offsets[i]);
        color += InputTexture.SampleLevel(InputSampler, (input.TexCoords * Size + offset) / Size, Mipmap) * weights[i];
        color += InputTexture.SampleLevel(InputSampler, (input.TexCoords * Size - offset) / Size, Mipmap) * weights[i];
    }
    return color;
}