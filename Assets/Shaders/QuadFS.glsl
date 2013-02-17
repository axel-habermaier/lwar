in vec2 TexCoords;

layout(binding = 0) uniform sampler2D Tex;

out vec4 Output;

void main()
{
	Output = texture(Tex, TexCoords);
}