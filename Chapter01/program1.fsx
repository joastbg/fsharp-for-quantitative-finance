/// Open the System.IO namespace
open System.IO

/// Sample stock data, from Yahoo Finance
let stockData = [
    "2013-06-06,51.15,51.66,50.83,51.52,9848400,51.52";
    "2013-06-05,52.57,52.68,50.91,51.36,14462900,51.36";
    "2013-06-04,53.74,53.75,52.22,52.59,10614700,52.59";
    "2013-06-03,53.86,53.89,52.40,53.41,13127900,53.41";
    "2013-05-31,54.70,54.91,53.99,54.10,12809700,54.10";
    "2013-05-30,55.01,55.69,54.96,55.10,8751200,55.10";
    "2013-05-29,55.15,55.40,54.53,55.05,8693700,55.05"
]

/// Split row on commas
let splitCommas (l:string) =
    l.Split(',')


/// Get the row with lowest trading volume
let lowestVolume =
    stockData
    |> List.map splitCommas
    |> List.minBy (fun x -> (int x.[5]))