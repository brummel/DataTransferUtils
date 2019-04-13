# DataTransferUtils Overview
DataTransferUtils is a command line utility to simplify moving data in and out of SQL Server databases.
It can be used as a fast, less cumbersome alternative to the SQL Server bcp.exe command line tool.

**Example**
```
C:\Utils> DataTransferUtils.exe TransferDataToFile -q "SELECT * FROM Audit.dbo.Logs" -f "AuditLogs.csv" -s ";"
```

The utility is built using the [CommandLineParser](https://github.com/commandlineparser/commandline) library.

To see a list of all available verbs, run the utility on the command line.
```
C:\Utils> DataTransferUtils.exe
```
