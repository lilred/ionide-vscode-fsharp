namespace Ionide.VSCode.FSharp

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.vscode
open Fable.Import.Node
open Ionide.VSCode.Helpers

open DTO
open Ionide.VSCode.Helpers

module SolutionExplorer =

    type Model =
        | Workspace of Projects : Model list
        | ReferenceList of References: Model list
        | FileList of Files: Model list
        | Project of name: string * ReferenceList: Model * FileList: Model
        | File of name: string
        | Reference of name: string

    let private getModel() =
        let projects = Project.getLoadedProjects ()
        projects |> Map.toList |> List.map (fun (_,proj) ->
            let files = proj.Files |> List.map (path.basename) |> List.map File |> FileList
            let refs = proj.References |> List.map (path.basename) |> List.map Reference |> ReferenceList
            let name = proj.Project |> path.basename
            Project(name,files, refs)
        ) |> Workspace

    let private getChildren node =
        match node with
            | Workspace projects -> projects
            | Project (name, files, refs) -> [yield refs; yield files]
            | ReferenceList refs -> refs
            | FileList files -> files
            | File _ -> []
            | Reference _ -> []
        |> List.toArray

    let private getLabel node =
        match node with
        | Workspace _ -> "Workspace"
        | Project (name,_,_) -> name
        | ReferenceList _ -> "References"
        | FileList _ -> "Files"
        | File name -> name
        | Reference name -> name

    let private hasChildren node =
        match node with
        | Workspace _ -> true
        | Project (name,_,_) -> true
        | ReferenceList refs -> refs |> List.isEmpty |> not
        | FileList files -> files |> List.isEmpty |> not
        | File name -> false
        | Reference name -> false

    let getClickCommand node =
        null


    let private createProvider () : TreeExplorerNodeProvider<Model> =

        { new TreeExplorerNodeProvider<Model>
          with
            member this.provideRootNode() =
                getModel() |> Case1

            member this.resolveChildren(node) =
                getChildren node |> Case1

            member this.getLabel(node) =
                getLabel node

            member this.getHasChildren(node) =
                hasChildren node

            member this.getClickCommand(node) =
                getClickCommand node


        }

    let activate () =
        window.registerTreeExplorerNodeProvider("ionideSolution", createProvider())
        |> ignore

        ()



