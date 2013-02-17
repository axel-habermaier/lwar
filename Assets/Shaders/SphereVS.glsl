layout(std140, binding = 0) uniform PerFrameConstants
{ 
	mat4 View;
	mat4 Projection;
	mat4 ViewProjection;
};

layout(std140, binding = 1) uniform PerObjectConstants
{ 
	mat4 Model;
};

layout(location = 0) in vec4 VertexPosition;
layout(location = 3) in vec3 VertexNormal;

out gl_PerVertex
{
    vec4 gl_Position;
};

out vec3 Normal;

void main()
{
	gl_Position = ViewProjection * Model * VertexPosition;
	Normal = VertexNormal;
}