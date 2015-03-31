.PHONY: all clean

VPATH = \
Source/Lwar/Server \
Source/Lwar/Dedicated  \
Source/Pegasus/Platform \
Source/Pegasus/Platform/Graphics \
Source/Pegasus/Platform/Graphics/OpenGL3 \
Source/Pegasus/Platform/Network \
Source/Pegasus/Platform/Platform \
Source/Pegasus/Platform/Utilities \

BUILD = Build/Debug/Server
DIST  = Binaries

SERVER_SRC    = \
address.c       \
array.c         \
client.c        \
clock.c         \
connection.c    \
debug.c         \
entity.c        \
id.c            \
log.c           \
message.c       \
performance.c   \
pool.c          \
pq.c            \
pack.c          \
real.c 			\
str.c           \
uint.c          \
update.c        \
packet.c        \
player.c        \
physics.c       \
queue.c         \
protocol.c      \
server.c        \
stream.c        \
rules.c         \
templates.c     \
unpack.c        \

DEDICATED_SRC = dedicated.c visualization.c window.c window_x11.c

PEGASUS_SRC   =       	\
OpenGL3.cpp           	\
BufferGL3.cpp         	\
ShaderGL3.cpp         	\
BindingsGL3.cpp       	\
PipelineStateGL3.cpp  	\
QueryGL3.cpp          	\
GraphicsDeviceGL3.cpp 	\
TextureGL3.cpp        	\
VertexLayoutGL3.cpp   	\
SwapChainGL3.cpp 		\
RenderTargetGL3.cpp 	\
Graphics.cpp 			\
Mouse.cpp 				\
Keyboard.cpp 			\
Platform.cpp 			\
NativeWindow.cpp 		\
Window.cpp 				\
Win32.cpp  				\
Memory.cpp 				\
NetworkException.cpp 	\
Network.cpp    			\
IPEndPoint.cpp 			\
UdpSocket.cpp  			\
IPAddress.cpp  			\
Prelude.cpp    			\


SERVER_OBJ    = $(addprefix $(BUILD)/,$(SERVER_SRC:.c=.o))
SERVER_SO     = $(DIST)/libServer.so
SERVER_LIB    = 

DEDICATED_OBJ = $(addprefix $(BUILD)/,$(DEDICATED_SRC:.c=.o))
DEDICATED_LIB = -lm -lGL -lX11 -lrt -lserver -L $(DIST)
DEDICATED_BIN = $(DIST)/dedicated

PEGASUS_OBJ   = $(addprefix $(BUILD)/,$(PEGASUS_SRC:.cpp=.o))
PEGASUS_SO    = $(DIST)/libPlatform.so
PEGASUS_LIB   = -lSDL2


CC = clang
CXX = clang++ -std=c++11
LD = clang
CFLAGS = -Wall -g -fPIC -ISource/Lwar/Server -DDEBUG
CXXFLAGS = -Wall -g -fPIC -ISource/Pegasus/Platform -DDEBUG

all: $(BUILD) $(SERVER_SO) $(DEDICATED_BIN) $(PEGASUS_SO)

run:
	#(cd $(DIST); mono Lwar.exe)
	echo "don't use it does not work!"

rund: $(DEDICATED_BIN)
	LD_LIBRARY_PATH=$(DIST) ./$(DEDICATED_BIN)

gdb: $(DEDICATED_BIN)
	LD_LIBRARY_PATH=$(DIST) gdb ./$(DEDICATED_BIN)

clean:
	rm $(SERVER_OBJ) $(DEDICATED_OBJ)

$(BUILD):
	mkdir -p $@

$(BUILD)/%.o: %.c
	$(CC) -c $< -o $@ $(CFLAGS)

$(BUILD)/%.o: %.cpp
	$(CXX) -c $< -o $@ $(CXXFLAGS)

$(SERVER_SO): $(SERVER_OBJ)
	$(LD) -shared $^ -o $@ $(SERVER_LIB)

$(PEGASUS_SO): $(PEGASUS_OBJ)
	$(LD) -shared $^ -o $@ $(PEGASUS_LIB)

$(DEDICATED_BIN): $(DEDICATED_OBJ) $(SERVER_SO)
	$(LD) $(DEDICATED_OBJ) -o $@ $(DEDICATED_LIB)
