# DataTransferUtils Overview
DataTransferUtils is a command line utility to simplify moving data in and out of SQL Server databases.
It was originally built as an fast, less cumbersome alternative to SQL Server bcp.exe.

The utility is built using the [CommandLineParser](https://github.com/commandlineparser/commandline) library.

To see a list of all available verbs, run the utility on the command line.
```
C:\Utils> DataTransferUtils.exe
```

## Example Usage
Use the TransferDataToFile verb to transfer the output of SQL query or stored procedure to a delimited file. x
By default, column headers will be included and will be inferred by the column names in the result set.
```
C:\Utils> DataTransferUtils.exe TransferDataToFile -q "SELECT * FROM Audit.dbo.Logs" -f "AuditLogs.csv"
```
**Note:** Database objects must be fully qualified in the query i.e. [Database Name].[Schema].[Object Name]

## Config
The connection string and file output folders are specified in the appSettings section of the DataTransferUitls.exe.config file.
```xml
<appSettings>
  <add key="DefaultConnectionString" value="Server=.; Database=Extracts; Trusted_Connection=True;"/>
  <add key="DefaultRootDirectory" value="C:\Utils"/>
</appSettings>
