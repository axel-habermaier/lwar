#include "Prelude.hpp"

using namespace Direct3D11;

Shader* GraphicsDevice::InitializeShader(ShaderType shaderType, byte* shaderCode, int32 sizeInBytes)
{
	ID3D11VertexShader* vertexShader;
	ID3D11PixelShader* pixelShader;

	auto size = safe_static_cast<UINT>(sizeInBytes);
	auto shader = PG_NEW(Shader);

	switch (shaderType)
	{
	case ShaderType::VertexShader:
		CheckResult(_device->CreateVertexShader(shaderCode, size, nullptr, &vertexShader), "Failed to create vertex shader.");
		shader->Obj.Attach(vertexShader);
		break;
	case ShaderType::FragmentShader:
		CheckResult(_device->CreatePixelShader(shaderCode, size, nullptr, &pixelShader), "Failed to create pixel shader.");
		shader->Obj.Attach(pixelShader);
		break;
	case ShaderType::GeometryShader:
		PG_ASSERT_NOT_REACHED("Not implemented");
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	return shader;
}

void GraphicsDevice::FreeShader(Shader* shader)
{
	PG_DELETE(shader);
}

void GraphicsDevice::SetShaderName(Shader* shader, const char* name)
{
	SetName(shader->Obj, name);
}

ShaderProgram* GraphicsDevice::InitializeShaderProgram(Shader* vertexShader, Shader* pixelShader)
{
	auto shaderProgram = PG_NEW(ShaderProgram);
	vertexShader->Obj.As(&shaderProgram->VertexShader);
	pixelShader->Obj.As(&shaderProgram->PixelShader);
	return shaderProgram;
}

void GraphicsDevice::FreeShaderProgram(ShaderProgram* shaderProgram)
{
	PG_DELETE(shaderProgram);
}

void GraphicsDevice::BindShaderProgram(ShaderProgram* shaderProgram)
{
	if (Graphics::ChangeState(&_boundVertexShader, shaderProgram->VertexShader.Get()))
		_context->VSSetShader(shaderProgram->VertexShader.Get(), nullptr, 0);

	if (Graphics::ChangeState(&_boundPixelShader, shaderProgram->PixelShader.Get()))
		_context->PSSetShader(shaderProgram->PixelShader.Get(), nullptr, 0);
}
