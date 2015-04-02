#I "packages/FAKE/tools"
#r "FakeLib.dll"
#r "System.Core"
#r "System.Xml"
#r "System.Xml.Linq"

open Fake
open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
open System.Xml
open System.Linq
open System.Xml.Linq

// MSBuild parameters for the projects
let buildParams configuration platform defaults = { 
    defaults with
        Verbosity = Some(Quiet)
        Targets = ["Build"]   
        Properties =
        [
            "Configuration", configuration
            "Platform", platform
            "DefineConstants", "Linux,x64"
        ]
}

// Builds the assets
let buildAssets configuration =
    // Builds an asset bundle lying at the given path
    let buildAssets bundle =
       if Shell.Exec ("mono", sprintf "--debug=mdb-optimizations ../../../Build/%s/pgc.exe compile-assets --bundle \"%s\" --platform Linux --assemblies \"../../../Build/%s/AnyCPU\" --target \"../../../Build/%s/AnyCPU\"" configuration (Path.GetFileName bundle) configuration configuration, Path.GetDirectoryName bundle) <> 0 then
          sprintf "Failed to compile asset bundle %s" bundle |> failwith

    // Builds a Xaml project lying at the given path, writing the generated code to the given target directory
    let buildXaml project target =
       if Shell.Exec ("mono", sprintf "--debug=mdb-optimizations ../../../Build/%s/pgc.exe compile-xaml --project \"%s\" --target \"../../../%s\"" configuration (Path.GetFileName project) target, Path.GetDirectoryName project) <> 0 then
          sprintf "Failed to compile Xaml project  %s" project |> failwith

    // Build the asset compiler and the asset projects
    let buildParams = buildParams configuration "AnyCPU"
    build buildParams "Source/Pegasus/Assets Compiler/Assets Compiler.csproj"
    build buildParams "Source/Pegasus/Assets/Pegasus Assets.csproj"
    build buildParams "Source/Lwar/Assets/Lwar Assets.csproj"

    // Copy the tools required by the asset compiler to the asset compiler output directory
    Copy (sprintf "Build/%s" configuration) ["Dependencies/Tools/nvcompress"; "Dependencies/Tools/nvassemble"]
  
    // Compile the asset bundles
    buildAssets "Source/Pegasus/Assets/MainBundle.xml"
    buildAssets "Source/Lwar/Assets/GameBundle.xml"
    buildAssets "Source/Lwar/Assets/MenuBundle.xml"

    // Compile the Xaml projects
    buildXaml "Source/Pegasus/User Interface/Pegasus UI.csproj" "Source/Pegasus/Pegasus/UserInterface/Views"
    buildXaml "Source/Lwar/User Interface/Lwar UI.csproj" "Source/Lwar/Lwar/UserInterface/Views"

// Builds the IL project
let buildIL configuration =
    let files = !! "Source/Pegasus/Pegasus/**/**.asm"
    if Shell.Exec ("ilasm", sprintf "/dll /output:Build/%s/AnyCPU/Pegasus.IL.dll %s" configuration (String.Join (" ", files)), "") <> 0 then
        failwith "Failed to compile IL project"
     
// Generates the code for all cvar and command registries        
let genRegistries configuration =
    let genRegistry file ns import directory =
        if Shell.Exec ("mono", sprintf "--debug=mdb-optimizations ../../../Build/%s/pgc.exe gen-registry --registry \"%s\" --namespace \"%s\" --import \"%s\"" configuration file ns import, directory) <> 0 then
            sprintf "Failed to generate code for registry %s" file |> failwith 

    printfn "Generating Pegasus cvar/command registries"
    genRegistry "Scripting/ICommands.cs" "Pegasus.Scripting" "" "Source/Pegasus/Pegasus"
    genRegistry "Scripting/ICvars.cs" "Pegasus.Scripting" "" "Source/Pegasus/Pegasus"

    printfn "Generating Lwar cvar/command registries"
    genRegistry "Scripting/ICommands.cs" "Lwar.Scripting" "../../Pegasus/Pegasus/Scripting/ICommands.cs" "Source/Lwar/Lwar"
    genRegistry "Scripting/ICvars.cs" "Lwar.Scripting" "../../Pegasus/Pegasus/Scripting/ICvars.cs" "Source/Lwar/Lwar"
  
// Generates the interop C++ and IL code for all interop files
let genInterop (project : string) configuration output =
    let interopFiles = 
        let ns = "http://schemas.microsoft.com/developer/msbuild/2003"
        let xn name = sprintf "{%s}%s" ns name |> XName.Get
        let document = XDocument.Load(project);
        document.Descendants(xn "Compile")
        |> Seq.filter (fun e -> e.Elements(xn "NativeInterop").Any ())
        |> Seq.map (fun e -> e.Attribute(XName.Get "Include").Value)

    printfn "Generating native interop code for C# project '%s'" project
    if Shell.Exec ("mono", sprintf "--debug=mdb-optimizations ../../../Build/%s/pgc.exe gen-interop --files \"%s\" --output \"%s\"" configuration (String.Join (";", interopFiles)) output, Path.GetDirectoryName project) <> 0 then
        sprintf "Failed to generate native interop code" |> failwith

// Compiles the native Server and Platform projects
let runMake target =
    if target = "Debug" then failwith "makefile does not support debug builds"
    if Shell.Exec ("make", if target = "Release" then "" else target) <> 0 then
        sprintf "make failed" |> failwith

// Uses the NUnit test runner to run all tests within the given assembly
let runTests assembly = 
    let runner = findToolFolderInSubPath "nunit-console.exe" "Source/packages"
    NUnit (fun p ->
        { p with
             ToolPath = runner
             DisableShadowCopy = true
             ShowLabels = false
             OutputFile = Path.Combine(Path.GetDirectoryName(assembly), Path.GetFileName(assembly) + ".xml") }
    ) [assembly]

// Compiles everything in the given configuration
let compile configuration =
    buildAssets configuration
    genRegistries configuration
    genInterop "Source/Pegasus/Pegasus/Pegasus.csproj" configuration "../Platform/Interop"
    buildIL configuration
    runMake configuration
    build (buildParams (sprintf "%s Linux" configuration) "x64") "Source/LwarMono.sln"
    runTests (sprintf "Build/%s/AnyCPU/Tests.dll" configuration)

// The Release target builds everything and copies the outputs to the Binaries directory
Target "Release" (fun _ -> 
    compile "Release"

    // Create or clean the output directory
    CleanDir "Binaries"

    // Copy the assemblies and all compiled asset bundles to the output directory
    let bundles = !! "Build/Release/AnyCPU/**/**.pak"
    Copy "Binaries" (seq { yield "Build/Release/AnyCPU/Lwar.exe"; yield! bundles })
    Copy "Binaries" ["Build/Release/AnyCPU/Lwar.exe"; "Build/Release/AnyCPU/Pegasus.dll"; "Build/Release/AnyCPU/Pegasus.IL.dll"]
    Copy "Binaries" ["Build/Release/AnyCPU/libPlatform.so"; "Build/Release/AnyCPU/libServer.so"]
)

// The Debug target builds everything
Target "Debug" (fun _ ->
    compile "Debug"
)

// Cleans all build output
Target "Clean" (fun _ ->
    CleanDir "Binaries"
    CleanDir "Build"
)

// Run the target that was specified via the command line
RunTargetOrDefault "Release"
