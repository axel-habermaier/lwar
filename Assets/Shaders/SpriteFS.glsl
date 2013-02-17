in vec2 TexCoords;
in vec4 Color;

layout(binding = 0) uniform sampler2D Tex;

out vec4 Output;

void main()
{
	Output = texture2D(Tex, TexCoords) * Color;
}