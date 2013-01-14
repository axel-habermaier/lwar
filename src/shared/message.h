#include "buffer.h"

typedef struct Message Message;

// Writes the message into the buffer; returns the number of bytes that have been written or 0 in the case
// that the message does not fit into the buffer
size_t message_pack(Buffer* buffer, Message* m);

// Reads a message from the buffer; returns the number of bytes that have been read or 0 in the case
// that no full message was extracted from the buffer, in which case the contents of m are undefined
size_t message_unpack(Buffer* buffer, Message* m);

void message_print(Message *m);

enum 
{
    MESSAGE_CONNECT,
    MESSAGE_JOIN,
    MESSAGE_LEAVE,
    MESSAGE_CHAT,
    MESSAGE_INPUT,
    MESSAGE_ADD,
    MESSAGE_REMOVE,
    MESSAGE_UPDATE,
};

struct Message {
    uint8_t type;

    union 
	{
        uint32_t seqno;
        uint32_t time; /* used only for update messages */
    };

    union 
	{
        struct 
		{
            Id player;
            PlayerName name;
        } join;

        struct 
		{
            Id player;
        } leave;

        struct 
		{
            Id player;
            ChatMsg text;
        } chat;

        struct 
		{
            Id player;
            bool up, down, left, right, shooting;
            uint32_t ack;
        } input;

        struct 
		{
            Id entity;
            Id player;
            uint8_t type;
        } add;

        struct 
		{
            Id entity;
        } remove;

        struct 
		{
            Id entity;
            int32_t  x, y;   /* scaled by SCALE */
            int32_t  vx,vy;
            uint16_t rot;    /* in radians, scaled by SCALE */
            uint8_t  health; /* in percent */
        } update;
    };
};
