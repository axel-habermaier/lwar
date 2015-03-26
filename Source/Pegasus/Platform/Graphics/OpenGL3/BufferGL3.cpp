#include "Prelude.hpp"

using namespace OpenGL3;

Buffer* GraphicsDevice::InitializeBuffer(BufferDescription* desc)
{
	auto buffer = PG_NEW(Buffer);
	buffer->Obj = Allocate(&GraphicsDevice::glGenBuffers, "Buffer");
	buffer->Size = desc->SizeInBytes;

	switch (desc->Type)
	{
	case BufferType::ConstantBuffer:
		buffer->Type = GL_UNIFORM_BUFFER;
		break;
	case BufferType::IndexBuffer:
		buffer->Type = GL_ELEMENT_ARRAY_BUFFER;
		break;
	case BufferType::VertexBuffer:
		buffer->Type = GL_ARRAY_BUFFER;
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	glBindBuffer(buffer->Type, buffer->Obj);
	glBufferData(buffer->Type, ToPointer(desc->SizeInBytes), desc->Data, Map(desc->Usage));

	return buffer;
}

void GraphicsDevice::FreeBuffer(Buffer* buffer)
{
	if (buffer == nullptr)
		return;

	glDeleteBuffers(1, &buffer->Obj);
	PG_DELETE(buffer);
}

void* GraphicsDevice::MapBuffer(Buffer* buffer, MapMode mode)
{
	glBindBuffer(buffer->Type, buffer->Obj);
	auto mappedBuffer = glMapBuffer(buffer->Type, Map(mode));

	if (mappedBuffer == nullptr)
		PG_DIE("Failed to map buffer.");

	return mappedBuffer;
}

void* GraphicsDevice::MapBufferRange(Buffer* buffer, MapMode mode, int32 offsetInBytes, int32 byteCount)
{
	glBindBuffer(buffer->Type, buffer->Obj);
	auto mappedBuffer = glMapBufferRange(buffer->Type, ToPointer(offsetInBytes), ToPointer(byteCount), Map(mode));

	if (mappedBuffer == nullptr)
		PG_DIE("Failed to map buffer.");

	return mappedBuffer;
}

void GraphicsDevice::UnmapBuffer(Buffer* buffer)
{
	glBindBuffer(buffer->Type, buffer->Obj);
	auto success = glUnmapBuffer(buffer->Type);

	if (!success)
		PG_DIE("Failed to unmap buffer.");
}

void GraphicsDevice::BindBuffer(Buffer* buffer, int32 slot)
{
	glBindBufferBase(GL_UNIFORM_BUFFER, safe_static_cast<uint32>(slot), buffer->Obj);
}

void GraphicsDevice::CopyBuffer(Buffer* buffer, void* data)
{
	glBindBuffer(GL_UNIFORM_BUFFER, buffer->Obj);
	glBufferSubData(GL_UNIFORM_BUFFER, 0, ToPointer(buffer->Size), data);
}

void GraphicsDevice::SetBufferName(Buffer* buffer, const char* name)
{
	PG_UNUSED(buffer);
	PG_UNUSED(name);
}