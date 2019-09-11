module Model

type Game = {
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
    currentPlayerIndex: int
    dealerIndex: int
}
and Player = {
    hand: (int * char) list
    bet: int
    cash: int
}