﻿open System
open Elmish
open Model
open Cards
open Update
open View

let rec getInteger min (askMessage: string) =
    Console.Write askMessage
    let result = Console.ReadLine ()
    let valid, parsed = Int32.TryParse result
    if valid && parsed >= min then parsed
    else
        printfn "invalid integer or number less than %i" min
        getInteger min askMessage

[<EntryPoint>]
let main _ =

    printfn "Five-Card-Draw Poker!"
    printfn "====================="
    printfn ""
    let numPlayers = getInteger 2 "Enter number of players: "
    printfn ""
    let startingCash = getInteger 1 "Enter cash per player: "

    let players = 
        Array.init numPlayers (fun _ ->
            {
                hand = []
                cash = startingCash
                currentBet = 0
            })

    let random = Random ()
    let init _ =
        {
            deck = shuffle random standardDeck
            discards = []
            players = players
            currentPlayerIndex = 0
            dealerIndex = 0
            state = Dealing
        }, Cmd.none

    Program.mkProgram init update view
    |> Program.run

    0