module Model

type Game = {
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
    currentPlayerIndex: int
    dealerIndex: int
    currentPool: int
}
and Player = {
    hand: (int * char) list
    state: PlayerState
    cash: int
}
and PlayerState = 
    | Playing of hand: (int * char) list * bet: int
    | Folded