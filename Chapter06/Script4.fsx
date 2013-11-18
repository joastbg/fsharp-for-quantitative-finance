open System
open System.Net

/// Calculate the standard deviation
let stddev(values:seq<float>) =
    values
    |> Seq.fold (fun acc x -> acc + (1.0 / float (Seq.length values)) * (x - (Seq.average values)) ** 2.0) 0.0
    |> sqrt

/// Calculate logarithmic returns
let calcDailyReturns(prices:seq<float>) =
    prices
    |> Seq.pairwise
    |> Seq.map (fun (x, y) -> log (x / y))

/// Annualized volatility
let annualVolatility(returns:seq<float>) =
    let sd = stddev(calcDailyReturns(returns))
    let days = Seq.length(returns)
    sd * sqrt(float days)

let formatLeadingZero(number:int):String =
    String.Format("{0:00}", number)

/// Helper function to create the Yahoo-finance URL
let constructURL(symbol, fromDate:DateTime, toDate:DateTime) =
    let fm = formatLeadingZero(fromDate.Month-1)
    let fd = formatLeadingZero(fromDate.Day)
    let fy = formatLeadingZero(fromDate.Year)
    let tm = formatLeadingZero(toDate.Month-1)
    let td = formatLeadingZero(toDate.Day)
    let ty = formatLeadingZero(toDate.Year)
    "http://ichart.finance.yahoo.com/table.csv?s=" + symbol + "&d=" + tm + "&e=" + td + "&f=" + ty + "&g=d&a=" + fm + "&b=" + fd + "&c=" + fy + "&ignore=.csv"

/// Synchronous fetching (just one request)
let fetchOne symbol fromDate toDate =
    let url = constructURL(symbol, fromDate, toDate)
    let uri = new System.Uri(url)
    let client = new WebClient()
    let html = client.DownloadString(uri)
    html

/// Parse CSV
let getPrices stock fromDate toDate =
    let data = fetchOne stock fromDate toDate
    data.Trim().Split('\n')
    |> Seq.skip 1
    |> Seq.map (fun s -> s.Split(','))
    |> Seq.map (fun s -> float s.[4])
    |> Seq.takeWhile (fun s -> s >= 0.0)

/// Returns a formatted string with volatility for a stock
let getAnnualizedVol stock fromStr toStr =
    let prices = getPrices stock (System.DateTime.Parse fromStr) (System.DateTime.Parse toStr)
    let vol = Math.Round(annualVolatility(prices) * 100.0, 2)
    sprintf "Volatility for %s is %.2f %%" stock vol

getAnnualizedVol "MSFT" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for MSFT is 21.30 %"

getAnnualizedVol "ORCL" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for ORCL is 20.44 %"

getAnnualizedVol "GOOG" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for GOOG is 14.80 %"

getAnnualizedVol "EBAY" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for EBAY is 20.82 %"

getAnnualizedVol "AAPL" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for AAPL is 25.16 %"

getAnnualizedVol "AMZN" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for AMZN is 21.10 %"

getAnnualizedVol "^GSPC" "2013-01-01" "2013-08-29"
// val it : string = "Volatility for ^GSPC is 9.15 %"