# Filtering SymSrv Symbol Server Proxy
A small SymSrv symbol server  proxy to download just the symbols you want written in .NET Core. From https://blogs.msdn.microsoft.com/ricom/2017/02/03/symbol-filter-redux/

## Why?

Analyzing [ETW traces][etwtraces] or crash dumps often requires downloading symbols. On Windows, the infrastructure to lookup and download symbols is provided via [SymSrv][symsrv].
Unfortunately, usually most of the symbols in a trace aren't indexed, and / or aren't relevant to the current investigation, wasting time and disk space.

SymbolFilterServer acts as a fast symbol server proxy, immediately returing `404 Not Found` for any symbol not in your provided allow list, and redirecting matching symbols to the real symbol store.

## Examples

To start, let's say you only want to download the symbols for `edgehtml.dll` from the official Microsoft Symbol Store, and attempts to load any other PDB should be blocked. Instead of having a `_NT_SYMBOL_PATH` that looks like this:

```PowerShell
PS> $Env:_NT_SYMBOL_PATH = "srv*C:\symbols*https://msdl.microsoft.com/download/symbols
```

Change the path to look like this:

```PowerShell
PS> $Env:_NT_SYMBOL_PATH = "srv*C:\symbols*http://localhost:8080/https://msdl.microsoft.com/download/symbols"
```

And run the proxy like this:

```PowerShell
PS> SymbolFilterServer.exe --port 8080 "edgehtml.dll"
```

If you have a list of symbols you'd like to reuse, instead of typing them all out on the command line, use a response file like this:

```PowerShell
PS> @("edgehtml.pdb", "ntdll.pdb", "eview.pdb") | Out-File "my-symbols.txt"
PS> SymbolFilterServer.exe --port 8080 @my-symbols.txt
```


[etwtraces]: https://docs.microsoft.com/en-us/windows-hardware/test/wpt/index
[symsrv]: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681416(v=vs.85).aspx