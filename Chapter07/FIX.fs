namespace TradingSystem

    (*
        If used in F# Interactive
        #r @"PATH_TO\QuickFix.dll"
    *)

    open QuickFix
    open QuickFix.Transport
    open QuickFix.FIX42
    open System.Globalization
    open System
    open Orders
    open QuickFix.Fields
    open System.ComponentModel

    module FIX =
        type ClientInitiator(orders:BindingList<Order>) = 
            member this.findOrder str = 
                try 
                    Some (orders |> Seq.find (fun o -> o.Timestamp = str))
                with | _ as ex -> printfn "Exception: %A" ex.Message; None
            interface IApplication with
                member this.OnCreate(sessionID : SessionID) : unit = printfn "OnCreate"
                member this.ToAdmin(msg : QuickFix.Message, sessionID : SessionID) : unit = printfn "ToAdmin %A" msg
                member this.FromAdmin(msg : QuickFix.Message, sessionID : SessionID) : unit = printfn "FromAdmin %A" msg
                member this.ToApp(msg : QuickFix.Message, sessionID : SessionID) : unit = printfn "FromAdmin - Report: %A" msg         
                member this.FromApp(msg : QuickFix.Message, sessionID : QuickFix.SessionID) : unit =
                    match msg with
                    | :? ExecutionReport as report ->
                        let qty = report.CumQty
                        let avg = report.AvgPx
                        let sta = report.OrdStatus
                        let oid = report.ClOrdID
                        let lqty = report.LeavesQty
                        let eqty = report.CumQty
                        let debug = fun str -> printfn "ExecutionReport (%s) # avg price: %s | qty: %s | status: %s | orderId: %s" str (avg.ToString()) (qty.ToString()) (sta.ToString()) (oid.ToString())
                        match sta.getValue() with
                        | OrdStatus.NEW ->                            
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.New
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "NEW"
                        | OrdStatus.FILLED ->
                            /// Update avg price, open price, ex price
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.Filled
                                o.AveragePrice <- double (avg.getValue())
                                o.OpenQty <- int (lqty.getValue())
                                o.ExecutedQty <- int (eqty.getValue())
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "FILLED"
                        | OrdStatus.PARTIALLY_FILLED ->                   
                            /// Update avg price, open price, ex price
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.PartiallyFilled
                                o.AveragePrice <- double (avg.getValue())
                                o.OpenQty <- int (lqty.getValue())
                                o.ExecutedQty <- int (eqty.getValue())
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "PARTIALLY_FILLED"
                        | OrdStatus.CANCELED ->
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.Cancelled
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "CANCELED"
                        | OrdStatus.REJECTED ->                             
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.Rejected
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "REJECTED"
                        | OrdStatus.REPLACED ->
                            match this.findOrder(oid.ToString()) with
                            | Some(o) ->
                                o.Status <- OrderStatus.Replaced                                
                            | _ -> printfn "ERROR: The order was not found in OMS"
                            debug "REPLACED"
                        | OrdStatus.EXPIRED -> 
                            printfn "ExecutionReport (EXPIRED) %A" report
                        | _ -> printfn "ExecutionReport (other) %A" report
                    | _ -> ()
                    
                member this.OnLogout(sessionID : SessionID) : unit = printf "OnLogout"
                member this.OnLogon(sessionID : SessionID) : unit = printf "OnLogon"

        type ConsoleLog() =
            interface ILog with
                member this.Clear() : unit = printf "hello"
                member this.OnEvent(str : string) : unit = printfn "%s" str
                member this.OnIncoming(str : string) : unit = printfn "%s" str
                member this.OnOutgoing(str : string) : unit = printfn "%s" str

        type ConsoleLogFactory(settings : SessionSettings) = 
            interface ILogFactory with
                member this.Create(sessionID : SessionID) : ILog = new NullLog() :> ILog

        type FIXEngine(orders:BindingList<Order>) =
            let settings = new SessionSettings(@"conf\config.cfg")
            let application = new ClientInitiator(orders)
            let storeFactory = FileStoreFactory(settings)
            let logFactory = new ConsoleLogFactory(settings)
            let messageFactory = new MessageFactory()
            let initiator = new SocketInitiator(application, storeFactory, settings)
            let currentID = initiator.GetSessionIDs() |> Seq.head
            let orders = orders
            member this.init() : unit =
                ()
            member this.start() : unit =
                initiator.Start()
            member this.stop() : unit =
                initiator.Stop()
            member this.sendOrder(order:Order) : unit =
                let fixOrder = new NewOrderSingle()
                // Convert to Order to NewOrderSingle
                fixOrder.Symbol <- new Symbol(order.Instrument)
                fixOrder.ClOrdID <- new ClOrdID(order.Timestamp)
                fixOrder.OrderQty <- new OrderQty(decimal order.Qty)
                fixOrder.OrdType <- new OrdType('2'); // Limit order
                fixOrder.Side <- new Side('1');
                fixOrder.Price <- new Price(decimal order.Price);
                fixOrder.TransactTime <- new TransactTime();
                fixOrder.HandlInst <- new HandlInst('2');
                fixOrder.SecurityType <- new SecurityType("OPT"); // Option
                fixOrder.Currency <- new Currency("USD");
                // Add to OMS
                orders.Add(order)
                // Send order to target
                Session.SendToTarget(fixOrder, currentID) |> ignore

    module Apa =
        // Use a list of NewOrderSingle as first step
        let orders = new BindingList<Order>()
        let fixEngine = new FIX.FIXEngine(orders)
        fixEngine.init()
        fixEngine.start()
        let buyOrder1 = new Order(OrderSide.Buy, OrderType.Limit, 24.50, Tif.GoodForDay, 100, "ERICB4A115", 0.0)
        let buyOrder2 = new Order(OrderSide.Buy, OrderType.Limit, 34.50, Tif.GoodForDay, 100, "ERICB4A116", 0.0)
        let buyOrder3 = new Order(OrderSide.Buy, OrderType.Limit, 44.50, Tif.GoodForDay, 100, "ERICB4A117", 0.0)
        fixEngine.sendOrder(buyOrder1)
        fixEngine.sendOrder(buyOrder2)
        fixEngine.sendOrder(buyOrder3)