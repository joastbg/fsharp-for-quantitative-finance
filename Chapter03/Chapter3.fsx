
///////////////////////// Linear regression using least squares

open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double
open MathNet.Numerics.Distributions

let X = DenseMatrix.ofColumnsList 5 2 [ List.init 5 (fun i -> 1.0); [ 10.0; 20.0; 30.0; 40.0; 50.0 ] ]
X
let y = DenseVector [| 8.0; 21.0; 32.0; 40.0; 49.0 |]
let p = X.QR().Solve(y)

printfn "X: %A" X
printfn "y: %s" (y.ToString())
printfn "p: %s" (p.ToString())

let (a, b) = (p.[0], p.[1])


///////////////////////// Statistics

let random = new System.Random()
let rnd() = random.NextDouble()
let data = [for i in 1 .. 5 -> rnd() * 10.0]

let avg = data |> Seq.average
let sum = data |> Seq.sum

let min = data |> Seq.min
let max = data |> Seq.max


////
let calcDailyReturns(prices:seq<float>) =
    prices
    |> Seq.pairwise
    |> Seq.map (fun (x, y) -> log (x / y))

let variance(values:seq<float>) =
    values
    |> Seq.map (fun x -> (1.0 / float (Seq.length values)) * (x - (Seq.average values)) ** 2.0)
    |> Seq.sum

variance [1.0 .. 6.0]

let stddev1(values:seq<float>) =
    sqrt(variance(values))

let stddev2(values:seq<float>) =
    values    
    |> Seq.fold (fun acc x -> acc + (1.0 / float (Seq.length values)) * (x - (Seq.average values)) ** 2.0) 0.0
    |> sqrt
    
stddev1 [2.0; 4.0; 4.0; 4.0; 5.0; 5.0; 7.0; 9.0]
stddev2 [2.0; 4.0; 4.0; 4.0; 5.0; 5.0; 7.0; 9.0]

let random = new System.Random()
let rnd() = random.NextDouble()
let data = [for i in 1 .. 100 -> rnd() * 10.0]

let var = variance data
let std = stddev2 data



///////////////////////// Regression with noisy data

open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double
open MathNet.Numerics.Distributions
    
/// Define our target functions
let f1 x = Math.Sqrt(Math.Exp(x))
let f2 x = SpecialFunctions.DiGamma(x*x)

/// Sample points
let xdata = [ 1.0 .. 1.0 .. 10.0 ]

/// Create data samples, with chosen parameters and with gaussian noise added
let fy (noise:IContinuousDistribution) x = 2.5*f1(x) - 4.0*f2(x) + noise.Sample()
let ydata = xdata |> List.map (fy (Normal.WithMeanVariance(0.0,2.0)))

/// Build matrix form
let X =
    [
        xdata |> List.map f1
        xdata |> List.map f2
    ] |> DenseMatrix.ofColumnsList 10 2
let y = DenseVector.ofList ydata

/// Solve
let p = X.QR().Solve(y)

printfn "X: %A" X
printfn "y: %s" (y.ToString())
printfn "p: %s" (p.ToString())

(p.[0], p.[1])


///////////////////////// Implementing algorithms

let rec bisect n N (f:float -> float) (a:float) (b:float) (t:float) : float =
    if n >= N then -1.0
    else
        let c = (a + b) / 2.0
        if f(c) = 0.0 || (b - a) / 2.0 < t then
            // Solution found
            c
        else
            if sign(f(c)) = sign(f(a)) then
                bisect (n + 1) N f c b t
            else    
                bisect (n + 1) N f a c t

let f = (fun x -> (x**2.0 - x - 6.0))
f(-2.0)
f(3.0)

let first = bisect 0 25 f -10.0 0.0 0.01
let second = bisect 0 25 f 0.0 10.0 0.01

first;;
second;;

///////////////////////// Statistics

/// Helpers to generate random numbers
let random = new Random()
let rnd() = random.NextDouble()
let data = [for i in 1 .. 500 -> rnd() * 10.0]

/// Calculates the variance of a sequence
let variance(values:seq<float>) =
    values
    |> Seq.map (fun x -> (1.0 / float (Seq.length values)) * (x - (Seq.average values)) ** 2.0)
    |> Seq.sum

/// Calculates the standard deviation of a sequence
let stddev(values:seq<float>) =
    values    
    |> Seq.fold (fun acc x -> acc + (1.0 / float (Seq.length values)) * (x - (Seq.average values)) ** 2.0) 0.0
    |> sqrt

let avg = data |> Seq.average
let sum = data |> Seq.sum
let min = data |> Seq.min
let max = data |> Seq.max
let var = data |> variance
let std = data |> stddev