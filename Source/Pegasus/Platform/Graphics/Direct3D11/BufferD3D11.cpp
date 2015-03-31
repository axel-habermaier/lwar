#include "Prelude.hpp"

using namespace Direct3D11;

Buffer* GraphicsDevice::InitializeBuffer(BufferDescription* desc)
{
	D3D11_BIND_FLAG bindFlag;

	switch (desc->Type)
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
	dataResource.pSysMem = desc->Data;
	dataResource.SysMemPitch = 0;
	dataResource.SysMemSlicePitch = 0;

	D3D11_BUFFER_DESC d3d11Desc;
	d3d11Desc.BindFlags = static_cast<UINT>(bindFlag);
	d3d11Desc.CPUAccessFlags = desc->Usage == ResourceUsage::Dynamic ? D3D11_CPU_ACCESS_WRITE : 0u;
	d3d11Desc.ByteWidth = safe_static_cast<UINT>(desc->SizeInBytes);
	d3d11Desc.MiscFlags = 0;
	d3d11Desc.StructureByteStride = 0;
	d3d11Desc.Usage = Map(desc->Usage);

	auto buffer = PG_NEW(Buffer);
	buffer->SizeInBytes = desc->SizeInBytes;

	CheckResult(_device->CreateBuffer(&d3d11Desc, desc->Data == nullptr ? nullptr : &dataResource, &buffer->Obj), "Failed to create buffer.");
	return buffer;
}

void GraphicsDevice::FreeBuffer(Buffer* buffer)
{
	PG_DELETE(buffer);
}

void* GraphicsDevice::MapBuffer(Buffer* buffer, MapMode mode)
{
	D3D11_MAPPED_SUBRESOURCE subResource;
	CheckResult(_context->Map(buffer->Obj.Get(), 0, Map(mode), 0, &subResource), "Failed to map buffer.");

	return subResource.pData;
}

void* GraphicsDevice::MapBufferRange(Buffer* buffer, MapMode mode, int32 offsetInBytes, int32 byteCount)
{
	PG_UNUSED(byteCount);
	return static_cast<byte*>(MapBuffer(buffer, mode)) + offsetInBytes;
}

void GraphicsDevice::UnmapBuffer(Buffer* buffer)
{
	_context->Unmap(buffer->Obj.Get(), 0);
}

void GraphicsDevice::BindBuffer(Buffer* buffer, int32 slot)
{
	_context->VSSetConstantBuffers(safe_static_cast<UINT>(slot), 1, buffer->Obj.GetAddressOf());
	_context->PSSetConstantBuffers(safe_static_cast<UINT>(slot), 1, buffer->Obj.GetAddressOf());
}

void GraphicsDevice::CopyBuffer(Buffer* buffer, void* data)
{
	Memory::Copy(MapBuffer(buffer, MapMode::WriteDiscard), data, safe_static_cast<UINT>(buffer->SizeInBytes));
	UnmapBuffer(buffer);
}

void GraphicsDevice::SetBufferName(Buffer* buffer, const char* name)
{
	SetName(buffer->Obj, name);
}
