
/// Order side
type OrderSide =
    Buy | Sell | Sellshort

/// Order type
type OrderType = 
    Market | Limit | Stop | StopLimit

/// Order status, according to FIX standard 4.2
type OrderStatus = 
    Created | New | Filled | PartiallyFilled | DoneForDay | Cancelled | Replaced | PendingCancel | Stopped | Rejected | Suspended | PendingNew | Calculated | Expired

/// Time in force
type Tif = 
    GoodForDay | GoodTilCancelled | ImmediateOrCancel | FillorKill

/// Order class
type Order(side: OrderSide, t: OrderType, p: float, tif: Tif, q: int, i: string, sp: float) =
    // Init order with status created
    let mutable St = OrderStatus.Created
    let mutable S = side
    member private this.Ts = System.DateTime.Now
    member private this.T = t
    member private this.P = p
    member private this.tif = tif
    member private this.Q = q
    member private this.I = i
    member private this.Sp = sp

    member this.Status
        with get() = St
        and set(st) = St <- st

    member this.Side
        with get() = S
        and set(s) = S <- s
    
    member this.Timestamp
        with get() = this.Ts

    member this.Type
        with get() = this.T
    
    member this.Qty
        with get() = this.Q

    member this.Price
        with get() = this.P
  
    member this.Tif
        with get() = this.tif

    member this.Instrument
        with get() = this.I

    member this.StopPrice
        with get() = this.Sp

    member this.toggleOrderSide() =
        S <- this.toggleOrderSide(S)
    
    member private this.toggleOrderSide(s: OrderSide) =
        match s with
        | Buy -> Sell
        | Sell -> Buy
        | Sellshort -> Sellshort
    
    static member (~-) (o : Order) =
        Order(o.toggleOrderSide(o.Side), o.Type, o.Price, o.tif, o.Q, o.I, o.Sp)
                             
/// Validation result, ok or failed with message
type Result = Valid of Order | Error of string

/// Validates an order for illustrative purposes
let validateOrder (result:Result) : Result = 
    match result with
    | Error s -> Error s
    | Valid order ->
        let orderType = order.Type
        let orderPrice = order.Price
        let stopPrice = order.StopPrice    
        match orderType with
        | OrderType.Limit -> 
            match orderPrice with       
            | p when p > 0.0 -> Valid order
            | _ -> Error "Limit orders must have a price > 0"
        | OrderType.Market -> Valid order
        | OrderType.Stop -> 
            match stopPrice with        
            | p when p > 0.0 -> Valid order
            | _ -> Error "Stop orders must have price > 0"
        | OrderType.StopLimit ->
            match stopPrice with
            | p when p > 0.0 && orderPrice > 0.0 -> Valid order
            | _ -> Error "Stop limit orders must both price > 0 and stop price > 0"

// Limit buy order
let buyOrder = Order(OrderSide.Buy, OrderType.Limit, 54.50, Tif.FillorKill, 100, "MSFT", 0.0)

// Limit buy order, no price
let buyOrderNoPrice = Order(OrderSide.Buy, OrderType.Limit, 0.0, Tif.FillorKill, 100, "MSFT", 0.0)

// Stop order that will be converted to limit order, no limit price
let stopLimitNoPrice = Order(OrderSide.Buy, OrderType.StopLimit, 0.0, Tif.FillorKill, 100, "MSFT", 45.50)

// Stop order that will be converted to market order
let stopNoPrice = Order(OrderSide.Buy, OrderType.Stop, 0.0, Tif.FillorKill, 100, "MSFT", 45.50)

// Stop order that will be converted to market order
let stopNoPriceNoInstrument = Order(OrderSide.Buy, OrderType.Stop, 0.0, Tif.FillorKill, 100, "", 45.50)

let buyOrderExceetsPreTradeRisk = Order(OrderSide.Sell, OrderType.Limit, 26.50, Tif.GoodForDay, 1000, "MSFT", 0.0)
let buyOrderBelowPricePreTradeRisk = Order(OrderSide.Sell, OrderType.Limit, 26.50, Tif.GoodForDay, 500, "MSFT", 0.0)

// Validate sample orders
validateOrder (Valid buyOrder) // Ok
validateOrder (Valid buyOrderNoPrice) // Failed
validateOrder (Valid stopLimitNoPrice) // Failed
validateOrder (Valid stopNoPrice) // Ok

/// Modify above to work with function composition

let validateInstrument (result:Result) : Result =
    match result with
    | Error l -> Error l
    | Valid order ->
        let orderInstrument = order.Instrument
        match orderInstrument.Length with
        | l when l > 0 -> Valid order 
        | _ -> Error "Must specify order Instrument"


validateInstrument (Valid stopNoPriceNoInstrument)

// Add check for specified Instrument
let validateOrderAndInstrument = validateOrder >> validateInstrument

validateOrderAndInstrument (Valid stopNoPriceNoInstrument)

let orderValueMax = 25000.0; // Order value max of $25,000

/// Add simple pre trade risk
let preTradeRiskRuleOne (result:Result) : Result =
    match result with
    | Error l -> Error l
    | Valid order ->
        let orderValue = (float order.Qty) * order.Price
        match orderValue with
        | v when orderValue > orderValueMax -> Error "Order value exceeded limit"
        | _ -> Valid order

// Using currying
let preTradeRiskRuleTwo (marketPrice:float) (result:Result) : Result =
    match result with
    | Error l -> Error l
    | Valid order ->
        let orderLimit = (float order.Qty) * order.Price
        match orderLimit with
        | v when orderLimit < marketPrice && order.Side = OrderSide.Buy -> Error "Order limit price below market price"
        | v when orderLimit > marketPrice && order.Side = OrderSide.Sell -> Error "Order limit price above market price"
        | _ -> Valid order

let validateOrderAndInstrumentAndPreTradeRisk = validateOrderAndInstrument >> preTradeRiskRuleOne
let validateOrderAndInstrumentAndPreTradeRisk2 marketPrice = validateOrderAndInstrument >> preTradeRiskRuleOne >> (preTradeRiskRuleTwo marketPrice)

validateOrderAndInstrumentAndPreTradeRisk (Valid stopNoPriceNoInstrument)
validateOrderAndInstrumentAndPreTradeRisk (Valid buyOrderExceetsPreTradeRisk)
validateOrderAndInstrumentAndPreTradeRisk2 25.0 (Valid buyOrderBelowPricePreTradeRisk)

/// Chain using List.reduce
let preTradeRiskRules marketPrice = [
    preTradeRiskRuleOne
    (preTradeRiskRuleTwo marketPrice)
    ]

/// Create function composition using reduce, >>, composite operator
let preTradeComposite = List.reduce (>>) (preTradeRiskRules 25.0)

preTradeComposite (Valid buyOrderExceetsPreTradeRisk)
preTradeComposite (Valid buyOrderBelowPricePreTradeRisk)