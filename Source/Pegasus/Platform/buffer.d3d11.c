 #include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgResourceUsage usage, pgVoid* data)
{
	D3D11_BUFFER_DESC desc;
	D3D11_SUBRESOURCE_DATA dataResource;
	dataResource.pSysMem = data;
	dataResource.SysMemPitch = 0;
	dataResource.SysMemSlicePitch = 0;

	desc.BindFlags = pgConvertBufferType(buffer->type);
	desc.CPUAccessFlags = usage == PG_USAGE_DYNAMIC ? D3D11_CPU_ACCESS_WRITE : 0;
	desc.ByteWidth = buffer->size;
	desc.MiscFlags = 0;
	desc.StructureByteStride = 0;
	desc.Usage = pgConvertResourceUsage(usage);
	
	PG_D3DCALL(ID3D11Device_CreateBuffer(PG_DEVICE(buffer), &desc, data == NULL ? NULL : &dataResource, &buffer->ptr), 
		"Failed to create buffer.");
}

pgVoid pgDestroyBufferCore(pgBuffer* buffer)
{
	PG_SAFE_RELEASE(ID3D11Buffer, buffer->ptr);
}

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode)
{
	D3D11_MAPPED_SUBRESOURCE subResource;
	PG_D3DCALL(ID3D11DeviceContext_Map(PG_CONTEXT(buffer), (ID3D11Resource*)buffer->ptr, 0, pgConvertMapMode(mode), 0, &subResource), 
		"Failed to map buffer.");
	return subResource.pData;
}

pgVoid* pgMapBufferRangeCore(pgBuffer* buffer, pgMapMode mode, pgInt32 offset, pgInt32 byteCount)
{
	pgVoid* ptr;
	PG_UNUSED(byteCount);

	ptr = pgMapBufferCore(buffer, mode);
	return ((pgByte*)ptr) + offset;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	ID3D11DeviceContext_Unmap(PG_CONTEXT(buffer), (ID3D11Resource*)buffer->ptr, 0);
}

pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetConstantBuffers(PG_CONTEXT(buffer), slot, 1, &buffer->ptr);
	ID3D11DeviceContext_PSSetConstantBuffers(PG_CONTEXT(buffer), slot, 1, &buffer->ptr);
}

pgVoid pgUpdateConstantBufferCore(pgBuffer* buffer, pgVoid* data)
{
	pgVoid* bufferData = pgMapBufferCore(buffer, PG_MAP_WRITE_DISCARD);
	memcpy(bufferData, data, buffer->size);
	pgUnmapBuffer(buffer);
}

#endif