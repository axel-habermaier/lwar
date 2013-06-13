open System
open System.Globalization
open System.IO
open System.Text
open System.Threading
open System.Reflection
open Microsoft.FSharp.Reflection

//====================================================================================================================
// Type declarations
//====================================================================================================================
type Vector2 = { X : float; Y : float }

type EntityType = Bullet | Ship | Planet | Rocket | Gun | Phaser | Sun | Ray | Shockwave | Jupiter | Moon | Mars

type Template = {
    Type            : EntityType;
    Act             : string;
    Collide         : string;
    Interval        : int;
    Energy          : float;
    Health          : float;
    Shield          : float;
    Length          : float;
    Mass            : float;
    Radius          : float;
    Acceleration    : Vector2;
    Decelaration    : Vector2;
    Rotation        : float;   
    Texture         : string;
    CubeMap         : string;
    Model           : Template -> string;
}

//====================================================================================================================
// Template definitions
//====================================================================================================================
let templates = seq {
    yield {
        Type            = EntityType.Bullet;
        Act             = "decay";
        Collide         = "bullet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 2000.0;
        Shield          = 1.0;
        Length          = 0.0;
        Mass            = 0.1;
        Radius          = 16.0;
        Acceleration    = { X = 0.0; Y = 1200.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Gun;
        Act             = "gun_shoot";
        Collide         = null;
        Interval        = 300;
        Energy          = 1000.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 0.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Phaser;
        Act             = "phaser_shoot";
        Collide         = null;
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 0.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Planet;
        Act             = "gravity";
        Collide         = "planet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 10000.0;
        Radius          = 256.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = "Textures/Planet";
        Model           = fun t -> sprintf "Model.CreateSphere(graphicsDevice, %A, %A)" <| int t.Radius <| int t.Radius / 8;
    }
    yield {
        Type            = EntityType.Mars;
        Act             = "gravity";
        Collide         = "planet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 10000.0;
        Radius          = 128.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = "Textures/Mars";
        Model           = fun t -> sprintf "Model.CreateSphere(graphicsDevice, %A, %A)" <| int t.Radius <| int t.Radius / 8;
    }
    yield {
        Type            = EntityType.Moon;
        Act             = "gravity";
        Collide         = "planet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 10000.0;
        Radius          = 64.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = "Textures/Moon";
        Model           = fun t -> sprintf "Model.CreateSphere(graphicsDevice, %A, %A)" <| int t.Radius <| int t.Radius / 8;
    }
    yield {
        Type            = EntityType.Jupiter;
        Act             = "gravity";
        Collide         = "planet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 10000.0;
        Radius          = 512.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = "Textures/Jupiter";
        Model           = fun t -> sprintf "Model.CreateSphere(graphicsDevice, %A, %A)" <| int t.Radius <| int t.Radius / 8;
    }
    yield {
        Type            = EntityType.Sun;
        Act             = "gravity";
        Collide         = "planet_hit";
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 10000.0;
        Radius          = 1500.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;   
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Ray;
        Act             = "ray_act";
        Collide         = null;
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Shield          = 0.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 2048.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0; 
        Texture         = null;
        CubeMap         = null;  
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Rocket;
        Act             = "aim";
        Collide         = null;
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 1.0;
        Shield          = 1.0;
        Length          = 0.0;
        Mass            = 1.0;
        Radius          = 16.0;
        Acceleration    = { X = 500.0; Y = 20.0 };
        Decelaration    = { X = 20.0; Y = 20.0 };
        Rotation        = 1.0;   
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
    yield {
        Type            = EntityType.Ship;
        Act             = null;
        Collide         = "ship_hit";
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 200.0;
        Shield          = 1.0;
        Length          = 0.0;
        Mass            = 1.0;
        Radius          = 64.0;
        Acceleration    = { X = 1000.0; Y = 1000.0 };
        Decelaration    = { X = 1000.0; Y = 1000.0 };
        Rotation        = 2.0;   
        Texture         = null;
        CubeMap         = null;
        Model           = fun _ -> null;
    }
}

//====================================================================================================================
// CodeWriter helper class
//====================================================================================================================
type CodeWriter() as this =
    let output = new StringBuilder()
    let mutable atBeginningOfLine = true
    let mutable indent = 0
    do this.AppendHeader()

    member public this.Append (s : string) =
        this.AddIndentation()
        output.Append s |> ignore

    member public this.AppendLine s =
        this.Append s
        this.Newline()

    member public this.Newline() =
        output.AppendLine() |> ignore
        atBeginningOfLine <- true

    member public this.AppendBlockStatement terminateWithSemicolon content =
        this.EnsureNewline()
        this.AppendLine("{")
        this.IncreaseIndent()

        content()

        this.EnsureNewline()
        this.DecreaseIndent()
        this.Append("}")

        if terminateWithSemicolon then
            this.Append(";")

        this.Newline()

    member public this.WriteToFile path =
        File.WriteAllText(path, output.ToString())
        printfn "'%s' has been generated." path

    member private this.EnsureNewline() =
        if not atBeginningOfLine then
            this.Newline()

    member private this.AddIndentation() =
        if atBeginningOfLine then 
            atBeginningOfLine <- false
            for i = 1 to indent do
                output.Append("    ") |> ignore

    member public this.IncreaseIndent() = indent <- indent + 1
    member public this.DecreaseIndent() = indent <- indent - 1

    member private this.AppendHeader() =
        this.AppendLine("//------------------------------------------------------------------------------")
        this.AppendLine("// <auto-generated>")
        this.AppendLine(sprintf "//     Generated by %s." __SOURCE_FILE__)
        this.AppendLine("// </auto-generated>")
        this.AppendLine("//------------------------------------------------------------------------------")
        this.Newline()

//====================================================================================================================
// Code generation
//====================================================================================================================
let clientOutput = "Source/Client/Gameplay/Entities/Templates.cs"
let clientEnumOutput = "Source/Client/Gameplay/Entities/EntityType.cs"
let serverSourceOutput = "Source/Server/templates.c"
let serverHeaderOutput = "Source/Server/entity.h"

// Set the thread culture to the invariant culture so that we don't get localized ToString() output for floating
// point values
Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture <- CultureInfo.InvariantCulture;

// Generate the server code
let generateServerCode =
    let output = new CodeWriter()
    let templateName t = (sprintf "type_%A" t).ToLower()
    let functionName f = if String.IsNullOrWhiteSpace(f) then "NULL" else f
    let setField v = output.AppendLine(sprintf "%A, " v)
    let setStringField s = output.AppendLine(sprintf "%s, " s)
    let setVectorField v = output.AppendLine(sprintf "{ %A, %A }, " v.X v.Y)
    let entityTypeName t = (sprintf "ENTITY_TYPE_%s" t).ToUpper()
    let prototypes projection format =
        for f in templates |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(projection t))) do
            output.AppendLine(sprintf format (projection f))

    // Write required includes
    output.AppendLine("#include <math.h>")
    output.AppendLine("#include <stdint.h>")
    output.AppendLine("#include <stddef.h>")
    output.AppendLine("#include <stdlib.h>")
    output.Newline()
    output.AppendLine("#include \"server.h\"")
    output.AppendLine("#include \"rules.h\"")
    output.AppendLine("#include \"vector.h\"")
    output.Newline()

    // Write function prototypes
    prototypes (fun t -> t.Act) "void %s(Entity *self);"
    prototypes (fun t -> t.Collide) "void %s(Entity *self, Entity *other, Real impact);"
    output.Newline()

    // Write template definitions
    for t in templates do
        output.AppendLine(sprintf "EntityType %s =" (templateName t.Type))
        output.AppendBlockStatement true (fun () ->
            output.AppendLine("// entity type")
            setStringField(entityTypeName (sprintf "%A" t.Type))
            output.AppendLine("// action")
            setStringField(functionName t.Act)
            output.AppendLine("// collide")
            setStringField(functionName t.Collide)
            output.AppendLine("// interval")
            setField(t.Interval)
            output.AppendLine("// energy")
            setField(t.Energy)
            output.AppendLine("// health")
            setField(t.Health)
            output.AppendLine("// shield")
            setField(t.Shield)
            output.AppendLine("// length")
            setField(t.Length)
            output.AppendLine("// mass")
            setField(t.Mass)
            output.AppendLine("// radius")
            setField(t.Radius)
            output.AppendLine("// acceleration")
            setVectorField(t.Acceleration)
            output.AppendLine("// deceleration")
            setVectorField(t.Decelaration)
            output.AppendLine("// rotation")
            setField(t.Rotation)
        )
        output.Newline()

    // Write the source file
    output.WriteToFile serverSourceOutput

    // Generate the header file containing the entity type enumeration
    let output = new CodeWriter()
    let entityTypes = FSharpType.GetUnionCases typeof<EntityType>
    output.AppendLine("enum")
   
    output.AppendBlockStatement true (fun () ->
        for entityType in entityTypes do
            output.AppendLine(sprintf "%s = %i, " (entityTypeName entityType.Name) (entityType.Tag + 1))
        )

    // Write the header file
    output.WriteToFile serverHeaderOutput

// Generate the client code
let generateClientCode =
    let output = new CodeWriter()

    // Generate the file containing the entity type enumeration
    output.AppendLine("namespace Lwar.Client.Gameplay.Entities")
    output.AppendBlockStatement false (fun () ->
        let entityTypes = FSharpType.GetUnionCases typeof<EntityType>
        output.AppendLine("public enum EntityType")
   
        output.AppendBlockStatement false (fun () ->
            for entityType in entityTypes do
                output.AppendLine(sprintf "%s = %i, " entityType.Name (entityType.Tag + 1))
            )
        )

    // Write the enumeration file
    output.WriteToFile clientEnumOutput

    // Generate the entity partial classes corresponding to the template definitions
    let output = new CodeWriter()

    output.AppendLine("namespace Lwar.Client.Gameplay.Entities")
    output.AppendBlockStatement false (fun () ->
        output.AppendLine("using Pegasus.Framework;")
        output.AppendLine("using Pegasus.Framework.Platform;")
        output.AppendLine("using Pegasus.Framework.Platform.Graphics;")
        output.AppendLine("using Pegasus.Framework.Platform.Memory;")
        output.AppendLine("using Pegasus.Framework.Rendering;")
        output.Newline()

        output.AppendLine("public static class Templates")
        output.AppendBlockStatement false (fun () ->
            for t in templates do
                output.AppendLine(sprintf "public static Template %A { get; private set; }" t.Type)
            output.Newline()

            output.AppendLine("public static void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)")
            output.AppendBlockStatement false (fun () ->
                output.AppendLine("Assert.ArgumentNotNull(graphicsDevice);")
                output.AppendLine("Assert.ArgumentNotNull(assets);")
                output.Newline()

                for t in templates do
                    output.AppendLine(sprintf "%A = new Template" t.Type)
                    output.AppendLine("(")
                    output.IncreaseIndent()
                    output.AppendLine(sprintf "maxEnergy: %Af," t.Energy)
                    output.AppendLine(sprintf "maxHealth: %Af," t.Health)
                    output.AppendLine(sprintf "radius: %Af," t.Radius)
                    if t.Texture <> null then
                        output.AppendLine(sprintf "texture: assets.LoadTexture2D(%A)," t.Texture)
                    else
                        output.AppendLine("texture: null,")
                    if t.CubeMap <> null then
                        output.AppendLine(sprintf "cubeMap: assets.LoadCubeMap(%A)," t.CubeMap)
                    else
                        output.AppendLine("cubeMap: null,")
                    let model = t.Model t
                    if model <> null then
                        output.AppendLine(sprintf "model: %s" model)
                    else
                        output.AppendLine("model: null")
                    output.DecreaseIndent()
                    output.AppendLine(");")
                    output.Newline()
            )

            output.Newline()
            output.AppendLine("public static void Dispose()")
            output.AppendBlockStatement false (fun () ->
                for t in templates do
                    output.AppendLine(sprintf "%A.SafeDispose();" t.Type)
            )
        )
    )

    // Write the partial classes file
    output.WriteToFile clientOutput

generateServerCode
generateClientCode
