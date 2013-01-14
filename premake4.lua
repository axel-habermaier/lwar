local sln = solution "lwar"
  sln.location = "build"
  objdir "build"

  configurations { "Debug", "Release" }
  
  configuration { "Debug" }
	targetdir "bin/debug"
	defines { "DEBUG" }
	flags { "Symbols", "ExtraWarnings" }
	
  configuration { "Release" }
	targetdir "bin/release"
	flags { "Optimize", "Symbols", "ExtraWarnings" }
	
  local proj = project "client"
	proj.location = "build"
	kind "ConsoleApp"
	language "C"
	files { "src/client/**.h", "src/client/**.c", "src/shared/prelude.c" }
	targetname "lwar-client"
	includedirs { "src/shared", "src/client", "dependencies/inc" }
	links { "shared", "glfw" }
	vpaths { [""] = { "src/client", "src/shared" } }
	pchheader "prelude.h"
    pchsource "src/shared/prelude.c"
	
	configuration "Debug"
		libdirs { "dependencies/lib/debug" }
		
	configuration "Release"
		libdirs { "dependencies/lib/release" }
	
	configuration "gmake"
		links { "GL", "X11", "rt" }
		
	configuration "vs2010"
		links { "OpenGL32", "Ws2_32" }
		debugargs { "192.168.0.100 Axel" }
 
  local proj = project "server"
	proj.location = "build"
	kind "ConsoleApp"
	language "C"
	files { "src/server/**.h", "src/server/**.c", "src/shared/prelude.c" }
    excludes { "server/scratch.c" }
	targetname "lwar-server"
	includedirs { "src/shared", "src/server" }
	links { "shared" }
	vpaths { [""] = { "src/server", "src/shared" } }
	pchheader "prelude.h"
    pchsource "src/shared/prelude.c"
	
	configuration { "gmake" }
		links { "m" }
		
	configuration "vs2010"
		links { "Ws2_32" }
	
  local proj = project "shared"
	proj.location = "build"
	kind "StaticLib"
	language "C"
	targetname "lwar-shared"
	files { "src/shared/**.h", "src/shared/**.c" }
	includedirs { "src/shared" }
	pchheader "prelude.h"
    pchsource "src/shared/prelude.c"
	vpaths { [""] = { "src/shared" } }
	
	configuration "vs2010"
		excludes { "src/shared/sys.linux.c" }
		
	configuration "gmake"
		excludes { "src/shared/sys.win32.c" }