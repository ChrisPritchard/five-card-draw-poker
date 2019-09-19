module View

open Model
open Terminal

let renderDealing model dispatch =
    printfn "Dealing player %i..." model.currentPlayerIndex
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