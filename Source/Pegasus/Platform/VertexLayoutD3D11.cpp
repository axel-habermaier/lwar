#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/GraphicsDevice.hpp"
#include "Graphics/Buffer.hpp"
#include "Graphics/VertexLayout.hpp"

void GraphicsDevice::Initialize(VertexLayout* vertexLayout, const VertexLayoutDesc& desc)
{
	vertexLayout->_indexBuffer = desc.Indices;
	vertexLayout->_indexOffset = desc.IndexOffset;
	vertexLayout->_bindingsCount = desc.BindingsCount;

	for (size_t i = 0; i < desc.BindingsCount; ++i)
		vertexLayout->_bindings[i] = desc.Bindings[i];

	D3D11_INPUT_ELEMENT_DESC inputDescs[Graphics::MaxVertexBindings];

	for (auto i = 0u; i < desc.BindingsCount; ++i)
	{
		inputDescs[i].AlignedByteOffset = 0;
		inputDescs[i].InputSlotClass = desc.Bindings[i].InstanceDataStepRate == 0 ? D3D11_INPUT_PER_VERTEX_DATA : D3D11_INPUT_PER_INSTANCE_DATA;
		inputDescs[i].InstanceDataStepRate = desc.Bindings[i].InstanceDataStepRate;
		inputDescs[i].Format = Direct3D11::Map(desc.Bindings[i].Format);
		inputDescs[i].SemanticIndex = Direct3D11::GetSemanticIndex(desc.Bindings[i].Semantics);
		inputDescs[i].SemanticName = Direct3D11::GetSemanticName(desc.Bindings[i].Semantics);
		inputDescs[i].InputSlot = static_cast<UINT>(desc.Bindings[i].Semantics);
	}

	Direct3D11::CheckResult(_device->CreateInputLayout(inputDescs, desc.BindingsCount, desc.SignatureShader, desc.SignatureShaderLength, &vertexLayout->_inputLayout),
							"Failed to create input layout.");
}

void GraphicsDevice::Free(VertexLayout* vertexLayout)
{
	UnsetState(&_boundVertexLayout, vertexLayout);
	Direct3D11::Release(vertexLayout->_inputLayout);
}

void GraphicsDevice::Bind(const VertexLayout* vertexLayout)
{
	if (!ChangeState(&_boundVertexLayout, vertexLayout))
		return;

	_context->IASetInputLayout(vertexLayout->_inputLayout);

	for (auto i = 0u; i < vertexLayout->_bindingsCount; ++i)
	{
		auto& binding = vertexLayout->_bindings[i];
		auto slot = static_cast<UINT>(binding.Semantics);
		auto stride = safe_static_cast<UINT>(binding.Stride);
		auto offset = safe_static_cast<UINT>(binding.Offset);

		auto buffer = static_cast<ID3D11Buffer*>(binding.Vertices->_buffer);
		_context->IASetVertexBuffers(slot, 1, &buffer, &stride, &offset);
	}

	if (vertexLayout->_indexBuffer != nullptr)
	{
		auto buffer = static_cast<ID3D11Buffer*>(vertexLayout->_indexBuffer->_buffer);
		auto indexFormat = Direct3D11::Map(vertexLayout->_indexBuffer->GetIndexSize());
		_context->IASetIndexBuffer(buffer, indexFormat, vertexLayout->_indexOffset);
	}
}

#endif