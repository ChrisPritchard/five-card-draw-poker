module Model

open System

type Game = {
    rng: Random
    rounds: int
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
    currentPlayerIndex: int
    dealerIndex: int
    state: GameState
}
with 
    member g.currentPool = g.players |> Array.sumBy (fun p -> p.currentBet)
    member g.currentPlayer = g.players.[g.currentPlayerIndex]
    member g.maxBet = g.players |> Seq.map (fun p -> p.currentBet) |> Seq.max
    member g.finalWinner () = g.players |> Array.indexed |> Array.find (fun (_, p) -> p.cash > 0) |> fst
and Player = {
    hand: (int * char) list
    cash: int
    currentBet: int
}
and GameState = 
    | Dealing
    | Discards
    | Betting
    | Reveal of winnerIndex: int
    | GameOver
    
type Messages = 
    | Deal
    | Discard of (int * char) list
    | Bet of amount: int
    | Fold
    | PayOut