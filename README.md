# Daffer

Functional wrapper for Dapper.

- Provides a set of F# friendly functions for Dapper
- Makes passing parameters less noisy using `["name" => value]`
- Converts `IEnumerable<'T>` to `'T list`
- Converts `Task<'T>` to `Async<'T>`

## Getting started

Install the NuGet package into your application.

### Package Manager

```shell
Install-Package Daffer
```

### .NET CLI

```shell
dotnet add package Daffer
```

## Usage

```fsharp
let user = query<User> connection "SELECT * FROM Users WHERE Id = @Id" ["Id" => 1]
```

## Definitions

```fsharp
type Parameter = string * obj

(=>)                          : string -> obj -> Parameter

execute                       : IDbConnection -> string -> Parameter list -> int
executeAsync                  : IDbConnection -> string -> Parameter list -> Async<int>
executeReader                 : IDbConnection -> string -> Parameter list -> IDataReader
executeReaderAsync            : IDbConnection -> string -> Parameter list -> Async<IDataReader>
executeScalar<'T>             : IDbConnection -> string -> Parameter list -> 'T
executeScalarAsync<'T>        : IDbConnection -> string -> Parameter list -> Async<'T>
query<'T>                     : IDbConnection -> string -> Parameter list -> 'T list
queryAsync<'T>                : IDbConnection -> string -> Parameter list -> Async<'T list>
queryFirst<'T>                : IDbConnection -> string -> Parameter list -> 'T
queryFirstAsync<'T>           : IDbConnection -> string -> Parameter list -> Async<'T>
queryFirstOrDefault<'T>       : IDbConnection -> string -> Parameter list -> 'T
queryFirstOrDefaultAsync<'T>  : IDbConnection -> string -> Parameter list -> Async<'T>
queryMultiple                 : IDbConnection -> string -> Parameter list -> SqlMapper.GridReader
queryMultipleAsync            : IDbConnection -> string -> Parameter list -> Async<SqlMapper.GridReader>
querySingle<'T>               : IDbConnection -> string -> Parameter list -> 'T
querySingleAsync<'T>          : IDbConnection -> string -> Parameter list -> Async<'T>
querySingleOrDefault<'T>      : IDbConnection -> string -> Parameter list -> 'T
querySingleOrDefaultAsync<'T> : IDbConnection -> string -> Parameter list -> Async<'T>
```
