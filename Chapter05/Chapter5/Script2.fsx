#r "System.Windows.Forms.DataVisualization.dll"

open System
open System.Net
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open Microsoft.FSharp.Control.WebExtensions
open MathNet.Numerics.Distributions;

// A normally distributed random generator
let normd = new Normal(0.0, 1.0)

// Create chart and form
let chart = new Chart(Dock = DockStyle.Fill)
let area = new ChartArea("Main")
chart.ChartAreas.Add(area)

let mainForm = new Form(Visible = true, TopMost = true, 
                        Width = 700, Height = 500)

do mainForm.Text <- "Wiener process in F#"
mainForm.Controls.Add(chart)

// Create serie for stock price
let wienerProcess = new Series("process")
do wienerProcess.ChartType <- SeriesChartType.Line
do wienerProcess.BorderWidth <- 2
do wienerProcess.Color <- Drawing.Color.Red
chart.Series.Add(wienerProcess)

let random = new System.Random()
let rnd() = random.NextDouble()
//let data = [for i in 1 .. 10 -> rnd()]
let T = 1.0
let N = 500.0
let dt:float = T / N

/// Sequences represent infinite number of elements
let W s =
    let rec loop x = seq { yield x; yield! loop (x + sqrt(dt)*normd.Sample()*s)}
    loop s

wienerProcess.Points.Clear()
do (Seq.take 100 (W 55.00)) |> Seq.iter (wienerProcess.Points.Add >> ignore)