# CLI Documentation

## Entity Framework Core 

[See Entity Framework Documentation](https://learn.microsoft.com/en-us/ef/core/)


### Install Package Manager Console

```
Microsoft.EntityFrameworkCore -Version 8.0.0
Microsoft.EntityFrameworkCore.Tools -Version 8.0.0
Microsoft.EntityFrameworkCore.Design
```


#### PostgreSql (Optional)

```
NuGet\Install-Package Npgsql.EntityFrameworkCore.PostgreSQL -Version 8.0.0
```


#### Sql Server (Optional)

```
NuGet\Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.0
```


### Migrating Database

```
Add-Migration InitialCreate
Update-Database
```

### Remove Migration
```
Remove-Migration
```