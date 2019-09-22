module Terminal

open System
open System.Threading

let clearTerminal () =
    Console.Clear ()
    Console.CursorLeft <- 0
    Console.CursorTop <- 0

let pause milliseconds = 
    let time = TimeSpan.FromMilliseconds milliseconds
    Thread.Sleep time

let printCard (rank: int, suit: char) =
    match rank with
    | 14 -> sprintf "A%c" suit
    | 13 -> sprintf "K%c" suit
    | 12 -> sprintf "Q%c" suit
    | 11 -> sprintf "J%c" suit
    | 10 -> sprintf "X%c" suit
    | _ -> sprintf "%i%c" rank suit

let readLine = Console.ReadLine
let readKey = Console.ReadKey

let rec readInt min defaultResult askMessage =
    printf "%s" askMessage
    let result = readLine ()
    if result = "" then defaultResult
    else
        let valid, parsed = Int32.TryParse result
        if valid && parsed >= min then parsed
        else
            printfn "invalid integer or number less than %i" min
            readInt min defaultResult askMessage