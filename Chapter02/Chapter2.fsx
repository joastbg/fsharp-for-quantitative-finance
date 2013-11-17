/// Background workers (1)

open System.Threading
open System.ComponentModel

let worker = new BackgroundWorker()
worker.DoWork.Add(fun args ->    
    for i in 1 .. 50 do
        // Simulates heavy calculation
        Thread.Sleep(1000)
        printfn "%A" i
)

worker.RunWorkerCompleted.Add(fun args ->
    printfn "Completed..."
)

worker.RunWorkerAsync()

/// Background workers (2)

open System.Threading
open System.ComponentModel

let worker = new BackgroundWorker()
worker.DoWork.Add(fun args ->    
    for i in 1 .. 50 do
        // Simulates heavy calculation
        Thread.Sleep(1000)
        printfn "A: %A" i
)

worker.DoWork.Add(fun args ->    
    for i in 1 .. 10 do
        // Simulates heavy calculation
        Thread.Sleep(500)
        printfn "B: %A" i
)

worker.RunWorkerCompleted.Add(fun args ->
    printfn "Completed..."
)

worker.RunWorkerAsync()

/// Background workers (3)
open System.ComponentModel

let workerCancel = new BackgroundWorker(WorkerSupportsCancellation = true)
workerCancel.DoWork.Add(fun args ->
    printfn "apan %A" args
    for i in 1 .. 50 do
        if (workerCancel.CancellationPending = false) then
            Thread.Sleep(1000)
            printfn "%A" i
)

workerCancel.RunWorkerCompleted.Add(fun args ->
    printfn "Completed..."
)

workerCancel.RunWorkerAsync()
workerCancel.CancelAsync()

/// Threads (1)
open System.Threading

let runMe() = 
    for i in 1 .. 10 do
        try
            Thread.Sleep(1000)
        with
            | :? System.Threading.ThreadAbortException as ex -> printfn "Exception %A" ex
        printfn "I'm still running..."

let thread = new Thread(runMe)
thread.Start()

/// Threads (2)
open System.Threading

let runMe() = 
    for i in 1 .. 10 do
        try
            Thread.Sleep(1000)
        with
            | :? System.Threading.ThreadAbortException as ex -> printfn "Exception %A" ex
        printfn "I'm still running..."

let createThread() =
    let thread = new Thread(runMe)
    thread.Start()

createThread()
createThread()

/// Thread pools (1)
open System.Threading

let runMe(arg:obj) = 
    for i in 1 .. 10 do
        try
            Thread.Sleep(1000)
        with
            | :? System.Threading.ThreadAbortException as ex -> printfn "Exception %A" ex
        printfn "%A still running..." arg

ThreadPool.QueueUserWorkItem(new WaitCallback(runMe), "One")
ThreadPool.QueueUserWorkItem(new WaitCallback(runMe), "Two")
ThreadPool.QueueUserWorkItem(new WaitCallback(runMe), "Three")

/// Async example (1)
open System.Net
open Microsoft.FSharp.Control.WebExtensions

/// Stock symbol and URL to Yahoo finance
let urlList = [ "MSFT", "http://ichart.finance.yahoo.com/table.csv?s=MSFT&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv" 
                "GOOG", "http://ichart.finance.yahoo.com/table.csv?s=GOOG&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv" 
                "EBAY", "http://ichart.finance.yahoo.com/table.csv?s=EBAY&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
                "AAPL", "http://ichart.finance.yahoo.com/table.csv?s=AAPL&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
                "ADBE", "http://ichart.finance.yahoo.com/table.csv?s=ADBE&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
              ]

/// Async fetch of CSV data
let fetchAsync(name, url:string) =
    async { 
        try 
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)
            printfn "Downloaded historical data for %s, recieved %d characters" name html.Length
        with
            | ex -> printfn "Exception: %s" ex.Message
    }

/// Helper function to run in async parallel
let runAll() =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel 
    |> Async.RunSynchronously
    |> ignore

/// Get max closing price from 2010-01-01 for each stock
runAll()

/// Async example (2)
open System.Net
open Microsoft.FSharp.Control.WebExtensions

/// Stock symbol and URL to Yahoo finance
let urlList = [ "MSFT", "http://ichart.finance.yahoo.com/table.csv?s=MSFT&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv" 
                "GOOG", "http://ichart.finance.yahoo.com/table.csv?s=GOOG&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv" 
                "EBAY", "http://ichart.finance.yahoo.com/table.csv?s=EBAY&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
                "AAPL", "http://ichart.finance.yahoo.com/table.csv?s=AAPL&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
                "ADBE", "http://ichart.finance.yahoo.com/table.csv?s=ADBE&d=6&e=6&f=2013&g=d&a=1&b=1&c=2010&ignore=.csv"
              ]

/// Parse CSV and extract max price
let getMaxPrice(data:string) =   
    let rows = data.Split('\n')
    rows
    |> Seq.skip 1
    |> Seq.map (fun s -> s.Split(','))
    |> Seq.map (fun s -> float s.[4])    
    |> Seq.take (rows.Length - 2)
    |> Seq.max

/// Async fetch of CSV data
let fetchAsync(name, url:string) =
    async { 
        try 
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)            
            let maxprice = (getMaxPrice(html.ToString()))
            printfn "Downloaded historical data for %s, max closing price since 2010-01-01: %f" name maxprice
        with
            | ex -> printfn "Exception: %s" ex.Message
    }

/// Helper function to run in async parallel
let runAll() =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel 
    |> Async.RunSynchronously
    |> ignore

/// Get max closing price from 2010-01-01 for each stock
runAll()

/// MailboxProcessor (1) - Max agent
open System

// Type for our agent
type Agent<'T> = MailboxProcessor<'T>

// Control messages to be sent to agent
type CounterMessage = 
    | Update of float
    | Reset

module Helpers =
    let genRandomNumber (n) =
        let rnd = new System.Random()
        float (rnd.Next(n, 100))

module MaxAgent =
    // Agent to keep track of max value and update GUI
    let sampleAgent = Agent.Start(fun inbox ->
        let rec loop max = async {
            let! msg = inbox.Receive()
            match msg with
            | Reset ->
                return! loop 0.0
            | Update value ->
                let max = Math.Max(max, value)

                Console.WriteLine("Max: " + max.ToString())

                do! Async.Sleep(1000)
                return! loop max
        } 
        loop 0.0)

let agent = MaxAgent.sampleAgent
let random = Helpers.genRandomNumber 5
agent.Post(Update random)

/// OOP (1)
type Order(s: OrderSide, t: OrderType, p: float) =
    let mutable S = s
    member this.T = t
    member this.P = p

    member this.Side
        with get()  = S
        and  set(s) = S <- s

    member this.Type
        with get() = this.T
        
    member this.Price
        with get() = this.P

    member this.toggleOrderSide() =
        match S with
        | Buy -> S <- Sell
        | Sell -> S <- Buy

/// OOP - Overloaded operators (1)
type Order(s: OrderSide, t: OrderType, p: float) =
    let mutable S = s
    member this.T = t
    member this.P = p

    member this.Side
        with get()  = S
        and  set(s) = S <- s

    member this.Type
        with get() = this.T
        
    member this.Price
        with get() = this.P

    member this.toggleOrderSide() =
        S <- this.toggleOrderSide(S)

    member private this.toggleOrderSide(s: OrderSide) =
        match s with
        | Buy -> Sell
        | Sell -> Buy

    static member (~-) (o : Order) =
        Order(o.toggleOrderSide(o.Side), o.Type, o.Price)