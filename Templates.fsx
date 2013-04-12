open System
open System.Globalization
open System.IO
open System.Text
open System.Threading
open System.Reflection

//====================================================================================================================
// Type declarations
//====================================================================================================================
type Vector2 = { X : float; Y : float }

type EntityType = | Bullet | Ship | Planet | Rocket | Ray | Shockwave | Gun | Phaser | Sun

type Template = {
    Name            : string;
    Type            : EntityType;
    Act             : string;
    Collide         : string;
    Interval        : int;
    Energy          : float;
    Health          : float;
    Length          : float;
    Mass            : float;
    Radius          : float;
    Acceleration    : Vector2;
    Decelaration    : Vector2;
    Rotation        : float;   
}

//====================================================================================================================
// Template definitions
//====================================================================================================================
let templates = seq {
    yield {
        Name            = "A";
        Type            = EntityType.Ship;
        Act             = "hi";
        Collide         = "gnah";
        Interval        = 3;
        Energy          = 1.04;
        Health          = 3.14;
        Length          = 1.0;
        Mass            = 10.0;
        Radius          = 15.0;
        Acceleration    = { X = 1.0; Y = 2.0 };
        Decelaration    = { X = 2.0; Y = 3.43 };
        Rotation        = 17.0;
    }
    yield {
        Name            = "Bullet";
        Type            = EntityType.Bullet;
        Act             = "decay";
        Collide         = null;
        Interval        = 0;
        Energy          = 0.0;
        Health          = 100.0;
        Length          = 0.0;
        Mass            = 0.1;
        Radius          = 8.0;
        Acceleration    = { X = 0.0; Y = 500.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
    }
}

//====================================================================================================================
// Code generation
//====================================================================================================================
let clientOutput = "Source/Client/Gameplay/Templates.cs"
let serverOutput = "Source/Server/templates.c"

Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture <- CultureInfo.InvariantCulture;

let generateServer =
    let output = new StringBuilder()
    let templateName t = (sprintf "type_%s" t).ToLower()
    let functionName f = if String.IsNullOrWhiteSpace(f) then "NULL" else f
    let setField (v : obj) = output.AppendFormat("   {0}, ", v).AppendLine() |> ignore
    let setVectorField v = output.AppendFormat("   {{ {0}, {1} }}, ", v.X, v.Y).AppendLine() |> ignore
    let entityType t = (sprintf "ENTITY_TYPE_%A" t).ToUpper()
    let prototypes projection format =
        for f in templates |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(projection t))) do
            output.AppendFormat(format, projection f).AppendLine() |> ignore

    // Output required includes
    output.AppendLine("#include <math.h>")
        .AppendLine("#include <stdint.h>")
        .AppendLine("#include <stddef.h>")
        .AppendLine("#include <stdlib.h>")
        .AppendLine()
        .AppendLine("#include \"server.h\"")
        .AppendLine("#include \"rules.h\"")
        .AppendLine("#include \"vector.h\"")
        .AppendLine()
        |> ignore

    // Output function prototypes
    prototypes (fun t -> t.Act) "void {0}(Entity *self);"
    prototypes (fun t -> t.Collide) "void {0}(Entity *self, Entity *other);"
    output.AppendLine() |> ignore

    // Output template definitions
    for t in templates do
        output.AppendFormat("EntityType {0} =", templateName t.Name).AppendLine().AppendLine("{") |> ignore
        setField(entityType t.Type) 
        setField(functionName t.Act)
        setField(functionName t.Collide)
        setField(t.Interval)
        setField(t.Energy)
        setField(t.Health)
        setField(t.Length)
        setField(t.Mass)
        setField(t.Radius)
        setVectorField(t.Acceleration)
        setVectorField(t.Decelaration)
        setField(t.Rotation)
        output.AppendLine("};").AppendLine() |> ignore

    File.WriteAllText(serverOutput, output.ToString())

generateServer