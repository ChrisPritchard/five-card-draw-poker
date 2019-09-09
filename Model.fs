
open System
open Elmish

let printcard (rank: int, suit: char) =
    match rank with
    | 14 -> sprintf "A%c" suit
    | 13 -> sprintf "K%c" suit
    | 12 -> sprintf "Q%c" suit
    | 11 -> sprintf "J%c" suit
    | 10 -> sprintf "X%c" suit
    | _ -> sprintf "%i%c" rank suit

let standardDeck = 
    Array.init 52 (fun i -> 
        match i % 4 with
        | 0 -> (i % 13) + 2, 'S'
        | 1 -> (i % 13) + 2, 'C'
        | 2 -> (i % 13) + 2, 'H'
        | _ -> (i % 13) + 2, 'D')

let shuffle (rnd: Random) deck = 
    let rec picker deck rem =
        if Array.isEmpty rem then deck
        else
            let deck = rem.[rnd.Next(0, rem.Length)]::deck
            let rem = Array.except (Array.ofList deck) rem
            picker deck rem
    picker [] deck

type Game = {
    deck: (int * char) list
    discards: (int * char) list
    players: Player []
}
and Player = {
    hand: (int * char) list
    bet: int
    cash: int
}

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
        { model with players = newPlayers }, Cmd.none
    | _ -> 
        model, Cmd.none
