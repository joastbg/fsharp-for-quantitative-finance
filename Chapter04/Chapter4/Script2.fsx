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

let mainForm = new Form(Visible = true, TopMost = true, 
                        Width = 700, Height = 500)

do mainForm.Text <- "Yahoo Finance data in F#"
mainForm.Controls.Add(chart)

// Create serie for stock price
let stockPrice = new Series("stockPrice")
do stockPrice.ChartType <- SeriesChartType.Line
do stockPrice.BorderWidth <- 2
do stockPrice.Color <- Drawing.Color.Red
chart.Series.Add(stockPrice)

// Create serie for moving average
let movingAvg = new Series("movingAvg")
do movingAvg.ChartType <- SeriesChartType.Line
do movingAvg.BorderWidth <- 2
do movingAvg.Color <- Drawing.Color.Blue
chart.Series.Add(movingAvg)

// Syncronous fetching (just one stock here)
let fetchOne() =
    let uri = new System.Uri("http://ichart.finance.yahoo.com/table.csv?s=ORCL&d=9&e=23&f=2012&g=d&a=2&b=13&c=1986&ignore=.csv")
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

// The plotting
let sp = getPrices()
do sp |> Seq.iter (stockPrice.Points.Add >> ignore)

let ma = movingAverage 100 sp
do ma |> Seq.iter (movingAvg.Points.Add >> ignore)