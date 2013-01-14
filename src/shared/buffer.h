#ifndef buffer_h__
#define buffer_h__

// ---------------------------------------------------------------------------------------------------------------------
// Buffer

// Can be used to read and write byte data - the fields in this structure should not be modified directly
// in order to ensure that no invariants are violated
typedef struct
{
	uint8_t* data;	
	size_t offset;	// The first byte that is read/written
	size_t len;		// The length of the buffer, that is, data is written in the range [offset, offset + len)
	size_t pos;	    // The current read/write position
} Buffer;

// Initializes new buffer
void buffer_init(Buffer* buffer, uint8_t* data, size_t offset, size_t len);

// Changes the current position by the given delta
void buffer_seek(Buffer* buffer, int32_t delta);

// Gets the current position in the buffer relative to the offset (that is, pos - offset)
size_t buffer_tell(Buffer* buffer);

// Checks whether the end of the buffer has been reached
bool buffer_end(Buffer* buffer);

// Indicates whether the given number of bytes can be read from or written to the buffer
bool buffer_fits(Buffer* buffer, size_t len);

// Writes the given value into the buffer (provided that the buffer does not overflow)
void buffer_write_bool(Buffer* buffer, bool value);
void buffer_write_uint8(Buffer* buffer, uint8_t value);
void buffer_write_int8(Buffer* buffer, int8_t value);
void buffer_write_uint16(Buffer* buffer, uint16_t value);
void buffer_write_int16(Buffer* buffer, int16_t value);
void buffer_write_uint32(Buffer* buffer, uint32_t value);
void buffer_write_int32(Buffer* buffer, int32_t value);
void buffer_write_uint64(Buffer* buffer, uint64_t value);
void buffer_write_int64(Buffer* buffer, int64_t value);
// Writes len bytes from value to the buffer
void buffer_write_array(Buffer* buffer, uint8_t* value, size_t len);

// Reads the given value from the buffer (provided that there are enough bytes left)
bool buffer_read_bool(Buffer* buffer);
uint8_t buffer_read_uint8(Buffer* buffer);
int8_t buffer_read_int8(Buffer* buffer);
uint16_t buffer_read_uint16(Buffer* buffer);
int16_t buffer_read_int16(Buffer* buffer);
uint32_t buffer_read_uint32(Buffer* buffer);
int32_t buffer_read_int32(Buffer* buffer);
uint64_t buffer_read_uint64(Buffer* buffer);
int64_t buffer_read_int64(Buffer* buffer);
// Reads len bytes from the buffer and writes it to the given value array
void buffer_read_array(Buffer* buffer, uint8_t* value, size_t len);

// ---------------------------------------------------------------------------------------------------------------------
// Endian conversion

// Switches the endianess of the given value
int64_t endian_int64(int64_t value);
uint64_t endian_uint64(uint64_t value);
int32_t endian_int32(int32_t value);
uint32_t endian_uint32(uint32_t value);
int16_t endian_int16(int16_t value);
uint16_t endian_uint16(uint16_t value);

#endif