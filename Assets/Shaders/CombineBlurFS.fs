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

struct PS_INPUT
{
	float2 TexCoords : TEXCOORD0;
};

Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

Texture2D InputTexture2 : register(t1);
SamplerState InputSampler2 : register(s1);

float4 Main(PS_INPUT input) : SV_TARGET
{
//    float4 color = InputTexture.Sample(InputSampler, input.TexCoords);
//    return color+InputTexture2.Sample(InputSampler2, input.TexCoords);
return InputTexture.Sample(InputSampler, input.TexCoords);
}