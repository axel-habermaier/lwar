 #include "prelude.h"

#ifdef DIRECT3D11

//====================================================================================================================
// Core functions
//====================================================================================================================

pgVoid pgCreateBufferCore(pgBuffer* buffer, pgBufferType type, pgResourceUsage usage, pgVoid* data, pgInt32 size)
{
	D3D11_BUFFER_DESC desc;
	D3D11_SUBRESOURCE_DATA dataResource;
	dataResource.pSysMem = data;
	dataResource.SysMemPitch = 0;
	dataResource.SysMemSlicePitch = 0;

	desc.BindFlags = pgConvertBufferType(type);
	desc.CPUAccessFlags = usage == PG_USAGE_DYNAMIC ? D3D11_CPU_ACCESS_WRITE : 0;
	desc.ByteWidth = size;
	desc.MiscFlags = 0;
	desc.StructureByteStride = 0;
	desc.Usage = pgConvertResourceUsage(usage);
	
	D3DCALL(ID3D11Device_CreateBuffer(DEVICE(buffer), &desc, data == NULL ? NULL : &dataResource, &buffer->ptr), 
		"Failed to create buffer.");
}

pgVoid pgDestroyBufferCore(pgBuffer* buffer)
{
	ID3D11Buffer_Release(buffer->ptr);
}

pgVoid* pgMapBufferCore(pgBuffer* buffer, pgMapMode mode)
{
	D3D11_MAPPED_SUBRESOURCE subResource;
	D3DCALL(ID3D11DeviceContext_Map(CONTEXT(buffer), (ID3D11Resource*)buffer->ptr, 0, pgConvertMapMode(mode), 0, &subResource), 
		"Failed to map buffer.");
	return subResource.pData;
}

pgVoid pgUnmapBufferCore(pgBuffer* buffer)
{
	ID3D11DeviceContext_Unmap(CONTEXT(buffer), (ID3D11Resource*)buffer->ptr, 0);
}

pgVoid pgBindConstantBufferCore(pgBuffer* buffer, pgInt32 slot)
{
	ID3D11DeviceContext_VSSetConstantBuffers(CONTEXT(buffer), slot, 1, &buffer->ptr);
	ID3D11DeviceContext_PSSetConstantBuffers(CONTEXT(buffer), slot, 1, &buffer->ptr);
}

#endif