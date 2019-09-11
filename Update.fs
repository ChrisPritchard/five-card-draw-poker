module Update

open Elmish
open Model

type Messages = 
    | Deal of playerIndex: int

let update message model = 
    match message, model.deck with
    | Deal pi, next::rest -> //  when pi >= 0 && pi < model.players.Length && model.players.[pi].hand.Length < 5
        let newPlayers = 
            model.players 
            |> Array.mapi (fun i p -> 
                if i <> pi then p 
                else { p with hand = next::p.hand } )
        { model with players = newPlayers; deck = rest }, Cmd.none
    | _ -> 
        model, Cmd.none

// messages:
// - dealer deals to all players
//  - start left of dealer and rotate until all have five
// - set blind bets and set user to after big blind
// - each player can:
//  - meet or raise (bet)
//  - pass
//  - fold
// - once all players have met or passed, the hands are revealed, ranked, and the winner gets the cash
// - game continues until all players are complete