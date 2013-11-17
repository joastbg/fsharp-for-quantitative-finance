/// Monte Carlo implementation

/// Convert the nr of days to years
let days_to_years d =
    (float d) / 365.25

/// Asset price at maturity for sample rnd
// s: stock price
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
// rnd: sample
let price_for_sample s t r v rnd =
    s*exp((r-v*v/2.0)*t+v*rnd*sqrt(t))


/// For each sample we run the monte carlo simulation
// s: stock price
// x: strike price of option
// t: time to expiration in years
// r: risk free interest rate
// v: volatility
// samples: random samples as input to simulation
let monte_carlo s x t r v (samples:seq<float>) =
    samples
    |> Seq.map (fun rnd -> (price_for_sample s t r v rnd) - x)
    |> Seq.average

///// Generate sample sequence
//let random = new System.Random()
//let rnd() = random.NextDouble()
//let data = [for i in 1 .. 1000 -> rnd() * 1.0]

/// Monte carlo for call option
//monte_carlo 58.60 60.0 0.5 0.01 0.3 data


/// Generate sample sequence
let random = new System.Random()
let rnd() = random.NextDouble()
let data = [for i in 1 .. 1000 -> rnd() * 1.0]

/// Monte carlo for call option
monte_carlo 58.60 60.0 0.5 0.01 0.3 data


let genRandomNumber (n) =
    let rnd = new System.Random()
    float (rnd.Next(n, 100))

genRandomNumber 10

open System.Windows.Forms 