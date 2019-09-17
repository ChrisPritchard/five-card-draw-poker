module Update

open System
open Elmish
open Model
open Cards

let random = Random ()

let rec nextCard model =
    match model.deck with
    | [] ->
        let shuffled = shuffle random (List.toArray model.discards)
        nextCard { model with deck = shuffled; discards = [] }
    | next::rest ->
        next, { model with deck = rest }

let nextCards n model =
    let rec deal acc model n =
        if n = 0 then acc, model
        else
            let next, model = nextCard model
            deal (next::acc) model (n - 1)
    deal [] model n

let nextPlayerIndex model =
    let next = model.currentPlayerIndex + 1
    if next = model.players.Length then 0 else next
    
let replaceCurrentPlayer newPlayer model =
    model.players 
    |> Array.mapi (fun i player -> 
        if i = model.currentPlayerIndex then newPlayer player
        else player)

let rec dealCard model =
    let nextCard, model = nextCard model

    let addCard player = { player with hand = nextCard::player.hand }
    let newPlayers = replaceCurrentPlayer addCard model

    let nextState = 
        if Array.exists (fun p -> p.hand.Length < 5) newPlayers
        then Dealing else Discards

    { model with 
        players = newPlayers
        currentPlayerIndex = nextPlayerIndex model
        state = nextState
    }, Cmd.none

let discardCards cards model =
    let newCards, model = nextCards (List.length cards) model

    let replaceHand player = 
        let afterDiscard = List.except cards player.hand
        { player with hand = newCards @ afterDiscard }
    let newPlayers = replaceCurrentPlayer replaceHand model

    let nextState = 
        if model.currentPlayerIndex = model.dealerIndex then Betting
        else Discards

    { model with 
        players = newPlayers
        currentPlayerIndex = nextPlayerIndex model
        state = nextState
    }, Cmd.none

let betAmount amount model =
    let increaseBet player = 
        { player with currentBet = player.currentBet + amount }
    let newPlayers = 
        if amount > 0 then replaceCurrentPlayer increaseBet model
        else model.players

    let nextState = 
        if amount = 0 then Reveal
        else Betting

    { model with 
        players = newPlayers
        currentPlayerIndex = nextPlayerIndex model
        state = nextState
    }, Cmd.none

let foldPlayer model =
    let foldPlayer player = { player with hand = [] }
    let newPlayers = replaceCurrentPlayer foldPlayer model
    let newDiscards = model.currentPlayer.hand @ model.discards

    { model with 
        players = newPlayers
        currentPlayerIndex = nextPlayerIndex model
        discards = newDiscards
    }, Cmd.none

let update message model = 
    match message with
    | Deal ->
        dealCard model
    | Discard cards ->
        discardCards cards model
    | Bet amount when model.currentPlayer.currentBet + amount >= model.maxBet ->
        betAmount amount model
    | Fold ->
        foldPlayer model
    | _ -> 
        failwith "invalid message for model state"

// messages:
// - dealer deals to all players
//  - start left of dealer and rotate until all have five
// - each player can choose zero to five cards from there hand to discard and be redealt
// - set blind bets and set user to after big blind
// - each player can:
//  - meet or raise (bet)
//  - pass
//  - fold
// - once all players have met or passed, the hands are revealed, ranked, and the winner gets the cash
// - game continues until all players are complete