/// Type to represent market data, bid ask, latest aggregation
type Quote =
    {
        bid : float
        ask : float
    }
    member this.midpoint() = (this.bid + this.ask) / 2.0

let q = {bid = 1.40; ask = 1.45} : Quote
q.midpoint()

type Quote2 =
    {
        bid : float
        ask : float
    }
    member this.midpoint() = (this.bid + this.ask) / 2.0
    member this.spread() = abs(this.bid - this.ask)


/// Change
// Often data is just sent from the feed handler, as bid or ask
type LightQuote = 
    | Bid of float | Ask of float


/// Brownian motion / Wiener process
let random = new System.Random()
let rnd() = random.NextDouble()
let data = [for i in 1 .. 10 -> rnd()]
let T = 1.0
let N = 500.0
let dt:float = T / N

/// Recursion
let primes =
    Seq.initInfinite (fun i -> i + 2) //need to skip 0 and 1 for isPrime
    |> Seq.map (fun i -> bigint(i))

let allEvens = 
    let rec loop x = seq { yield x; yield! loop (x + 2) }
    loop 0;;

Seq.take 5 allEvens

/// Sequences represent infinite number of elements
// p -> probability mean
// s -> scaling factor
let W p s =
    let rec loop x = seq { yield x; yield! loop (x + sqrt(dt)*(rnd()-(1.0-p))*s)}
    loop 0.0;;

Seq.take 50 (W 0.6 10.0)

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

do mainForm.Text <- "Wiener process in F#"
mainForm.Controls.Add(chart)

// Create serie for stock price
let wienerProcess = new Series("process")
do wienerProcess.ChartType <- SeriesChartType.Line
do wienerProcess.BorderWidth <- 2
do wienerProcess.Color <- Drawing.Color.Red
chart.Series.Add(wienerProcess)

do (Seq.take 100 (W 0.55 100.0)) |> Seq.iter (wienerProcess.Points.Add >> ignore)

