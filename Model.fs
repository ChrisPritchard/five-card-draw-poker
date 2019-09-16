module Model

type Game = {
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
    currentPlayerIndex: int
    dealerIndex: int
    state: GameState
}
with 
    member g.currentPool = g.players |> Array.sumBy (fun p -> p.currentBet)
    member g.maxBet = g.players |> Seq.map (fun p -> p.currentBet) |> Seq.max
and Player = {
    hand: (int * char) list
    cash: int
    currentBet: int
}
and GameState = 
    | Dealing
    | Discards
    | Betting
    | HandResult
    | GameOver
    
type Messages = 
    | Deal
    | Discard of (int * char) list
    | Bet of amount: int
    | Fold
    | PayOut
    | GameOver