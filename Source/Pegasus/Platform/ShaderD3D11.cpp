#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/Shader.hpp"
#include "Graphics/GraphicsDevice.hpp"

void GraphicsDevice::Initialize(Shader* shader, const byte* shaderCode, uint32 sizeInBytes)
{
	switch (shader->_type)
	{
	case ShaderType::VertexShader:
	{
		auto vertexShaderPtr = reinterpret_cast<ID3D11VertexShader**>(&shader->_shader);
		Direct3D11::CheckResult(_device->CreateVertexShader(shaderCode, sizeInBytes, nullptr, vertexShaderPtr), "Failed to create vertex shader.");
		break;
	}
	case ShaderType::FragmentShader:
	{
		auto fragmentShaderPtr = reinterpret_cast<ID3D11PixelShader**>(&shader->_shader);
		Direct3D11::CheckResult(_device->CreatePixelShader(shaderCode, sizeInBytes, nullptr, fragmentShaderPtr), "Failed to create pixel shader.");
		break;
	}
	default:
		PG_NO_SWITCH_DEFAULT;
	}
}

void GraphicsDevice::Free(Shader* shader)
{
	switch (shader->_type)
	{
	case ShaderType::VertexShader:
		UnsetState(&_boundVertexShader, static_cast<VertexShader*>(shader));
		break;
	case ShaderType::FragmentShader:
		UnsetState(&_boundFragmentShader, static_cast<FragmentShader*>(shader));
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	Direct3D11::Release(shader->_shader);
}

void GraphicsDevice::SetName(Shader* shader, const char* name)
{
	Direct3D11::SetName(static_cast<ID3D11DeviceChild*>(shader->_shader), name);
}

void GraphicsDevice::Initialize(ShaderProgram* shaderProgram, VertexShader& vertexShader, FragmentShader& fragmentShader)
{
	shaderProgram->_vertexShader = &vertexShader;
	shaderProgram->_fragmentShader = &fragmentShader;
}

void GraphicsDevice::Free(ShaderProgram* shaderProgram)
{
	UnsetState(&_boundShaderProgram, shaderProgram);
}

void GraphicsDevice::Bind(const ShaderProgram* shaderProgram)
{
	if (!ChangeState(&_boundShaderProgram, shaderProgram))
		return;

	if (ChangeState(&_boundVertexShader, shaderProgram->_vertexShader))
		_context->VSSetShader(static_cast<ID3D11VertexShader*>(shaderProgram->_vertexShader->_shader), nullptr, 0);

	if (ChangeState(&_boundFragmentShader, shaderProgram->_fragmentShader))
		_context->PSSetShader(static_cast<ID3D11PixelShader*>(shaderProgram->_fragmentShader->_shader), nullptr, 0);
}

#endif