module View

open Model
open Terminal

let handString = Seq.rev >> Seq.map printCard >> String.concat " "

let renderHands model hideOthers = 
    for i = model.players.Length downto 1 do
        let hand =
            if not hideOthers || i = 1 then handString model.players.[i-1].hand
            else Array.create model.players.[i-1].hand.Length "##" |> String.concat " "
        printfn "Player %i: %s" (i - 1) hand

let renderDealing model dispatch =
    printfn "Current player cash amounts:"
    for i = model.players.Length downto 1 do
        printfn "Player %i: $%i" (i - 1) model.players.[i-1].cash

    printfn ""

    renderHands model true
    pause 250.
    dispatch Deal

let renderDiscards (model: Game) dispatch =
    printfn "Your cards: %s" (handString model.currentPlayer.hand)
    printfn ""
    // TODO select for discard
    printfn "Press enter to continue"
    readLine () |> ignore
    dispatch (Discard [])

let aiDiscards model dispatch =
    // TODO AI
    printfn "Player %i discards %i cards" model.currentPlayerIndex 0
    dispatch (Discard [])

let rec renderBetting (model: Game) dispatch =

    let minBet = 
        let required = model.maxBet - model.currentPlayer.currentBet
        min required model.currentPlayer.cash

    printfn "Your cards: %s" (handString model.currentPlayer.hand)
    printfn "Your current bet: $%i" model.currentPlayer.currentBet
    printfn "Minimum bet: $%i" minBet
    printfn "Current pool: $%i" model.currentPool
    printfn ""

    // TODO ask for bet, raise, meet or fold
    let possible = min (minBet + 100) model.currentPlayer.cash

    printfn "Enter Y to raise or bet $%i" possible
    printfn "Or enter to meet minimum"
    
    let choice = (readLine ()).ToLowerInvariant ()
    if choice = "" then dispatch (Bet minBet)
    elif choice = "y" then dispatch (Bet possible)
    else
        printfn "invalid entry, please try again"
        printfn ""
        renderBetting model dispatch

let aiBetting (model: Game) dispatch =
    // TODO AI
    let bet = 
        let minBet = model.maxBet - model.currentPlayer.currentBet
        if minBet = 0 then min 100 model.currentPlayer.cash else minBet
    
    printfn "Player %i meets the current bet $%i" model.currentPlayerIndex bet
    dispatch (Bet bet)

let renderReveal winner model dispatch =
    renderHands model false

    if winner = 0 then
        printfn "You won!"
    else
        printfn "Player %i won with hand: %s" winner (handString model.players.[winner].hand)
    printfn "Your cards: %s" (handString model.players.[0].hand)
    printfn ""
    printfn "Press enter to payout and end the round."
    readLine () |> ignore
    dispatch PayOut

let renderGameOver model dispatch =
    printfn "Game is over after %i rounds. Winner was player %i" model.rounds (model.finalWinner ())
    printfn ""
    printfn "Press any key to exit."
    readKey true |> ignore
    exit 0

let view model dispatch =
    clearTerminal ()
    match model.state with
    | Dealing ->
        renderDealing model dispatch
    | Discards ->
        if model.currentPlayerIndex = 0 then
            renderDiscards model dispatch
        else
            aiDiscards model dispatch
    | Betting ->
        if model.currentPlayerIndex = 0 then
            renderBetting model dispatch
        else
            aiBetting model dispatch
    | Reveal winner -> 
        renderReveal winner model dispatch
    | GameOver ->
        renderGameOver model dispatch