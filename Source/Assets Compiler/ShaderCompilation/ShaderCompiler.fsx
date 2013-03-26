open System
open System.Reflection

//====================================================================================================================
// Shader metamodel
//====================================================================================================================
type Semantics =
    | Position
    | TexCoords of int
    | Normal
    | Color of int

type Parameter = {
    Name : string;
    Semantics : Semantics;
}

type ShaderType = 
    | VertexShader
    | FragmentShader

type Shader = {
    Type : ShaderType;
    Name : string;
    Inputs : Parameter list;
    Outputs : Parameter list;
}

//====================================================================================================================
// Reflection
//====================================================================================================================
let reflectShaders (assembly : Assembly) =
    let isEffect (t : Type) =
        t.GetCustomAttributes() 
            |> Seq.map (fun a -> a.ToString())
            |> Seq.exists (fun n -> n = "Pegasus.AssetsCompiler.ShaderCompilation.EffectAttribute")

    let effects =
        assembly.GetTypes() |> 
            Seq.filter (fun t -> isEffect t)

    for e in effects do
        printfn "%s" e.FullName

//====================================================================================================================
// Execute compiler
//====================================================================================================================
if fsi.CommandLineArgs.Length < 3 then
    printfn "Invalid shader compiler invocation: [assembly name] [C# file 1] [C# file 2] ..."

let assetAssembly = fsi.CommandLineArgs.[1]
let csharpFiles = fsi.CommandLineArgs.[2..]

let assembly = Assembly.LoadFile(assetAssembly)
reflectShaders(assembly)