#include "Prelude.hpp"

using namespace Direct3D11;

VertexLayout* GraphicsDevice::InitializeVertexLayout(VertexLayoutDescription* desc)
{
	auto vertexLayout = PG_NEW(VertexLayout);
	vertexLayout->IndexBuffer = desc->IndexBuffer == nullptr ? nullptr : static_cast<Buffer*>(desc->IndexBuffer)->Obj;
	vertexLayout->IndexOffset = safe_static_cast<UINT>(desc->IndexOffset);
	vertexLayout->IndexFormat = Map(desc->IndexSize);
	vertexLayout->BindingsCount = desc->BindingsCount;

	for (auto i = 0; i < desc->BindingsCount; ++i)
	{
		vertexLayout->Bindings[i].Slot = static_cast<UINT>(desc->Bindings[i].Semantics);
		vertexLayout->Bindings[i].Stride = safe_static_cast<UINT>(desc->Bindings[i].Stride);
		vertexLayout->Bindings[i].Offset = safe_static_cast<UINT>(desc->Bindings[i].Offset);
		vertexLayout->Bindings[i].VertexBuffer = static_cast<Buffer*>(desc->Bindings[i].VertexBuffer)->Obj;
	}

	D3D11_INPUT_ELEMENT_DESC inputDescs[Graphics::MaxVertexBindings];

	for (auto i = 0; i < desc->BindingsCount; ++i)
	{
		inputDescs[i].AlignedByteOffset = 0;
		inputDescs[i].InputSlotClass = desc->Bindings[i].StepRate == 0 ? D3D11_INPUT_PER_VERTEX_DATA : D3D11_INPUT_PER_INSTANCE_DATA;
		inputDescs[i].InstanceDataStepRate = desc->Bindings[i].StepRate;
		inputDescs[i].Format = Map(desc->Bindings[i].Format);
		inputDescs[i].SemanticIndex = GetSemanticIndex(desc->Bindings[i].Semantics);
		inputDescs[i].SemanticName = GetSemanticName(desc->Bindings[i].Semantics);
		inputDescs[i].InputSlot = static_cast<UINT>(desc->Bindings[i].Semantics);
	}

	auto bindingsCount = safe_static_cast<UINT>(desc->BindingsCount);
	auto signatureLength = safe_static_cast<SIZE_T>(desc->SignatureLength);

	CheckResult(
		_device->CreateInputLayout(inputDescs, bindingsCount, desc->Signature, signatureLength, &vertexLayout->InputLayout),
		"Failed to create input layout.");

	return vertexLayout;
}

void GraphicsDevice::FreeVertexLayout(VertexLayout* vertexLayout)
{
	PG_DELETE(vertexLayout);
}

void GraphicsDevice::BindVertexLayout(VertexLayout* vertexLayout)
{
	_context->IASetInputLayout(vertexLayout->InputLayout.Get());

	for (auto i = 0; i < vertexLayout->BindingsCount; ++i)
	{
		auto& binding = vertexLayout->Bindings[i];
		_context->IASetVertexBuffers(binding.Slot, 1, binding.VertexBuffer.GetAddressOf(), &binding.Stride, &binding.Offset);
	}

	if (vertexLayout->IndexBuffer != nullptr)
		_context->IASetIndexBuffer(vertexLayout->IndexBuffer.Get(), vertexLayout->IndexFormat, vertexLayout->IndexOffset);
}
