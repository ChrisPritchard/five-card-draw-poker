# five-card-draw-poker

A simple implementation of a simple variant of Poker, in F# and Elmish, for educational purposes.

## Development Steps

1. I described the game in plain english, coming up with this description:

```text
- dealer deals to all players
  - start left of dealer and rotate until all have five
 - set blind bets and set user to after big blind
 - each player can:
  - meet or raise (bet)
  - pass
  - fold
 - once all players have met or passed, the hands are revealed, ranked, and the winner gets the cash
 - game continues until all players are complete
```

2. From the above I sketched out something that would model the above, 'designing with types' as it were:

```fsharp
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
```

3. Next I came up with a list of messages, things that can occur in a game that would trigger a state change:

```fsharp
type Messages = 
    | Deal of playerIndex: int
    | DealAll
    | Bet of playerIndex: int * amount: int
    | Fold of playerIndex: int
    | PayOut
    | GameOver
```

4. With the model and messages in hand, I got to work creating the update function, which mutates a model based on a given message. Through the course of this work, the game's model and messages were altered as they were implemented.

  - For each message, I created a function that transforms the model into the new gamestate
  - The core update function calls these sub functions based on the message that comes through
  - Where appropriate, the update function can gate a given sub function based on model conditions (e.g. preventing a bet of less than the minimum)
