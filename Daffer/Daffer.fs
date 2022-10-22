namespace Daffer

    open Dapper
    open System

    type private OptionHandler<'T> () =
        inherit SqlMapper.TypeHandler<'T option> ()

        let isDbNull value =
            isNull value || value = box DBNull.Value

        override __.SetValue (param, value) =
            param.Value <-
                match value with
                | Some x -> box x
                | None -> null

        override __.Parse value =
            match isDbNull value with
            | true -> None
            | false -> Some (value :?> 'T)

    type Parameter = string * obj

    module Builder =

        open System.Collections.Generic
        open System.Data
        open System.Threading.Tasks

        type Build = {
            Transaction : IDbTransaction option
            CommandTimeout : int option
            CommandType : CommandType option
            Buffered : bool option
        }

        let create () =
            { Transaction = None
              CommandTimeout = None
              CommandType = None
              Buffered = None }

        let addTransaction transaction build =
            { build with Transaction = Some transaction }

        let addCommandTimeout commandTimeout build =
            { build with CommandTimeout = Some commandTimeout }

        let addCommandType commandType build =
            { build with CommandType = Some commandType }

        let addBuffered buffered build =
            { build with Buffered = Some buffered }

        type private Arguments = string * IDictionary<string,obj> * IDbTransaction * Nullable<int> * Nullable<CommandType>

        let private toArguments (sql : string) (parameters : Parameter list) build =
            let transaction = Option.defaultValue null build.Transaction
            let commandTimeout = Option.toNullable build.CommandTimeout
            let commandType = Option.toNullable build.CommandType
            (sql, dict parameters, transaction, commandTimeout, commandType)

        let private toQueryArguments sql parameters build =
            let (sql, parameters, transaction, commandTimeout, commandType) = toArguments sql parameters build
            let buffered = Option.defaultValue true build.Buffered
            (sql, parameters, transaction, buffered, commandTimeout, commandType)

        let private run<'T> sql parameters (runner : Arguments -> 'T) build =
            toArguments sql parameters build |> runner

        let private runAsync<'T> sql parameters (runner : Arguments -> Task<'T>) build =
            async {
                return! toArguments sql parameters build
                    |> runner
                    |> Async.AwaitTask
            }

        let execute (connection : IDbConnection) sql parameters =
            run sql parameters connection.Execute 

        let executeAsync (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.ExecuteAsync 

        let executeReader (connection : IDbConnection) sql parameters =
            run sql parameters connection.ExecuteReader

        let executeReaderAsync (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.ExecuteReaderAsync

        let executeScalar<'T> (connection : IDbConnection) sql parameters =
            run sql parameters connection.ExecuteScalar<'T>

        let executeScalarAsync<'T> (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.ExecuteScalarAsync<'T>

        let query<'T> (connection : IDbConnection) sql parameters build =
            toQueryArguments sql parameters build
                |> connection.Query<'T>
                |> List.ofSeq

        let queryAsync<'T> (connection : IDbConnection) sql parameters build =
            async {
                let! result = runAsync sql parameters connection.QueryAsync<'T> build
                return List.ofSeq result
            }

        let queryFirst<'T> (connection : IDbConnection) sql parameters =
            run sql parameters connection.QueryFirst<'T>

        let queryFirstAsync<'T> (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.QueryFirstAsync<'T>

        let queryFirstOrDefault<'T> (connection : IDbConnection) sql parameters =
            run sql parameters connection.QueryFirstOrDefault<'T>

        let queryFirstOrDefaultAsync<'T> (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.QueryFirstOrDefaultAsync<'T>

        let queryMultiple (connection : IDbConnection) sql parameters =
            run sql parameters connection.QueryMultiple

        let queryMultipleAsync (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.QueryMultipleAsync

        let querySingle<'T> (connection : IDbConnection) sql parameters =
            run sql parameters connection.QuerySingle<'T>

        let querySingleAsync<'T> (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.QuerySingleAsync<'T>

        let querySingleOrDefault<'T> (connection : IDbConnection) sql parameters =
            run sql parameters connection.QuerySingleOrDefault<'T>

        let querySingleOrDefaultAsync<'T> (connection : IDbConnection) sql parameters =
            runAsync sql parameters connection.QuerySingleOrDefaultAsync<'T>

        let private firstMaybe = function
            | [] -> None
            | x :: _ -> Some x

        let queryFirstMaybe<'T> connection sql parameters =
            query<'T> connection sql parameters >> firstMaybe

        let queryFirstMaybeAsync<'T> connection sql parameters build =
            async {
                let! result = queryAsync<'T> connection sql parameters build
                return result |> firstMaybe
            }

        let private singleMaybe = function
            | [] -> None
            | [x] -> Some x
            | _ -> InvalidOperationException "Sequence contains more than one element" |> raise

        let querySingleMaybe<'T> connection sql parameters =
            query<'T> connection sql parameters >> singleMaybe

        let querySingleMaybeAsync<'T> (connection : IDbConnection) sql parameters build =
            async {
                let! result = queryAsync<'T> connection sql parameters build
                return result |> singleMaybe
            }

    [<AutoOpen>]
    module Core =

        open System.Data

        let (=>) (name : string) (value : obj) =
            (name, value)

        let private addOptionHandler<'T> () =
            SqlMapper.AddTypeHandler (OptionHandler<'T> ())

        let addOptionHandlers () =
            addOptionHandler<bool> ()
            addOptionHandler<byte> ()
            addOptionHandler<sbyte> ()
            addOptionHandler<char> ()
            addOptionHandler<single> ()
            addOptionHandler<double> ()
            addOptionHandler<decimal> ()
            addOptionHandler<int8> ()
            addOptionHandler<uint8> ()
            addOptionHandler<int16> ()
            addOptionHandler<uint16> ()
            addOptionHandler<int32> ()
            addOptionHandler<uint32> ()
            addOptionHandler<int64> ()
            addOptionHandler<uint64> ()
            addOptionHandler<string> ()
            addOptionHandler<Guid> ()
            addOptionHandler<DateTime> ()

        let execute connection sql parameters =
            Builder.create () |> Builder.execute connection sql parameters

        let executeAsync connection sql parameters =
            async {
                return! Builder.create () |> Builder.executeAsync connection sql parameters
            }

        let executeReader connection sql parameters =
            Builder.create () |> Builder.executeReader connection sql parameters

        let executeReaderAsync connection sql parameters =
            async {
                return! Builder.create () |> Builder.executeReaderAsync connection sql parameters
            }

        let executeScalar<'T> connection sql parameters =
            Builder.create () |> Builder.executeScalar<'T> connection sql parameters

        let executeScalarAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.executeScalarAsync<'T> connection sql parameters
            }

        let query<'T> connection sql parameters =
            Builder.create () |> Builder.query<'T> connection sql parameters

        let queryAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.queryAsync<'T> connection sql parameters
            }

        let queryFirst<'T> connection sql parameters =
            Builder.create () |> Builder.queryFirst<'T> connection sql parameters

        let queryFirstAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.queryFirstAsync<'T> connection sql parameters
            }

        let queryFirstOrDefault<'T> connection sql parameters =
            Builder.create () |> Builder.queryFirstOrDefault<'T> connection sql parameters

        let queryFirstOrDefaultAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.queryFirstOrDefaultAsync<'T> connection sql parameters
            }

        let queryMultiple connection sql parameters =
            Builder.create () |> Builder.queryMultiple connection sql parameters

        let queryMultipleAsync connection sql parameters =
            async {
                return! Builder.create () |> Builder.queryMultipleAsync connection sql parameters
            }

        let querySingle<'T> connection sql parameters =
            Builder.create () |> Builder.querySingle<'T> connection sql parameters

        let querySingleAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.querySingleAsync<'T> connection sql parameters
            }

        let querySingleOrDefault<'T> connection sql parameters =
            Builder.create () |> Builder.querySingleOrDefault<'T> connection sql parameters

        let querySingleOrDefaultAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.querySingleOrDefaultAsync<'T> connection sql parameters
            }

        let queryFirstMaybe<'T> connection sql parameters =
            Builder.create () |> Builder.queryFirstMaybe<'T> connection sql parameters

        let queryFirstMaybeAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.queryFirstMaybeAsync<'T> connection sql parameters
            }

        let querySingleMaybe<'T> connection sql parameters =
            Builder.create () |> Builder.querySingleMaybe<'T> connection sql parameters

        let querySingleMaybeAsync<'T> connection sql parameters =
            async {
                return! Builder.create () |> Builder.querySingleMaybeAsync<'T> connection sql parameters
            }
