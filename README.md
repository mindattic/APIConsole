# APIConsole

Console app that bulk-validates a CSV of US mailing addresses against the USPS address verification API, 100 requests at a time, and writes the standardized addresses to a results CSV.

![C#](https://img.shields.io/badge/C%23-console-512BD4) ![.NET](https://img.shields.io/badge/.NET-6.0-512BD4) ![Platform](https://img.shields.io/badge/platform-Windows-0078D6) ![Status](https://img.shields.io/badge/status-prototype-orange)

```text
test1.csv                     USPS address verification API              results.csv
+------------------------+    +------------------------------+    +------------------------+
| STE 100,1 MAIN ST,...  | -> | GET ...Verify&XML=<encoded>  | -> | STE 100,1 MAIN ST,...  |
| ...one row per address |    | 100 concurrent, 1 ms spacing |    | standardized, ZIP+4    |
+------------------------+    +------------------------------+    +------------------------+
```

A private prototype from February 2023, the headless sibling of [USPSAddressValidator](https://github.com/mindattic/USPSAddressValidator). There is no hosted build; run it from source.

## Why

- Clean a mailing list in one unattended run instead of pasting addresses into a web form one by one.
- Get USPS-standardized street, city, state, ZIP5 and ZIP+4 back in the same column order you sent.
- Keep the run fast with 100 concurrent requests, while a per-request delay keeps it polite.
- See progress every 1,000 requests and a friendly total time at the end.

## Features

- Reads a header-less CSV with six columns per row: Address1, Address2, City, State, Zip5, Zip4.
- Builds one `AddressValidateRequest` XML document per row (Revision 1), URL-encodes it as ISO-8859-1 and sends it as a GET query string.
- Throttles with a `SemaphoreSlim` (100 in flight) and a 1 ms delay after each request.
- Collects every successful HTTP response, then deserializes each into a typed `AddressValidateResponse` model (address lines, city abbreviation, ZIP5, ZIP4, delivery point, carrier route, DPV confirmation and footnotes, business, vacant flags).
- Writes Address1, Address2, City, State, Zip5, Zip4 per validated address to `results.csv` and opens it in Notepad.
- Prints the total as text such as "N requests completed in 1 Minute, 12 Seconds, 30 Milliseconds".

## Quick start

Prerequisites: Windows, the .NET 6 SDK, and your own USPS Web Tools user ID.

```powershell
git clone https://github.com/mindattic/APIConsole.git
cd APIConsole
```

1. Open `Program.cs` and set the configuration constants at the top of the class (see Configuration below). The API user ID belongs to whoever runs the tool; supply your own.
2. Put an input file named `test1.csv` in the folder you run from (the project folder when you use `dotnet run`).
3. Run it:

```powershell
dotnet run
```

You should see "Starting...", a progress line every 1,000 requests, "Done.", and then `results.csv` opens in Notepad.

## Configuration

All settings are compile-time constants at the top of `Program.cs`:

| Constant | Default | Meaning |
| --- | --- | --- |
| `THREAD_COUNT` | 100 | Maximum concurrent requests |
| `UPDATE_UI` | true | Print progress lines |
| `UPDATE_RATE` | 1000 | Print a progress line every N completed requests |
| `TASK_DELAY` | 1 | Milliseconds each worker waits after a request |
| `BASE_URL` | USPS Verify endpoint | Request prefix; the encoded XML is appended |
| `USER_ID` | set in source | Your USPS Web Tools user ID |
| `INPUT_FILE` | `test1.csv` | Input CSV, relative to the working folder |
| `OUTPUT_FILE` | `results.csv` | Output CSV, overwritten each run |

## Input format

No header row. One address per line, in USPS Web Tools field order, where Address1 is the apartment or suite and Address2 is the street line:

```text
STE 100,1 MAIN ST,SPRINGFIELD,IL,62701,
,500 OAK AVE,PORTLAND,OR,97201,
```

## How it works

```text
Main
 +- Load      CSVUtility -> DataTable
 +- Build     DataRow -> AddressValidateRequest (6 columns)
 +- Run       for each request:
 |              await SemaphoreSlim(100)
 |              Task.Run: ComposeXML -> UrlEncode(ISO-8859-1) -> HttpClient.GetAsync
 |                        keep 2xx responses, progress line every 1,000
 |              Task.WhenAll
 +- Complete  XmlSerializer -> AddressValidateResponse -> CSV line
              FileUtility.Save(results.csv) -> open in notepad.exe
```

## Project layout

| Path | Purpose |
| --- | --- |
| `APIConsole.sln`, `APIConsole.csproj` | Console app, `net6.0`, no NuGet dependencies |
| `Program.cs` | Configuration constants and the load, build, run, complete pipeline |
| `Models/AddressValidateRequest.cs` | XML request model |
| `Models/AddressValidateResponse.cs` | XML response model |
| `Models/Logger.cs` | Concurrent message, success and error log (not used by `Program.cs` yet) |
| `Utilities/CSVUtility.cs` | CSV to `DataTable` via `TextFieldParser` |
| `Utilities/FileUtility.cs` | Overwrite a file and open it in a text editor |
| `Utilities/PrintUtility.cs` | Console write helpers |
| `Extensions/` | `string.Repeat`, `List.ChunkBy`, `TimeSpan.ToFriendlyDisplay` |
| `commit.cmd` | Stage, commit with a timestamp message, and push |

## Limitations

- Configuration lives in source constants; there is no config file, environment variable or command-line argument yet.
- No sample input is committed; you provide `test1.csv`.
- Address values are inserted into the XML without escaping, so a value containing `&` or `<` produces an invalid request.
- A response that comes back with an `Error` element is parsed as null and stops the results pass with an exception.
- Non-2xx responses are dropped silently; there is no failure report.
- It targets the legacy USPS Web Tools XML Verify API. Check that the endpoint is still available to your account before relying on it.
- No tests.

## Documentation

There are no separate docs; this README and the source are the reference. [USPSAddressValidator](https://github.com/mindattic/USPSAddressValidator) is the Windows Forms version of the same pipeline, and [ApiCaller](https://github.com/mindattic/ApiCaller) is the generic JSON load tester that uses the same throttled request loop.

## License

No license file. All rights reserved.

Part of [MindAttic](https://mindattic.com) — see more projects at [github.com/mindattic](https://github.com/mindattic). Related: [ApiCaller](https://github.com/mindattic/ApiCaller), [USPSAddressValidator](https://github.com/mindattic/USPSAddressValidator).
