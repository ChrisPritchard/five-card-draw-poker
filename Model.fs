module Model

type Game = {
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
    currentPlayerIndex: int
    dealerIndex: int
}
with member g.currentPool = g.players |> Array.sumBy (fun p -> p.currentBet)
and Player = {
    hand: (int * char) list
    cash: int
    currentBet: int
}