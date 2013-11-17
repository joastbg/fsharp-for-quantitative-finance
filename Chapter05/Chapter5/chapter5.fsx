


// FSChart
#r @"System.Windows.Forms.DataVisualization.dll" 
#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter5\packages\FSharp.Charting.0.87\lib\net40\FSharp.Charting.dll"

open System
open System.Net
open FSharp.Charting
open Microsoft.FSharp.Control.WebExtensions
open System.Windows.Forms.DataVisualization.Charting

module FSharpCharting = 
    fsi.AddPrinter(fun (ch:FSharp.Charting.ChartTypes.GenericChart) -> ch.ShowChart(); "FSharpCharting")

// Syncronous fetching (just one stock here)
let fetchOne() =
    let uri = new System.Uri("http://ichart.finance.yahoo.com/table.csv?s=ORCL&d=9&e=23&f=2012&g=d&a=2&b=13&c=2012&ignore=.csv")
    let client = new WebClient()
    let html = client.DownloadString(uri)
    html

// Parse CSV and re-arrange O,H,L,C - > H,L,O,C
let getOHLCPrices() =
    let data = fetchOne()
    data.Split('\n')
    |> Seq.skip 1
    |> Seq.map (fun s -> s.Split(','))
    |> Seq.map (fun s -> s.[0], float s.[2], float s.[3], float s.[1], float s.[4])
    |> Seq.truncate 50

// Candlestick chart price range specified
let ohlcPrices = getOHLCPrices() |> Seq.toList
Chart.Candlestick(ohlcPrices).WithYAxis(Max = 34.0, Min = 30.0)

/// Histogram
#r @"System.Windows.Forms.DataVisualization.dll" 
#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter5\packages\FSharp.Charting.0.87\lib\net40\FSharp.Charting.dll"
#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter5\packages\MathNet.Numerics.2.6.1\lib\net40\MathNet.Numerics.dll"

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Statistics
open FSharp.Charting

module FSharpCharting2 = 
    fsi.AddPrinter(fun (ch:FSharp.Charting.ChartTypes.GenericChart) -> ch.ShowChart(); "FSharpCharting")

let dist = new Normal(0.0, 1.0)
let samples = dist.Samples() |> Seq.take 10000 |> Seq.toList
let histogram = new Histogram(samples, 35);

let getValues =
    let bucketWidth = Math.Abs(histogram.LowerBound - histogram.UpperBound) / (float histogram.BucketCount)
    [0..(histogram.BucketCount-1)]
    |> Seq.map (fun i -> (histogram.Item(i).LowerBound + histogram.Item(i).UpperBound)/2.0, histogram.Item(i).Count)

Chart.Column getValues

//
type Quote =
    {
    bid : float
    ask : float
    }

let q1 : Quote = {bid = 100.0; ask = 200.0}

let (didParse, value) = Int32.TryParse("123");; 

let (bid, ask) = (100.0, 110.0);;
bid
ask

let (x, y, _) = (3.0, 2.0, 4.0);;

let (|RegexContains|_|) pattern input = 
    let matches = System.Text.RegularExpressions.Regex.Matches(input, pattern)
    if matches.Count > 0 then Some [ for m in matches -> m.Value ]
    else None

let testString = function
    | RegexContains "http://\S+" urls -> printfn "Got urls: %A" urls
    | RegexContains "[^@]@[^.]+\.\W+" emails -> printfn "Got email address: %A" emails
    | RegexContains "\d+" numbers -> printfn "Got numbers: %A" numbers
    | _ -> printfn "Didn't find anything."

let (|RegexNumber|_|) input =
    let matches = System.Text.RegularExpressions.Regex.Matches(input, "\d+.\d+")
    if matches.Count > 0 then Some [ for m in matches -> m.Value ]
    else None

let testNumbers = function
    | RegexNumber "1.21" -> printfn "Got urls: %A" 1
    | RegexNumber "not a number"
    | _ -> "nothing"

let (|Integer|_|) str =
    match System.Int32.TryParse(str) with
    | (true, int) -> Some(int)
    | _ -> None

let (|Double|_|) str =
    match System.Double.TryParse(str) with
    | (true, num) -> Some(num)
    | _ -> None

let testNumbers a = function
    | Double a -> printfn "apa %A" a
    | Integer a -> printfn "apa %A" a
    | _ -> printfn "nothing"

testNumbers "1.0"
testNumbers "1"

// create an active pattern
let (|Integer|_|) str =
   match System.Int32.TryParse(str) with
   | (true,num) -> Some(num)
   | _ -> None

// create an active pattern
let (|Double|_|) str =
   match System.Double.TryParse(str) with
   | (true,num) -> Some(num)
   | _ -> None

let testParse numberStr = 
    match numberStr with
    | Integer num -> printfn "Parsed an integer '%A'" num
    | Double num -> printfn "Parsed a double '%A'" num
    | _ -> printfn "Couldn't parse string: %A" numberStr

testParse "1.0"
testParse "1"