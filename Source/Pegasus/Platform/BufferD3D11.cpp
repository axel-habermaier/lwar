#include "Direct3D11.hpp"

#ifdef PG_GRAPHICS_DIRECT3D11

#include "Graphics/Buffer.hpp"
#include "Graphics/GraphicsDevice.hpp"
#include "Utilities/Memory.hpp"

void GraphicsDevice::Initialize(Buffer* buffer, ResourceUsage usage, const void* data)
{
	D3D11_BIND_FLAG bindFlag;

	switch (buffer->_type)
	{
	case BufferType::ConstantBuffer:
		bindFlag = D3D11_BIND_CONSTANT_BUFFER;
		break;
	case BufferType::IndexBuffer:
		bindFlag = D3D11_BIND_INDEX_BUFFER;
		break;
	case BufferType::VertexBuffer:
		bindFlag = D3D11_BIND_VERTEX_BUFFER;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	D3D11_SUBRESOURCE_DATA dataResource;
	dataResource.pSysMem = data;
	dataResource.SysMemPitch = 0;
	dataResource.SysMemSlicePitch = 0;

	D3D11_BUFFER_DESC desc;
	desc.BindFlags = static_cast<UINT>(bindFlag);
	desc.CPUAccessFlags = usage == ResourceUsage::Dynamic ? D3D11_CPU_ACCESS_WRITE : 0u;
	desc.ByteWidth = buffer->_sizeInBytes;
	desc.MiscFlags = 0;
	desc.StructureByteStride = 0;
	desc.Usage = Direct3D11::Map(usage);

	Direct3D11::CheckResult(_device->CreateBuffer(&desc, data == nullptr ? nullptr : &dataResource, &buffer->_buffer),
							"Failed to create buffer.");
}

void GraphicsDevice::Free(Buffer* buffer)
{
	for (auto i = 0; i < Graphics::ConstantBufferSlotCount && buffer->_type == BufferType::ConstantBuffer; ++i)
		UnsetState(&_boundConstantBuffers[i], static_cast<ConstantBuffer*>(buffer));

	Direct3D11::Release(buffer->_buffer);
}

void* GraphicsDevice::Map(Buffer* buffer, MapMode mode)
{
	D3D11_MAPPED_SUBRESOURCE subResource;
	Direct3D11::CheckResult(_context->Map(buffer->_buffer, 0, Direct3D11::Map(mode), 0, &subResource), "Failed to map buffer.");

	return subResource.pData;
}

void* GraphicsDevice::MapRange(Buffer* buffer, MapMode mode, uint32 offsetInBytes, uint32 byteCount)
{
	PG_UNUSED(byteCount);

	auto ptr = Map(buffer, mode);
	return offsetInBytes + static_cast<byte*>(ptr);
}

void GraphicsDevice::Unmap(Buffer* buffer)
{
	_context->Unmap(buffer->_buffer, 0);
}

void GraphicsDevice::Bind(const ConstantBuffer* buffer, uint32 slot)
{
	if (!ChangeState(&_boundConstantBuffers[slot], buffer))
		return;

	_context->VSSetConstantBuffers(slot, 1, &buffer->_buffer);
	_context->PSSetConstantBuffers(slot, 1, &buffer->_buffer);
}

void GraphicsDevice::Copy(ConstantBuffer* buffer, const void* data)
{
	auto bufferData = Map(buffer, MapMode::WriteDiscard);
	Memory::Copy(bufferData, data, buffer->_sizeInBytes);
	Unmap(buffer);
}

void GraphicsDevice::SetName(Buffer* buffer, const char* name)
{
	Direct3D11::SetName(buffer->_buffer, name);
}

#endif