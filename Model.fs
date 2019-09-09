
open System

let printcard (rank: int, suit: char) =
    match rank with
    | 14 -> sprintf "A%c" suit
    | 13 -> sprintf "K%c" suit
    | 12 -> sprintf "Q%c" suit
    | 11 -> sprintf "J%c" suit
    | 10 -> sprintf "X%c" suit
    | _ -> sprintf "%i%c" rank suit

let randomdeck (rnd: Random) = 
    let start = [|0..51|] |> Array.map (fun i -> 
        match i % 4 with
        | 0 -> (i % 14) + 1, 'S'
        | 1 -> (i % 14) + 1, 'C'
        | 2 -> (i % 14) + 1, 'H'
        | _ -> (i % 14) + 1, 'D')
    let rec picker deck rem =
        if Array.isEmpty rem then deck
        else
            let deck = rem.[rnd.Next(0, rem.Length)]::deck
            let rem = Array.except (Array.ofList deck) rem
            picker deck rem
    picker [] start

let random = Random ()
let test = randomdeck random
let text = test |> List.map printcard |> String.concat " "
Console.WriteLine text