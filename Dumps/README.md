# Dumps

## Import

```PowerShell
Get-ChildItem . -Recurse | where {$_.extension -eq ".reg"} | % {
    reg import "`"$($_.FullName)`""
}
```
