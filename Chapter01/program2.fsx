/// Open the System.IO namespace
open System.IO

let filePath = "table.csv"

/// Split row on commas
let splitCommas (l:string) =
    l.Split(',')

/// Read a file into a string array
let openFile (name : string) =
    try
        let content = File.ReadAllLines(name)
        content |> Array.toList
    with
        | :? System.IO.FileNotFoundException as e -> printfn "Exception! %s " e.Message; ["empty"]

/// Get the row with lowest trading volume, from file
let lowestVolume =
    openFile filePath
    |> List.map splitCommas
    |> Seq.skip 1
    |> Seq.minBy (fun x -> (int x.[5]))

/// Use printfn with generic formatter, %A
printfn "Lowest volume, found in row: %A" lowestVolume
lowestVolume.[0]
