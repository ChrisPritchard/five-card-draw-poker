module View

open Model

let renderDealing model dispatch = ()
let renderDiscards model dispatch = ()
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