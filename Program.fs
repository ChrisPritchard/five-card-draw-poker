open System
open Elmish
open Terminal
open Model
open Cards
open Update
open View

[<EntryPoint>]
let main _ =

    printfn "Five-Card-Draw Poker!"
    printfn "====================="
    printfn ""
    let numPlayers = readInt 2 5 "Enter number of players (default 5): "
    printfn ""
    let startingCash = readInt 1 1000 "Enter cash per player (default 1000): "

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
            rng = random
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