# Daffer

Functional wrapper for Dapper.

- Provides a set of F# friendly functions for Dapper
- Makes passing parameters less noisy using `["name" => value]`
- Converts `IEnumerable<'T>` to `'T list`
- Converts `Task<'T>` to `Async<'T>`
- Provides alternatives to `QueryFirstOrDefault<'T>` and `QuerySingleOrDefault<'T>` that returns `'T option`
- Handles conversion of primitives to and from `'T option`

## Getting started

Install the NuGet package into your application.

### Package manager

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

### Maybe functions

Functions returning `null` have a counterpart that returns `'T option` instead.

| `null`                          |         `'T option`         |
| --------------------------------|---------------------------- |
| `queryFirstOrDefault<'T>`       | `queryFirstMaybe<'T>`       |
| `queryFirstOrDefaultAsync<'T>`  | `queryFirstMaybeAsync<'T>`  |
| `queryFirstOrDefaultTask<'T>`   | `queryFirstMaybeTask<'T>`   |
| `querySingleOrDefault<'T>`      | `querySingleMaybe<'T>`      |
| `querySingleOrDefaultAsync<'T>` | `querySingleMaybeAsync<'T>` |
| `querySingleOrDefaultTask<'T>`  | `querySingleMaybeTask<'T>`  |

### Converting primitives to and from `'T option`

The following primitive types can be automatically converted to and from `'T option` by calling `addOptionHandlers`
on application initialization.

- `bool`
- `byte`
- `sbyte`
- `char`
- `single`
- `double`
- `decimal`
- `int8`
- `uint8`
- `int16`
- `uint16`
- `int32`
- `uint32`
- `int64`
- `uint64`
- `string`
- `Guid`
- `DateTime`

### Optional arguments

The `Builder` module can be used for calling the underlying Dapper methods with optional arguments such as:

- `transaction`
- `commandTimeout`
- `commandType`
- `buffered`

```fsharp
let user =
    Builder.create ()
        |> Builder.addTransaction transaction
        |> Builder.addCommandTimeout 1000
        |> Builder.addCommandType CommandType.StoredProcedure
        |> Builder.addBuffered false
        |> Builder.query<User> connection "SELECT * FROM Users WHERE Id = @Id" ["id" => 1]
```

## Definitions

```fsharp
type Parameter = string * obj

(=>)                          : string -> obj -> Parameter

addOptionHandlers             : unit -> unit

execute                       : IDbConnection -> string -> Parameter list -> int
executeAsync                  : IDbConnection -> string -> Parameter list -> Async<int>
executeTask                   : IDbConnection -> string -> Parameter list -> Task<int>
executeReader                 : IDbConnection -> string -> Parameter list -> IDataReader
executeReaderAsync            : IDbConnection -> string -> Parameter list -> Async<IDataReader>
executeReaderTask             : IDbConnection -> string -> Parameter list -> Task<IDataReader>
executeScalar<'T>             : IDbConnection -> string -> Parameter list -> 'T
executeScalarAsync<'T>        : IDbConnection -> string -> Parameter list -> Async<'T>
executeScalarTask<'T>         : IDbConnection -> string -> Parameter list -> Task<'T>
query<'T>                     : IDbConnection -> string -> Parameter list -> 'T list
queryAsync<'T>                : IDbConnection -> string -> Parameter list -> Async<'T list>
queryTask<'T>                 : IDbConnection -> string -> Parameter list -> Task<'T list>
queryFirst<'T>                : IDbConnection -> string -> Parameter list -> 'T
queryFirstAsync<'T>           : IDbConnection -> string -> Parameter list -> Async<'T>
queryFirstTask<'T>            : IDbConnection -> string -> Parameter list -> Task<'T>
queryFirstOrDefault<'T>       : IDbConnection -> string -> Parameter list -> 'T
queryFirstOrDefaultAsync<'T>  : IDbConnection -> string -> Parameter list -> Async<'T>
queryFirstOrDefaultTask<'T>   : IDbConnection -> string -> Parameter list -> Task<'T>
queryMultiple                 : IDbConnection -> string -> Parameter list -> SqlMapper.GridReader
queryMultipleAsync            : IDbConnection -> string -> Parameter list -> Async<SqlMapper.GridReader>
queryMultipleTask             : IDbConnection -> string -> Parameter list -> Task<SqlMapper.GridReader>
querySingle<'T>               : IDbConnection -> string -> Parameter list -> 'T
querySingleAsync<'T>          : IDbConnection -> string -> Parameter list -> Async<'T>
querySingleTask<'T>           : IDbConnection -> string -> Parameter list -> Task<'T>
querySingleOrDefault<'T>      : IDbConnection -> string -> Parameter list -> 'T
querySingleOrDefaultAsync<'T> : IDbConnection -> string -> Parameter list -> Async<'T>
querySingleOrDefaultTask<'T>  : IDbConnection -> string -> Parameter list -> Task<'T>
queryFirstMaybe<'T>           : IDbConnection -> string -> Parameter list -> 'T option
queryFirstMaybeAsync<'T>      : IDbConnection -> string -> Parameter list -> Async<'T option>
queryFirstMaybeTask<'T>       : IDbConnection -> string -> Parameter list -> Task<'T option>
querySingleMaybe<'T>          : IDbConnection -> string -> Parameter list -> 'T option
querySingleMaybeAsync<'T>     : IDbConnection -> string -> Parameter list -> Async<'T option>
querySingleMaybeTask<'T>      : IDbConnection -> string -> Parameter list -> Task<'T option>
```

## Builder definitions

```fsharp
type Build = {
    Transaction : IDbTransaction option
    CommandTimeout : int option
    CommandType : CommandType option
    Buffered : bool option
}

create                        : unit -> Build

addTransaction                : IDbTransaction -> Build -> Build
addCommandTimeout             : int -> Build -> Build
addCommandType                : CommandType -> Build -> Build
addBuffered                   : bool -> Build -> Build

execute                       : IDbConnection -> string -> Parameter list -> Build -> int
executeAsync                  : IDbConnection -> string -> Parameter list -> Build -> Async<int>
executeTask                   : IDbConnection -> string -> Parameter list -> Build -> Task<i nt>
executeReader                 : IDbConnection -> string -> Parameter list -> Build -> IDataReader
executeReaderAsync            : IDbConnection -> string -> Parameter list -> Build -> Async<IDataReader>
executeReaderTask             : IDbConnection -> string -> Parameter list -> Build -> Task<IDataReader>
executeScalar<'T>             : IDbConnection -> string -> Parameter list -> Build -> 'T
executeScalarAsync<'T>        : IDbConnection -> string -> Parameter list -> Build -> Async<'T>
executeScalarTask<'T>         : IDbConnection -> string -> Parameter list -> Build -> Task<'T>
query<'T>                     : IDbConnection -> string -> Parameter list -> Build -> 'T list
queryAsync<'T>                : IDbConnection -> string -> Parameter list -> Build -> Async<'T list>
queryTask<'T>                 : IDbConnection -> string -> Parameter list -> Build -> Task<'T list>
queryFirst<'T>                : IDbConnection -> string -> Parameter list -> Build -> 'T
queryFirstAsync<'T>           : IDbConnection -> string -> Parameter list -> Build -> Async<'T>
queryFirstTask<'T>            : IDbConnection -> string -> Parameter list -> Build -> Task<'T>
queryFirstOrDefault<'T>       : IDbConnection -> string -> Parameter list -> Build -> 'T
queryFirstOrDefaultAsync<'T>  : IDbConnection -> string -> Parameter list -> Build -> Async<'T>
queryFirstOrDefaultTask<'T>   : IDbConnection -> string -> Parameter list -> Build -> Task<'T>
queryMultiple                 : IDbConnection -> string -> Parameter list -> Build -> SqlMapper.GridReader
queryMultipleAsync            : IDbConnection -> string -> Parameter list -> Build -> Async<SqlMapper.GridReader>
queryMultipleTask             : IDbConnection -> string -> Parameter list -> Build -> Task<SqlMapper.GridReader>
querySingle<'T>               : IDbConnection -> string -> Parameter list -> Build -> 'T
querySingleAsync<'T>          : IDbConnection -> string -> Parameter list -> Build -> Async<'T>
querySingleTask<'T>           : IDbConnection -> string -> Parameter list -> Build -> Task<'T>
querySingleOrDefault<'T>      : IDbConnection -> string -> Parameter list -> Build -> 'T
querySingleOrDefaultAsync<'T> : IDbConnection -> string -> Parameter list -> Build -> Async<'T>
querySingleOrDefaultTask<'T>  : IDbConnection -> string -> Parameter list -> Build -> Task<'T>
queryFirstMaybe<'T>           : IDbConnection -> string -> Parameter list -> Build -> 'T option
queryFirstMaybeAsync<'T>      : IDbConnection -> string -> Parameter list -> Build -> Async<'T option>
queryFirstMaybeTask<'T>       : IDbConnection -> string -> Parameter list -> Build -> Task<'T o ption>
querySingleMaybe<'T>          : IDbConnection -> string -> Parameter list -> Build -> 'T option
querySingleMaybeAsync<'T>     : IDbConnection -> string -> Parameter list -> Build -> Async<'T option>
querySingleMaybeTask<'T>      : IDbConnection -> string -> Parameter list -> Build -> Task<'T option>
```
