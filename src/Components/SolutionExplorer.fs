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
        | Project of name: string * Nodes: Model list
        | File of name: string
        | Reference of name: string

    let private getModel() =
        let projects = Project.getLoadedProjects ()
        projects |> Map.toList |> List.map (fun (_,proj) ->
            let files = proj.Files |> List.map File
            let refs = proj.References |> List.map Reference
            let nodes = [yield! refs; yield! files]
            let name = proj.Project |> path.basename
            Project(name,nodes)
        ) |> Workspace

    let private getChildren node =
        match node with
            | Workspace projects -> projects
            | Project (name, nodes) -> nodes
            | File _ -> []
            | Reference _ -> []
        |> List.toArray

    let private getLabel node =
        match node with
        | Workspace _ -> "Workspace"
        | Project (name,_) -> name
        | File name -> name
        | Reference name -> name

    let private hasChildren node =
        match node with
        | Workspace _ -> true
        | Project (name,_) -> true
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



