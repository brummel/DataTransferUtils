# DataTransferUtils Overview
DataTransferUtils is a command line utility to simplify moving data in and out of SQL Server databases.
It was originally built as an fast, less cumbersome alternative to SQL Server bcp.exe.

The utility is built using the [CommandLineParser](https://github.com/commandlineparser/commandline) library.

To see a list of all available verbs, run the utility on the command line.
```
C:\Utils> DataTransferUtils.exe
```

## Examples
Use the TransferDataToFile verb to transfer the output of SQL query or stored procedure to a delimited file.
```
C:\Utils> DataTransferUtils.exe TransferDataToFile -q "SELECT * FROM Audit.dbo.Logs" -f "AuditLogs.csv"
```

## Config
Specify the defualt connection string and file output folder in  DataTransferUitls.exe.config file.
```xml
<appSettings>
  <add key="DefaultConnectionString" value="Server=.; Database=Extracts; Trusted_Connection=True;"/>
  <add key="DefaultRootDirectory" value="C:\Temp\"/>
</appSettings>
