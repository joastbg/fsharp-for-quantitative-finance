open System.IO
open FSharp.Charting
open System.Windows.Forms.DataVisualization.Charting

let mlist = [for x in [0..10] do yield (x, x*2)]

/// Plot values using FSharpChart
fsi.AddPrinter(fun (ch:FSharp.Charting.ChartTypes.GenericChart) -> ch.ShowChart(); "FSharpChartingSmile")
Chart.Line(mlist)

//Chart.Line(fits).WithTitle("Volatility Smile")

/// Payoff for European call option
// s: stock price
// k: strike price of option
let payoffCall k s =
    max (s-k) 0.0

/// Payoff for European Put option
// s: stock price
// k: strike price of option
let payoffPut k s =
    max (k-s) 0.0

// Calculate the payoff of a given option 
let payoff payoffFunction = 
    [ for s in 0.0 .. 10.0 .. 100.0 -> s, payoffFunction s ]

// Compare the payoff of call and put options
let callPayoff = payoff (payoffCall 50.0)
let putPayoff = payoff (payoffPut 50.0)

let chart = Chart.Combine [Chart.Line(callPayoff); Chart.Line(putPayoff).WithTitle("Payoff diagram")]

//Chart.Line(callPayoff).WithTitle("Payoff - Call Option")
Chart.Line(putPayoff).WithTitle("Payoff - Put Option")

// Display the payoff of European call and put options
let chart = Chart.Combine [ Chart.Line(callPayoff, Name="Call option").WithLegend(); Chart.Line(putPayoff, Name="Put option").WithLegend().WithTitle("Payoff diagram") ]
chart.WithTitle("Payoff diagram")

////////////

/// Payoff for long straddle
// s: stock price
// k: strike price of option
let longStraddle k s = 
    (payoffCall k s) +
    (payoffPut k s) 

/// Payoff for Short straddle
// s: stock price
// k: strike price of option
let shortStraddle k s = 
    -(payoffCall k s) +
    -(payoffPut k s) 

/// Payoff for long butterfly
// s: stock price
// h: high price
// l: low price
let longButterfly l h s = 
    (payoffCall l s) +
    (payoffCall h s) -
    2.0 * (payoffCall ((l + h) / 2.0) s)

/// Payoff for short butterfly
// s: stock price
// h: high price
// l: low price
let shortButterfly l h s = 
    -(payoffCall l s) +
    -(payoffCall h s) -
    2.0 * -(payoffCall ((l + h) / 2.0) s)

Chart.Line(payoff (longStraddle 50.0)).WithTitle("Payoff - Long straddle")

Chart.Line(payoff (shortStraddle 50.0)).WithTitle("Payoff - Short straddle")

Chart.Line(payoff (longButterfly 20.0 80.0)).WithTitle("Payoff - Long butterfly")

Chart.Line(payoff (shortButterfly 20.0 80.0)).WithTitle("Payoff - Short butterfly")