// Another example with Bollinger Bands
#r "System.Windows.Forms.DataVisualization.dll" 

open System
open System.Net
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open Microsoft.FSharp.Control.WebExtensions

// Create chart and form
let chart = new Chart(Dock = DockStyle.Fill)
let area = new ChartArea("Main")
chart.ChartAreas.Add(area)

// Add legends
chart.Legends.Add(new Legend())

let mainForm = new Form(Visible = true, TopMost = true, 
                        Width = 700, Height = 500)

do mainForm.Text <- "Yahoo Finance data in F# - Bollinger Bands"
mainForm.Controls.Add(chart)

// Create serie for stock price
let stockPrice = new Series("stockPrice")
do stockPrice.ChartType <- SeriesChartType.Line
do stockPrice.BorderWidth <- 2
do stockPrice.Color <- Drawing.Color.DarkGray
chart.Series.Add(stockPrice)

// Create serie for moving average
let movingAvg = new Series("movingAvg")
do movingAvg.ChartType <- SeriesChartType.Line
do movingAvg.BorderWidth <- 2
do movingAvg.Color <- Drawing.Color.Blue
chart.Series.Add(movingAvg)

// Create serie for upper band
let upperBand = new Series("upperBand")
do upperBand.ChartType <- SeriesChartType.Line
do upperBand.BorderWidth <- 2
do upperBand.Color <- Drawing.Color.Red
chart.Series.Add(upperBand)

// Create serie for lower band
let lowerBand = new Series("lowerBand")
do lowerBand.ChartType <- SeriesChartType.Line
do lowerBand.BorderWidth <- 2
do lowerBand.Color <- Drawing.Color.Green
chart.Series.Add(lowerBand)

// Syncronous fetching (just one stock here)
let fetchOne() =
    let uri = new System.Uri("http://ichart.finance.yahoo.com/table.csv?s=ORCL&d=9&e=23&f=2012&g=d&a=2&b=13&c=2012&ignore=.csv")
    let client = new WebClient()
    let html = client.DownloadString(uri)
    html

// Parse CSV
let getPrices() =
    let data = fetchOne()
    data.Split('\n')
    |> Seq.skip 1
    |> Seq.map (fun s -> s.Split(','))
    |> Seq.map (fun s -> float s.[4])
    |> Seq.truncate 2500

// Calc moving average
let movingAverage n (prices:seq<float>) =
    prices    
    |> Seq.windowed n
    |> Seq.map Array.sum
    |> Seq.map (fun a -> a / float n)

// Stddev
let stddev2(values:seq<float>) =
    let avg = Seq.average values
    values    
    |> Seq.fold (fun acc x -> acc + (1.0 / float (Seq.length values)) * (x - avg) ** 2.0) 0.0
    |> sqrt

let movingStdDev n (prices:seq<float>) =
    prices
    |> Seq.windowed n
    |> Seq.map stddev2

// The plotting
let sp = getPrices()
do sp |> Seq.iter (stockPrice.Points.Add >> ignore)

let ma = movingAverage 100 sp
do ma |> Seq.iter (movingAvg.Points.Add >> ignore)

// Bollinger bands, K = 2.0
let ub = movingStdDev 100 sp
// Upper
Seq.zip ub ma |> Seq.map (fun (a,b) -> b + 2.0 * a) |> Seq.iter (upperBand.Points.Add >> ignore)
// Lower
Seq.zip ub ma |> Seq.map (fun (a,b) -> b - 2.0 * a) |> Seq.iter (lowerBand.Points.Add >> ignore)