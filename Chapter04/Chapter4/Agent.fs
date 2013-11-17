namespace Agents

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