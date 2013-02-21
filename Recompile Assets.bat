cd Assets
rmdir /Q /S obj

cd ..

cd Binaries/Debug
rmdir /Q /S Assets
"Assets Compiler.exe"

cd ..
cd Release
rmdir /Q /S Assets
"Assets Compiler.exe"

cd ..
cd ..