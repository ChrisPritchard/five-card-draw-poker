module Cards

open System

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

type Hand = 
    private 
    | HighCard of int list | Pair of int | TwoPair of int * int | ThreeOfAKind of int
    | Straight of int list | Flush of int list | FullHouse of int * int | FourOfAKind of int
    | StraightFlush of int list | Kicker of int

let private rankHand (hand: (int * char) list) =
    let grouped = hand |> List.groupBy fst
    let has n = 
        grouped |> List.filter (fun p -> snd p |> List.length = n) 
        |> List.length
    let get n = 
        grouped |> List.filter (fun p -> snd p |> List.length = n) 
        |> List.map fst |> List.sortByDescending id
    let single n = get n |> List.head
    let pair n = get n |> (fun lst -> (lst.[0],lst.[1]))
    let remainder = get 1 |> List.map Kicker

    if has 2 = 1 && has 3 = 0 then (Pair (single 2), remainder)
    elif has 2 = 2 then (TwoPair (pair 2), remainder)
    elif has 3 = 1 && has 2 = 0 then (ThreeOfAKind (single 3), remainder)
    elif has 3 = 1 && has 2 = 1 then (FullHouse (single 3, single 2), [])
    elif has 4 = 1 then (FourOfAKind (single 4), remainder)
    else
        let isStraight = 
            let ranked = hand |> List.map fst |> List.sort
            let diff = (List.last ranked) - (List.head ranked)
            diff = 4 || diff = 12
        let isFlush = hand |> List.map snd |> List.distinct |> List.length |> (=) 1

        let swapAce lst = 
            if List.contains 5 lst then 
                List.map (fun n -> if n = 14 then 1 else n) lst 
                else lst

        if isFlush && isStraight then (StraightFlush (get 1 |> swapAce), [])
        elif isFlush then (Flush (get 1), [])
        elif isStraight then (Straight (get 1 |> swapAce), [])
        else
            (HighCard (get 1), [])

let bestHand (hands: seq<(int * char) list>) =
    hands |> Seq.map (fun h -> h, rankHand h)
          |> Seq.sortByDescending snd
          |> Seq.head 
          |> fst