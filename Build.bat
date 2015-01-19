@echo off

echo =====================================================================
echo Compiling solution...
echo =====================================================================
cd Source
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv" Lwar.sln /Rebuild "Release Direct3D11|x86"
cd..

if exist \Binaries (
    rd Binaries /S /Q
)

mkdir Binaries
cd Binaries

echo =====================================================================
echo Merging assemblies...
echo =====================================================================
cd ..\Build\Release\AnyCPU\
..\..\..\Dependencies\Tools\ILRepack.exe /out:Lwar.exe /internalize Lwar.exe Pegasus.dll Pegasus.IL.dll
cd ..\..\..\Binaries

echo =====================================================================
echo Copying files...
echo =====================================================================
copy "..\Build\Release\AnyCPU\*.pak" 
copy "..\Build\Release\AnyCPU\Lwar.exe"
copy "..\Build\Release\AnyCPU\SDL2.dll" 

cd..