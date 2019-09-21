module View

open Model
open Terminal

let renderHands model = 
    for i = model.players.Length downto 1 do
        let hand =
            if i = 1 then model.players.[0].hand |> Seq.map printCard |> String.concat " "
            else Array.create model.players.[i-1].hand.Length "##" |> String.concat " "
        printfn "Player %i: %s" (i - 1) hand

let renderDealing model dispatch =
    renderHands model
    pause 500.
    dispatch Deal

let renderDiscards model dispatch =
    if model.currentPlayerIndex = 0 then
        printfn "Your cards:"
        let hand = model.players.[0].hand |> Seq.map printCard |> String.concat " "
        printfn "%s" hand
        printfn ""
        // TODO select for discard
        printfn "Press enter to continue"
        readLine () |> ignore
        dispatch (Discard [])
    else
        // TODO AI
        printfn "Player %i discards %i cards" model.currentPlayerIndex 0
        dispatch (Discard [])

let renderBetting model dispatch = ()
let renderReveal winner model dispatch = ()
let renderGameOver model dispatch = ()

let view model dispatch =
    clearTerminal ()
    match model.state with
    | Dealing ->
        renderDealing model dispatch
    | Discards ->
        renderDiscards model dispatch
    | Betting ->
        renderBetting model dispatch
    | Reveal winner -> 
        renderReveal winner model dispatch
    | GameOver ->
        renderGameOver model dispatch