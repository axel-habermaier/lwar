.PHONY: all clean

VPATH = Source/Lwar/Server Source/Lwar/Dedicated
BUILD = Build/Debug/Server

SERVER_H      = server.h
SERVER_SRC    = array.c client.c connection.c debug.c entity.c format.c log.c \
                message.c physics.c packet.c player.c pq.c protocol.c queue.c \
                rules.c server.c pool.c time.c templates.c uint.c update.c
SERVER_OBJ    = $(addprefix $(BUILD)/,$(SERVER_SRC:.c=.o))
SERVER_SO     = $(BUILD)/libserver.so
SERVER_LIB    = 

DEDICATED_H   =
DEDICATED_SRC = dedicated.c visualization.c window.c window_x11.c
DEDICATED_OBJ = $(addprefix $(BUILD)/,$(DEDICATED_SRC:.c=.o))
DEDICATED_LIB = -lm -lGL -lX11 -lrt -lserver -L $(BUILD)
DEDICATED_BIN = $(BUILD)/dedicated

CC = clang
LD = clang
CFLAGS = -Wall -g -fPIC -ISource/Lwar/Server

all: $(BUILD) $(SERVER_SO) $(DEDICATED_BIN)

run: $(DEDICATED_BIN)
	LD_LIBRARY_PATH=$(BUILD) ./$(DEDICATED_BIN)

gdb: $(DEDICATED_BIN)
	LD_LIBRARY_PATH=$(BUILD) gdb ./$(DEDICATED_BIN)

clean:
	rm $(SERVER_OBJ) $(DEDICATED_OBJ)

$(BUILD):
	mkdir -p $@

$(BUILD)/%.o: %.c $(SERVER_H)
	$(CC) -c $< -o $@ $(CFLAGS)

$(SERVER_SO): $(SERVER_OBJ)
	$(LD) -shared $^ -o $@ $(SERVER_LIB)

$(DEDICATED_BIN): $(DEDICATED_OBJ) $(SERVER_SO)
	$(LD) $(DEDICATED_OBJ) -o $@ $(DEDICATED_LIB)
