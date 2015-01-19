#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake
open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions;

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
    let modifiedFiles = new List<string> ()
    for file in files do 
        // Unfortunately, Mono's ilasm is quite limited:
        // - we have to remove all preprocessor directives, simulation the preprocessor manually
        // - we have to remove the aggressiveinlining flag, as it is not supported by Mono's ilasm but
        //   shouldn't be required for OpenGL code anyway (all OpenGL entry points have less than 32 bytes
        //   of IL code, making them eligible for inlining by default); Direct3D11 might be slower, but
        //   can't be run on Linux anyway
        let content = File.ReadAllText file
        let content = content.Replace("aggressiveinlining", "")
        let content = content.Replace("#include \"Debug.asm\"", "")
        let content = content.Replace("#define DEBUG", "")
        let content = 
            if configuration = "Release" then
                // In release builds, remove the preprocessor directives and the enclosed content
                let regex = new Regex("""\#ifdef DEBUG(.|\r|\n)*?\#endif""", RegexOptions.Multiline)
                regex.Replace (content, "")
            else
                // In debug builds, just remove the preprocessor directives and leave the content as-is
                let content = content.Replace("#ifdef DEBUG", "")
                content.Replace("#endif", "")

        // Write the modified file content to a temporary file
        let fileName = sprintf "Build/%s/%s%d" configuration (Path.GetFileName file) modifiedFiles.Count
        modifiedFiles.Add fileName
        File.WriteAllText (fileName , content)  

    // Finally, compile the IL code
    if Shell.Exec ("ilasm", sprintf "/dll /output:Build/%s/AnyCPU/Pegasus.IL.dll %s" configuration (String.Join (" ", modifiedFiles)), "") <> 0 then
        failwith "Failed to compile IL project"

// The Release target builds the assets the Release and Debug targets
Target "Release" (fun _ -> 
    buildAssets "Release"
    buildIL "Release"

    // We can now compile Pegasus and Lwar
    build (buildParams "Release Linux" "x64") "Source/LwarMono.sln"

    // Create or clean the output directory
    CleanDir "Binaries"

    // Copy the assemblies and all compiled asset bundles to the output directory
    let bundles = !! "Build/Release/AnyCPU/**/**.pak"
    Copy "Binaries" (seq { yield "Build/Release/AnyCPU/Lwar.exe"; yield! bundles })
    Copy "Binaries" ["Build/Release/AnyCPU/Lwar.exe"; "Build/Release/AnyCPU/Pegasus.dll"; "Build/Release/AnyCPU/Pegasus.IL.dll"]
)

// The Debug target only builds the assets and the IL library
Target "Debug" (fun _ ->
    buildAssets "Debug"
    buildIL "Debug"
)

// Run the target that was specified via the command line
RunTargetOrDefault "Release"
