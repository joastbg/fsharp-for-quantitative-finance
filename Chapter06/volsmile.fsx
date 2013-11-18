/// Black-Scholes implementation

/// Helper function
let pow x n = exp(n * log(x))

/// Cumulative distribution function
let cnd x =
   let a1 =  0.31938153
   let a2 = -0.356563782
   let a3 =  1.781477937
   let a4 = -1.821255978
   let a5 =  1.330274429
   let pi = 3.141592654
   let l  = abs(x)
   let k  = 1.0 / (1.0 + 0.2316419 * l)
   let w  = ref (1.0-1.0/sqrt(2.0*pi)*exp(-l*l/2.0)*(a1*k+a2*k*k+a3*(pow k 3.0)+a4*(pow k 4.0)+a5*(pow k 5.0)))
   if (x < 0.0) then  w := 1.0 - !w
   !w

/// Black-Scholes
// call_put_flag: 'c' if call option; otherwise put option
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes call_put_flag s x t r v =
   let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
   let d2=d1-v*sqrt(t)
   let res = ref 0.0
    
   if (call_put_flag = 'c') then
      res := s*cnd(d1)-x*exp(-r*t)*cnd(d2)
   else
      res := x*exp(-r*t)*cnd(-d2)-s*cnd(-d1)
   !res

/// Convert the nr of days to years
let days_to_years d =
    (float d) / 365.25

#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter3\packages\MathNet.Numerics.FSharp.2.5.0\lib\net40\MathNet.Numerics.FSharp.dll"
#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter5\packages\MathNet.Numerics.2.6.1\lib\net40\MathNet.Numerics.dll"

open MathNet.Numerics.Distributions;

/// Normal distribution
let normd = new Normal(0.0, 1.0)
normd.Density(100.0)

type PutCallFlag = Put | Call

/// Delta
// call_put_flag: 'c' if call option; otherwise put option
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes_delta call_put_flag s x t r v =
    let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
    match call_put_flag with
    | Put -> cnd(d1) - 1.0
    | Call -> cnd(d1)

/// Gamma
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes_gamma s x t r v =
    let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
    normd.Density(d1)

/// Vega
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes_vega s x t r v =
    let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))    
    s*normd.Density(d1)*sqrt(t)

/// Theta
// call_put_flag: 'c' if call option; otherwise put option
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes_theta call_put_flag s x t r v =
    let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
    let d2=d1-v*sqrt(t)
    let res = ref 0.0
    match call_put_flag with
    | Put -> -(s*normd.Density(d1)*v)/(2.0*sqrt(t))+r*x*exp(-r*t)*cnd(-d2)
    | Call -> -(s*normd.Density(d1)*v)/(2.0*sqrt(t))-r*x*exp(-r*t)*cnd(d2)

/// Rho
// call_put_flag: 'c' if call option; otherwise put option
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
let black_scholes_rho call_put_flag s x t r v =
    let d1=(log(s / x) + (r+v*v*0.5)*t)/(v*sqrt(t))
    let d2=d1-v*sqrt(t)
    let res = ref 0.0
    match call_put_flag with
    | Put -> -x*t*exp(-r*t)*cnd(-d2)
    | Call -> x*t*exp(-r*t)*cnd(d2)


/// Plot delta of call option as function of underlying price
#r "System.Windows.Forms.DataVisualization.dll"

open System
open System.Net
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open Microsoft.FSharp.Control.WebExtensions

/// Create chart and form
let chart = new Chart(Dock = DockStyle.Fill)
let area = new ChartArea("Main")
chart.ChartAreas.Add(area)
chart.Legends.Add(new Legend())

let mainForm = new Form(Visible = true, TopMost = true, 
                        Width = 700, Height = 500)

do mainForm.Text <- "Option delta as a function of underlying price"
mainForm.Controls.Add(chart)

/// Create serie for call option delta
let optionDeltaCall = new Series("Call option delta")
do optionDeltaCall.ChartType <- SeriesChartType.Line
do optionDeltaCall.BorderWidth <- 2
do optionDeltaCall.Color <- Drawing.Color.Red
chart.Series.Add(optionDeltaCall)

/// Create serie for call option gamma
let optionGammaCall = new Series("Call option gamma")
do optionGammaCall.ChartType <- SeriesChartType.Line
do optionGammaCall.BorderWidth <- 2
do optionGammaCall.Color <- Drawing.Color.Blue
chart.Series.Add(optionGammaCall)

/// Create serie for call option theta
let optionThetaCall = new Series("Call option theta")
do optionThetaCall.ChartType <- SeriesChartType.Line
do optionThetaCall.BorderWidth <- 2
do optionThetaCall.Color <- Drawing.Color.Green
chart.Series.Add(optionThetaCall)

/// Create serie for call option vega
let optionVegaCall = new Series("Call option vega")
do optionVegaCall.ChartType <- SeriesChartType.Line
do optionVegaCall.BorderWidth <- 2
do optionVegaCall.Color <- Drawing.Color.Purple
chart.Series.Add(optionVegaCall)

/// Calculate and plot call delta
let opd = [for x in [10.0..1.0..70.0] do yield black_scholes_delta Call x 60.0 0.5 0.01 0.3]
do opd |> Seq.iter (optionDeltaCall.Points.Add >> ignore)

/// Calculate and plot call gamma
let opg = [for x in [10.0..1.0..70.0] do yield black_scholes_gamma x 60.0 0.5 0.01 0.3]
do opg |> Seq.iter (optionGammaCall.Points.Add >> ignore)

/// Calculate and plot call theta
let opt = [for x in [10.0..1.0..70.0] do yield black_scholes_theta Call x 60.0 0.5 0.01 0.3]
do opt |> Seq.iter (optionThetaCall.Points.Add >> ignore)

/// Calculate and plot call vega
let opv = [for x in [10.0..1.0..70.0] do yield black_scholes_vega x 60.0 0.1 0.01 0.3]
do opv |> Seq.iter (optionVegaCall.Points.Add >> ignore)
