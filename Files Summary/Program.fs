open System.IO

let OUTPUT_FILE = "summary.txt"

let get_file_extension (file:string) =
    let split_file = file.Split('.')
    split_file.[split_file.Length - 1].ToLower()

let rec get_all_files path = seq {
    yield! Directory.EnumerateFiles(path)
    for directory in Directory.EnumerateDirectories(path) do yield! (get_all_files directory)
}

let get_all_files_filtered extensions = get_all_files >> Seq.filter (fun file -> List.exists (fun ext -> ext = (get_file_extension file)) extensions)

let create_header file = (file:string) + "\r\n" + (String.replicate (file.Length * 2) "-") + "\r\n"
let write_to_file (writer:StreamWriter) (data:string) = writer.Write(data)
let read_file (file:string) = File.ReadAllText(file)

[<EntryPoint>]
let main argv = 
    let writer = new StreamWriter(OUTPUT_FILE)

    get_all_files_filtered (argv |> List.ofArray) (Directory.GetCurrentDirectory())
    |> Seq.iter (fun file -> ((create_header file) + (read_file file) + "\r\n") |> write_to_file writer)

    writer.Close()
    0 // return an integer exit code
