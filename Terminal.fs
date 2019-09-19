module Terminal

open System

let printCard (rank: int, suit: char) =
    match rank with
    | 14 -> sprintf "A%c" suit
    | 13 -> sprintf "K%c" suit
    | 12 -> sprintf "Q%c" suit
    | 11 -> sprintf "J%c" suit
    | 10 -> sprintf "X%c" suit
    | _ -> sprintf "%i%c" rank suit

let readLine = Console.ReadLine

let rec getInteger min askMessage =
    printf "%s" askMessage
    let result = readLine ()
    let valid, parsed = Int32.TryParse result
    if valid && parsed >= min then parsed
    else
        printfn "invalid integer or number less than %i" min
        getInteger min askMessage