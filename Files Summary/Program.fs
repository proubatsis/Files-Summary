open System.IO

let OUTPUT_FILE = "summary.txt"
let NEW_LINE = "\r\n"
let HEADER_SEPERATOR = "-"

let get_file_extension (file:string) =
    let split_file = file.Split('.')
    split_file.[split_file.Length - 1].ToLower()

(* Yield all the file names in a given path and all of its subdirectories. *)
let rec get_all_files path = seq {
    yield! Directory.EnumerateFiles(path)
    for directory in Directory.EnumerateDirectories(path) do yield! (get_all_files directory)
}

(* Filter get_all_files to yield only files with the specified extension. *)
let get_all_files_filtered extensions = get_all_files >> Seq.filter (fun file -> List.exists (fun ext -> ext = (get_file_extension file)) extensions)

(* Create a header to indicate which file is being displayed in OUTPUT_FILE. *)
let create_header file = (file:string) + NEW_LINE + (String.replicate (file.Length * 2) HEADER_SEPERATOR) + NEW_LINE

let write_to_file (writer:StreamWriter) (data:string) = writer.Write(data)
let read_file (file:string) = File.ReadAllText(file)

[<EntryPoint>]
let main argv = 
    let writer = new StreamWriter(OUTPUT_FILE)

    get_all_files_filtered (argv |> List.ofArray) (Directory.GetCurrentDirectory())
    |> Seq.iter (fun file -> ((create_header file) + (read_file file) + NEW_LINE) |> write_to_file writer)

    writer.Close()
    0 // return an integer exit code
