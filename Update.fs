﻿module Update

open Elmish
open Model
open Cards

let rec nextCard model =
    match model.deck with
    | [] ->
        let shuffled = shuffle model.rng (List.toArray model.discards)
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

let rec nextPlayerIndex model =
    let next = model.currentPlayerIndex + 1
    let wrap = if next = model.players.Length then 0 else next
    if model.players.[wrap].cash = 0 then
        nextPlayerIndex { model with currentPlayerIndex = wrap }
    else
        wrap

let rec nextDealerIndex model =
    nextPlayerIndex { model with currentPlayerIndex = model.dealerIndex }
    
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

let findWinner model =
    let winningHand = 
        model.players 
        |> Array.choose (fun player -> 
            match player.hand with [] -> None | _ -> Some player.hand)
        |> bestHand
    let winnerIndex = 
        model.players 
        |> Seq.indexed 
        |> Seq.pick (fun (i, player) -> 
            if player.hand = winningHand then Some i else None)
    Reveal winnerIndex

let betAmount amount model =
    let increaseBet player = 
        { player with currentBet = player.currentBet + amount }
    let newPlayers = 
        if amount > 0 then replaceCurrentPlayer increaseBet model
        else model.players

    let nextState = 
        if amount = 0 then findWinner model
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

    let nextState = 
        if (Array.filter (fun p -> p.hand <> []) model.players).Length = 1 then findWinner model
        else Betting

    { model with 
        players = newPlayers
        currentPlayerIndex = nextPlayerIndex model
        discards = newDiscards
        state = nextState
    }, Cmd.none

let endRound winner model =
    let hands = model.players |> Seq.collect (fun p -> p.hand) |> Seq.toList
    let newDiscards = hands @ model.discards

    let newPlayers = 
        model.players
        |> Array.mapi (fun i player ->
            let toAdd = if i = winner then model.currentPool else 0
            { player with hand = []; currentBet = 0; cash = player.cash + toAdd })

    let nextState = 
        if (Array.filter (fun p -> p.cash > 0) model.players).Length = 1 then GameOver
        else Dealing

    let newModel = 
        { model with 
            players = newPlayers
            currentPlayerIndex = nextDealerIndex model // we bump this by one in the next statement
            dealerIndex = nextDealerIndex model
            discards = newDiscards
            state = nextState }
    { newModel with currentPlayerIndex = nextPlayerIndex newModel }, Cmd.none

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
    | PayOut ->
        match model.state with
        | Reveal winner ->
            endRound winner model
        | _ -> failwith "cannot payout without winner"
    | _ -> 
        failwith "invalid message for model state"