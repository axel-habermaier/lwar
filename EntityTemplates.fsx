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

type EntityType = Bullet | Ship | Planet | Rocket | Ray | Shockwave | Gun | Phaser | Sun

type Template = {
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
    yield {
        Type            = EntityType.Gun;
        Act             = "gun_shoot";
        Collide         = null;
        Interval        = 300;
        Energy          = 1000.0;
        Health          = 1.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 0.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
    }
    yield {
        Type            = EntityType.Phaser;
        Act             = "phaser_shoot";
        Collide         = null;
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 1.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 0.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
    }
    yield {
        Type            = EntityType.Planet;
        Act             = "gravity";
        Collide         = null;
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Length          = 0.0;
        Mass            = 10000.;
        Radius          = 128.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
    }
    yield {
        Type            = EntityType.Ray;
        Act             = "ray_act";
        Collide         = null;
        Interval        = 0;
        Energy          = 0.0;
        Health          = 1.0;
        Length          = 0.0;
        Mass            = 0.0;
        Radius          = 512.0;
        Acceleration    = { X = 0.0; Y = 0.0 };
        Decelaration    = { X = 0.0; Y = 0.0 };
        Rotation        = 0.0;  
    }
    yield {
        Type            = EntityType.Rocket;
        Act             = "aim";
        Collide         = null;
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 1.0;
        Length          = 0.0;
        Mass            = 1.0;
        Radius          = 16.0;
        Acceleration    = { X = 500.0; Y = 20.0 };
        Decelaration    = { X = 20.0; Y = 20.0 };
        Rotation        = 1.0;  
    }
    yield {
        Type            = EntityType.Ship;
        Act             = null;
        Collide         = null;
        Interval        = 0;
        Energy          = 1000.0;
        Health          = 200.0;
        Length          = 0.0;
        Mass            = 1.0;
        Radius          = 32.0;
        Acceleration    = { X = 200.0; Y = 200.0 };
        Decelaration    = { X = 200.0; Y = 200.0 };
        Rotation        = 3.0;  
    }
}

//====================================================================================================================
// Code generation
//====================================================================================================================
let clientOutput = "Source/Client/Gameplay/Templates.cs"
let serverOutput = "Source/Server/templates.c"

// Set the thread culture to the invariant culture so that we don't get localized ToString() output for floating
// point values
Thread.CurrentThread.CurrentCulture <- CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture <- CultureInfo.InvariantCulture;

// Generate the server code
let generateServer =
    let output = new StringBuilder()
    let templateName t = (sprintf "type_%A" t).ToLower()
    let functionName f = if String.IsNullOrWhiteSpace(f) then "NULL" else f
    let setField (v : obj) = output.AppendFormat("   {0}, ", v).AppendLine() |> ignore
    let setVectorField v = output.AppendFormat("   {{ {0}, {1} }}, ", v.X, v.Y).AppendLine() |> ignore
    let entityType t = (sprintf "ENTITY_TYPE_%A" t).ToUpper()
    let prototypes projection format =
        for f in templates |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(projection t))) do
            output.AppendFormat(format, projection f).AppendLine() |> ignore

    // Write required includes
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

    // Write function prototypes
    prototypes (fun t -> t.Act) "void {0}(Entity *self);"
    prototypes (fun t -> t.Collide) "void {0}(Entity *self, Entity *other);"
    output.AppendLine() |> ignore

    // Write template definitions
    for t in templates do
        output.AppendFormat("EntityType {0} =", templateName t.Type).AppendLine().AppendLine("{") |> ignore
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