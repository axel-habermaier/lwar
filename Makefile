.PHONY: all clean

VPATH = Source/Server Source/Dedicated
BUILD = build/Debug/server

SERVER_H      = server.h
SERVER_SRC    = client.c connection.c entity.c log.c message.c physics.c pq.c protocol.c rules.c server.c time.c uint.c
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
CFLAGS = -Wall -g -fPIC -ISource/Server

all: $(BUILD) $(SERVER_SO) $(DEDICATED_BIN)

run: $(DEDICATED_BIN)
	LD_LIBRARY_PATH=$(BUILD) ./$(DEDICATED_BIN) -visual

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
