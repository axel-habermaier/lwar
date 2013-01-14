#include "prelude.h"
#include "buffer.h"

// ---------------------------------------------------------------------------------------------------------------------
// Buffer

void buffer_init(Buffer* buffer, uint8_t* data, size_t offset, size_t len)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(data);

	buffer->data = data;
	buffer->offset = offset;
	buffer->len = len;
	buffer->pos = offset;
}

void buffer_seek(Buffer* buffer, int32_t delta)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_IN_RANGE(buffer->pos + delta, buffer->offset, buffer->offset + buffer->len);

	buffer->pos += delta;
}

size_t buffer_tell(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	return buffer->pos - buffer->offset;
}

bool buffer_end(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	return buffer->pos >= buffer->offset + buffer->len;
}

bool buffer_fits(Buffer* buffer, size_t len)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	return buffer->pos + len <= buffer->offset + buffer->len;
}

void buffer_write_bool(Buffer* buffer, bool value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)(value == true ? 1 : 0));
}

void buffer_write_uint8(Buffer* buffer, uint8_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t)), "Out of bounds.");

	buffer->data[buffer->pos++] = value;
}

void buffer_write_int8(Buffer* buffer, int8_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int8_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
}

void buffer_write_uint16(Buffer* buffer, uint16_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint16_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
}

void buffer_write_int16(Buffer* buffer, int16_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int16_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
}

void buffer_write_uint32(Buffer* buffer, uint32_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint32_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
	buffer_write_uint8(buffer, (uint8_t)(value >> 16));
	buffer_write_uint8(buffer, (uint8_t)(value >> 24));
}

void buffer_write_int32(Buffer* buffer, int32_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int32_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
	buffer_write_uint8(buffer, (uint8_t)(value >> 16));
	buffer_write_uint8(buffer, (uint8_t)(value >> 24));
}

void buffer_write_uint64(Buffer* buffer, uint64_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint64_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
	buffer_write_uint8(buffer, (uint8_t)(value >> 16));
	buffer_write_uint8(buffer, (uint8_t)(value >> 24));
	buffer_write_uint8(buffer, (uint8_t)(value >> 32));
	buffer_write_uint8(buffer, (uint8_t)(value >> 40));
	buffer_write_uint8(buffer, (uint8_t)(value >> 48));
	buffer_write_uint8(buffer, (uint8_t)(value >> 56));
}

void buffer_write_int64(Buffer* buffer, int64_t value)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int64_t)), "Out of bounds.");

	buffer_write_uint8(buffer, (uint8_t)value);
	buffer_write_uint8(buffer, (uint8_t)(value >> 8));
	buffer_write_uint8(buffer, (uint8_t)(value >> 16));
	buffer_write_uint8(buffer, (uint8_t)(value >> 24));
	buffer_write_uint8(buffer, (uint8_t)(value >> 32));
	buffer_write_uint8(buffer, (uint8_t)(value >> 40));
	buffer_write_uint8(buffer, (uint8_t)(value >> 48));
	buffer_write_uint8(buffer, (uint8_t)(value >> 56));
}

void buffer_write_array(Buffer* buffer, uint8_t* value, size_t len)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(value);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t) * len), "Out of bounds.");

	mem_copy(buffer->data + buffer->pos, value, len);
	buffer->pos += len;
}

bool buffer_read_bool(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t)), "Out of bounds.");

	return buffer_read_uint8(buffer) == 1 ? true : false;
}

uint8_t buffer_read_uint8(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t)), "Out of bounds.");

	return buffer->data[buffer->pos++];
}

int8_t buffer_read_int8(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int8_t)), "Out of bounds.");

	return (int8_t)buffer_read_uint8(buffer);
}

uint16_t buffer_read_uint16(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint16_t)), "Out of bounds.");

	return (uint16_t)(buffer_read_uint8(buffer) | buffer_read_uint8(buffer) << 8);
}

int16_t buffer_read_int16(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int16_t)), "Out of bounds.");

	return (int16_t)(buffer_read_uint8(buffer) | buffer_read_uint8(buffer) << 8);
}

uint32_t buffer_read_uint32(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint32_t)), "Out of bounds.");

	return (uint32_t)(buffer_read_uint8(buffer) | buffer_read_uint8(buffer) << 8 |
		buffer_read_uint8(buffer) << 16 | buffer_read_uint8(buffer) << 24);
}

int32_t buffer_read_int32(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int32_t)), "Out of bounds.");

	return (int32_t)(buffer_read_uint8(buffer) | buffer_read_uint8(buffer) << 8 |
		buffer_read_uint8(buffer) << 16 | buffer_read_uint8(buffer) << 24);
}

uint64_t buffer_read_uint64(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint64_t)), "Out of bounds.");

	return (uint64_t)(
		((uint64_t)buffer_read_uint8(buffer)) | 
		((uint64_t)buffer_read_uint8(buffer)) << 8 |
		((uint64_t)buffer_read_uint8(buffer)) << 16 | 
		((uint64_t)buffer_read_uint8(buffer)) << 24 |
		((uint64_t)buffer_read_uint8(buffer)) << 32 | 
		((uint64_t)buffer_read_uint8(buffer)) << 40 |
		((uint64_t)buffer_read_uint8(buffer)) << 48 | 
		((uint64_t)buffer_read_uint8(buffer)) << 56);
}

int64_t buffer_read_int64(Buffer* buffer)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(int64_t)), "Out of bounds.");

	return (int64_t)(
		((int64_t)buffer_read_int8(buffer)) | 
		((int64_t)buffer_read_int8(buffer)) << 8 |
		((int64_t)buffer_read_int8(buffer)) << 16 | 
		((int64_t)buffer_read_int8(buffer)) << 24 |
		((int64_t)buffer_read_int8(buffer)) << 32 | 
		((int64_t)buffer_read_int8(buffer)) << 40 |
		((int64_t)buffer_read_int8(buffer)) << 48 | 
		((int64_t)buffer_read_uint8(buffer)) << 56);
}

void buffer_read_array(Buffer* buffer, uint8_t* value, size_t len)
{
	LWAR_ASSERT_NOT_NULL(buffer);
	LWAR_ASSERT_NOT_NULL(value);
	LWAR_ASSERT(buffer_fits(buffer, sizeof(uint8_t) * len), "Out of bounds.");

	mem_copy(value, buffer->data + buffer->pos, len);
}

// ---------------------------------------------------------------------------------------------------------------------
// Endian conversion

int64_t endian_int64(int64_t value)
{
	return ((int64_t)(uint8_t)(value)) << 56 |
			((int64_t)(uint8_t)(value >> 8)) << 48 |
			((int64_t)(uint8_t)(value >> 16)) << 40 |
			((int64_t)(uint8_t)(value >> 24)) << 32 |
			((int64_t)(uint8_t)(value >> 32)) << 24 |
			((int64_t)(uint8_t)(value >> 40)) << 16 |
			((int64_t)(uint8_t)(value >> 48)) << 8 |
			(uint8_t)(value >> 56);
}

uint64_t endian_uint64(uint64_t value)
{
	return ((uint64_t)((uint8_t)(value)) << 56 |
			((uint64_t)(uint8_t)(value >> 8)) << 48 |
			((uint64_t)(uint8_t)(value >> 16)) << 40 |
			((uint64_t)(uint8_t)(value >> 24)) << 32 |
			((uint64_t)(uint8_t)(value >> 32)) << 24 |
			((uint64_t)(uint8_t)(value >> 40)) << 16 |
			((uint64_t)(uint8_t)(value >> 48)) << 8 |
			(uint8_t)(value >> 56));
}

int32_t endian_int32(int32_t value)
{
	return ((uint8_t)(value)) << 24 |
			((uint8_t)(value >> 8)) << 16 |
			((uint8_t)(value >> 16)) << 8 |
			((uint8_t)(value >> 24));
}

uint32_t endian_uint32(uint32_t value)
{
	return (uint32_t)(((uint8_t)(value)) << 24 |
					 ((uint8_t)(value >> 8)) << 16 |
					 ((uint8_t)(value >> 16)) << 8 |
					 ((uint8_t)(value >> 24)));
}

int16_t endian_int16(int16_t value)
{
	return (int16_t)(((uint8_t)(value)) << 8 | ((uint8_t)(value >> 8)));
}

uint16_t endian_uint16(uint16_t value)
{
	return (uint16_t)(((uint8_t)(value)) << 8 | ((uint8_t)(value >> 8)));
}