module Update

open System
open Elmish
open Model
open Cards

type Messages = 
    | Deal of playerIndex: int
    | DealAll
    | Bet of playerIndex: int * amount: int
    | Fold of playerIndex: int
    | PayOut
    | GameOver

let random = Random ()

let rec dealCard playerIndex model =
    match model.deck with
    | [] ->
        let shuffled = shuffle random (List.toArray model.discards)
        dealCard playerIndex { model with deck = shuffled; discards = [] }
    | next::rest ->
        let newPlayers = 
            model.players 
            |> Array.mapi (fun i player -> 
                if i = playerIndex then { player with hand = next::player.hand }
                else player)
        { model with players = newPlayers; deck = rest }

let update message model = 
    match message with
    | Deal pi when pi >= 0 && pi < model.players.Length && model.players.[pi].hand.Length < 5 ->
        dealCard pi model, Cmd.none
    | _ -> 
        failwith "invalid message for model state"

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