mozroots --import --sync
mono Dependencies/Tools/NuGet.exe install FAKE -OutputDirectory "Source/packages" -ExcludeVersion
mono Dependencies/Tools/NuGet.exe install Nunit.Runners -OutputDirectory "Source/packages" -version 2.6.3
mono Dependencies/Tools/NuGet.exe restore "Source/Pegasus/Assets Compiler/packages.config" -OutputDirectory "Source/packages"
mono Dependencies/Tools/NuGet.exe restore "Source/Pegasus/Tests/packages.config" -OutputDirectory "Source/packages"
mono Source/packages/FAKE/tools/FAKE.exe Source/Build.fsx $1
