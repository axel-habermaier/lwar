open System
open System.IO
open System.Linq
open System.Xml

let modifyCompileAs (path : string) =
    let content = File.ReadAllText(path);
    let modified = content.Replace("<CompileAs>CompileAsC</CompileAs>", "<CompileAs>CompileAsCpp</CompileAs>")
    File.WriteAllText(path, modified)

let modifyPlatform (path : string) =
    let content = File.ReadAllText(path);
    let modified = content.Replace("Label=\"Configuration\">", "Label=\"Configuration\"><PlatformToolset>v110</PlatformToolset>")
    File.WriteAllText(path, modified)

let modify (path : string) =
    modifyCompileAs path
    modifyPlatform path

modify "build/client.vcxproj"
modify "build/server.vcxproj"
modify "build/shared.vcxproj"