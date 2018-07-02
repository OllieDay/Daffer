namespace Daffer

    [<AutoOpen>]
    module Core =

        open Dapper
        open System.Data

        type Parameter = string * obj

        let (=>) (name : string) (value : obj) =
            (name, value)

        let execute (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.Execute (sql, dict parameters)

        let executeAsync (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.ExecuteAsync (sql, dict parameters) |> Async.AwaitTask
            }

        let executeReader (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.ExecuteReader (sql, dict parameters)

        let executeReaderAsync (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.ExecuteReaderAsync (sql, dict parameters) |> Async.AwaitTask
            }

        let executeScalar<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.ExecuteScalar<'T> (sql, dict parameters)

        let executeScalarAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.ExecuteScalarAsync<'T> (sql, dict parameters) |> Async.AwaitTask
            }

        let query<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.Query<'T> (sql, dict parameters) |> List.ofSeq

        let queryAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                let! result = connection.QueryAsync<'T> (sql, dict parameters) |> Async.AwaitTask
                return result |> List.ofSeq
            }

        let queryFirst<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.QueryFirst<'T> (sql, dict parameters)

        let queryFirstAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.QueryFirstAsync<'T> (sql, dict parameters) |> Async.AwaitTask
            }

        let queryFirstOrDefault<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.QueryFirstOrDefault<'T> (sql, dict parameters)

        let queryFirstOrDefaultAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.QueryFirstOrDefaultAsync<'T> (sql, dict parameters) |> Async.AwaitTask
            }

        let queryMultiple (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.QueryMultiple (sql, dict parameters)

        let queryMultipleAsync (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.QueryMultipleAsync (sql, dict parameters) |> Async.AwaitTask
            }

        let querySingle<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.QuerySingle<'T> (sql, dict parameters)

        let querySingleAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.QuerySingleAsync<'T> (sql, dict parameters) |> Async.AwaitTask
            }

        let querySingleOrDefault<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            connection.QuerySingleOrDefault<'T> (sql, dict parameters)

        let querySingleOrDefaultAsync<'T> (connection : IDbConnection) sql (parameters : Parameter list) =
            async {
                return! connection.QuerySingleOrDefaultAsync<'T> (sql, dict parameters) |> Async.AwaitTask
            }
