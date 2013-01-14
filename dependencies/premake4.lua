local sln = solution "lwar-dependencies"
  sln.location = "build"
  objdir "build"

  configurations { "Debug", "Release" }
  
  configuration { "Debug" }
	targetdir "lib/debug"
	defines { "DEBUG" }
	flags { "Symbols" }
	
  configuration { "Release" }
	targetdir "lib/release"
	flags { "Optimize" }
	
  local proj = project "glfw"
	proj.location = "build"
	kind "StaticLib"
	language "C"
	files { "src/glfw/*.c" }
	includedirs { "inc", "src/glfw" }
	
	configuration "vs2010"
		files { "src/glfw/win32/*.c" }
		includedirs { "src/glfw/win32" }
		
	configuration "gmake"
		files { "src/glfw/x11/*.c" }
		includedirs { "src/glfw/x11" }
