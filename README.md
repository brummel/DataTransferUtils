# DataTransferUtils Overview
DataTransferUtils is a command line utility to simplify moving data in and out of SQL Server databases.
It was originally built as an fast, less cumbersome alternative to SQL Server bcp.exe.

The utility is built using the [CommandLineParser](https://github.com/commandlineparser/commandline) library.

To see a list of all available verbs, run the utility on the command line.
```
C:\Utils> DataTransferUtils.exe
```

## Usage
```
C:\Utils> DataTransferUtils.exe TransferDataToFile -q "SELECT * FROM Audit.dbo.Logs" -f "AuditLogs.csv"
```

## Config
```xml
<AppSettings>
  <Config/>
</AppSettings>
