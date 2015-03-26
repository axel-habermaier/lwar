#include "Prelude.hpp"

using namespace OpenGL3;

Shader* GraphicsDevice::InitializeShader(ShaderType shaderType, byte* shaderCode, int32 sizeInBytes)
{
	uint32 type;
	switch (shaderType)
	{
	case ShaderType::VertexShader:
		type = GL_VERTEX_SHADER;
		break;
	case ShaderType::FragmentShader:
		type = GL_FRAGMENT_SHADER;
		break;
	case ShaderType::GeometryShader:
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	auto shader = PG_NEW(Shader);
	shader->Obj = glCreateShader(type);

	if (shader->Obj == 0)
		PG_DIE("Failed to create OpenGL shader object.");

	glShaderSource(shader->Obj, 1, &shaderCode, &sizeInBytes);
	glCompileShader(shader->Obj);

	char buffer[4096];
	int32 success, logLength;

	glGetShaderiv(shader->Obj, GL_COMPILE_STATUS, &success);
	glGetShaderInfoLog(shader->Obj, sizeof(buffer) / sizeof(char), &logLength, buffer);

	if (success == GL_FALSE)
		PG_DIE("Shader compilation failed: %s", buffer);

	if (logLength != 0)
		PG_WARN("%s", buffer);

	return shader;
}

void GraphicsDevice::FreeShader(Shader* shader)
{
	if (shader == nullptr)
		return;

	glDeleteShader(shader->Obj);
	PG_DELETE(shader);
}

void GraphicsDevice::SetShaderName(Shader* shader, const char* name)
{
	PG_UNUSED(shader);
	PG_UNUSED(name);
}

ShaderProgram* GraphicsDevice::InitializeShaderProgram(Shader* vertexShader, Shader* fragmentShader)
{
	auto program = PG_NEW(ShaderProgram);
	program->Obj = glCreateProgram();
	if (program == 0)
		PG_DIE("Failed to create OpenGL program object.");

	glAttachShader(program->Obj, vertexShader->Obj);
	glAttachShader(program->Obj, fragmentShader->Obj);
	glLinkProgram(program->Obj);

	char buffer[4096];
	int32 success, logLength;

	glGetProgramiv(program->Obj, GL_LINK_STATUS, &success);
	glGetProgramInfoLog(program->Obj, sizeof(buffer) / sizeof(char), &logLength, buffer);

	if (success == GL_FALSE)
		PG_DIE("Program linking failed: %s", buffer);

	if (logLength != 0)
		PG_WARN("%s", buffer);

	glDetachShader(program->Obj, vertexShader->Obj);
	glDetachShader(program->Obj, fragmentShader->Obj);

	return program;
}

void GraphicsDevice::FreeShaderProgram(ShaderProgram* shaderProgram)
{
	if (shaderProgram == nullptr)
		return;

	glDeleteProgram(shaderProgram->Obj);
	PG_DELETE(shaderProgram);
}

void GraphicsDevice::BindShaderProgram(ShaderProgram* shaderProgram)
{
	glUseProgram(shaderProgram->Obj);
}