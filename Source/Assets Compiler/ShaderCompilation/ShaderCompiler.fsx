open System
open System.Linq
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

type Effect = {
    Name : string;
    Shaders : Shader list;
}

//====================================================================================================================
// Reflection
//====================================================================================================================
let reflectEffects (assembly : Assembly) =
    // Checks whether a type is an effect
    let isEffect (t : Type) =
        t.GetCustomAttributes() 
            |> Seq.exists (fun a -> a.ToString() = "Pegasus.AssetsCompiler.ShaderCompilation.EffectAttribute")

    // Gets the type infos for all effects
    let effectTypes =
        assembly.GetTypes() 
            |> Seq.filter isEffect
            |> Seq.map (fun t -> t.GetTypeInfo())

    // Creates a list of shader instances from the type
    let shaders (t : TypeInfo) =
        // Creates the shader instances
        let createShaders shaderType =
            // Checks whether a method is a shader
            let isShader (m : MethodInfo) =
                let attributeType = sprintf "Pegasus.AssetsCompiler.ShaderCompilation.%AAttribute" shaderType
                m.GetCustomAttributes() 
                    |> Seq.exists (fun a -> a.ToString() = attributeType)

            // Gets the semantics of the parameter
            let getSemantics (p : ParameterInfo) =
                let attribute = p.GetCustomAttributes() |> Seq.head
                match attribute.GetType().FullName with
                | "Pegasus.AssetsCompiler.ShaderCompilation.Semantics.PositionAttribute" -> Position
                | "Pegasus.AssetsCompiler.ShaderCompilation.Semantics.NormalAttribute" -> Normal
                | "Pegasus.AssetsCompiler.ShaderCompilation.Semantics.TexCoordsAttribute" -> TexCoords(0)
                | "Pegasus.AssetsCompiler.ShaderCompilation.Semantics.ColorAttribute" -> Color(0)
                | _ -> failwith "Unknown parameter semantics"

            // Gets the parameters of the shader
            let parameters f (m : MethodInfo) =
                m.GetParameters() 
                    |> Seq.filter f
                    |> Seq.map (fun p -> { Name = p.Name; Semantics = getSemantics p })
                    |> Seq.toList

            let inputs = parameters (fun p -> not p.IsOut)
            let outputs = parameters (fun p -> p.IsOut)

            t.DeclaredMethods
                |> Seq.filter isShader
                |> Seq.map (fun m -> { Type = shaderType; Name = m.Name; Inputs = inputs m; Outputs = outputs m })

        let vertexShaders = createShaders VertexShader
        let fragmentShaders = createShaders FragmentShader

        vertexShaders 
            |> Seq.append fragmentShaders
            |> Seq.toList

    // Creates an effect instance from a type
    let effect (t : TypeInfo) =
        { Name = t.Name; Shaders = shaders t }

    for e in effectTypes |> Seq.map effect do
        printfn "%A" e

//====================================================================================================================
// Execute compiler
//====================================================================================================================
if fsi.CommandLineArgs.Length < 3 then
    printfn "Invalid shader compiler invocation: [assembly name] [C# file 1] [C# file 2] ..."

let assetAssembly = fsi.CommandLineArgs.[1]
let csharpFiles = fsi.CommandLineArgs.[2..]

let assembly = Assembly.LoadFile(assetAssembly)
reflectEffects(assembly)