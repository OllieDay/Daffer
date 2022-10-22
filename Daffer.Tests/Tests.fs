module Tests

    open Daffer
    open FsUnit
    open Npgsql
    open Xunit

    let connectionString = "Server=localhost; Port=5432; Database=postgres; User Id=postgres; Password=password"
    let connection = new NpgsqlConnection (connectionString)

    [<Fact>]
    let ``execute with no rows affected returns -1`` () =
        execute connection "" []
            |> should equal -1

    [<Fact>]
    let ``execute with 1 row affected returns 1`` () =
        let sql = """
            create temp table x (
                id int
            );

            insert into x (id)
            values (1);

            drop table x;
        """
        execute connection sql []
            |> should equal 1

    [<Fact>]
    let ``executeAsync with no rows affected returns -1`` () =
        executeAsync connection "" []
            |> Async.RunSynchronously
            |> should equal -1

    [<Fact>]
    let ``executeAsync with 1 row affected returns 1`` () =
        let sql = """
            create temp table x (
                id int
            );

            insert into x (id)
            values (1);

            drop table x;
        """
        executeAsync connection sql []
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``executeReader returns values`` () =
        use reader = executeReader connection "select @value" ["value" => 1]
        reader.Read () |> ignore
        reader.[0]
            |> should equal 1

    [<Fact>]
    let ``executeReaderAsync returns values`` () =
        async {
            use! reader = executeReaderAsync connection "select @value" ["value" => 1]
            reader.Read () |> ignore
            reader.[0]
                |> should equal 1
        }

    [<Fact>]
    let ``executeScalar with no results returns 0`` () =
        executeScalar<int> connection "select null limit 0" []
            |> should equal 0

    [<Fact>]
    let ``executeScalar with 1 result returns value`` () =
        executeScalar<int> connection "select @value" ["value" => 1]
            |> should equal 1

    [<Fact>]
    let ``executeScalarAsync with no results returns 0`` () =
        executeScalarAsync<int> connection "select null limit 0" []
            |> Async.RunSynchronously
            |> should equal 0

    [<Fact>]
    let ``executeScalarAsync with 1 result returns value`` () =
        executeScalarAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``query with no results returns empty list`` () =
        query<int> connection "select null limit 0" []
            |> should equal List.empty<int>

    [<Fact>]
    let ``query with 2 rows returns values`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        query<int> connection sql ["a" => 1; "b" => 2]
            |> should equal [1; 2]

    [<Fact>]
    let ``queryAsync with no results returns empty list`` () =
        queryAsync<int> connection "select null limit 0" []
            |> Async.RunSynchronously
            |> should equal List.empty<int>

    [<Fact>]
    let ``queryAsync with 2 rows returns values`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        queryAsync<int> connection sql ["a" => 1; "b" => 2]
            |> Async.RunSynchronously
            |> should equal [1; 2]

    [<Fact>]
    let ``queryFirst with no results throws exception`` () =
        (fun () ->
            queryFirst<int> connection "select null limit 0" [] |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``queryFirst with 1 row returns value`` () =
        queryFirst<int> connection "select @value" ["value" => 1]
            |> should equal 1

    [<Fact>]
    let ``queryFirst with 2 rows returns first value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        queryFirst<int> connection sql ["a" => 1; "b" => 2]
            |> should equal 1

    [<Fact>]
    let ``queryFirstAsync with no results throws exception`` () =
        (fun () ->
            queryFirstAsync<int> connection "select null limit 0" []
                |> Async.RunSynchronously |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``queryFirstAsync with 1 row returns value`` () =
        queryFirstAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``queryFirstAsync with 2 rows returns first value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        queryFirstAsync<int> connection sql  ["a" => 1; "b" => 2]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``queryFirstOrDefault with no results returns default value`` () =
        queryFirstOrDefault<int> connection "select null limit 0" []
            |> should equal Unchecked.defaultof<int>

    [<Fact>]
    let ``queryFirstOrDefault with 1 row returns value`` () =
        queryFirstOrDefault<int> connection "select @value" ["value" => 1]
            |> should equal 1

    [<Fact>]
    let ``queryFirstOrDefault with 2 rows returns first value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        queryFirstOrDefault<int> connection sql ["a" => 1; "b" => 2]
            |> should equal 1

    [<Fact>]

    let ``queryFirstOrDefaultAsync with no results returns default value`` () =
        queryFirstOrDefaultAsync<int> connection "select null limit 0" []
            |> Async.RunSynchronously
            |> should equal Unchecked.defaultof<int>

    [<Fact>]
    let ``queryFirstOrDefaultAsync with 1 row returns value`` () =
        queryFirstOrDefaultAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``queryFirstOrDefaultAsync with 2 rows returns first value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        queryFirstOrDefaultAsync<int> connection sql ["a" => 1; "b" => 2]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``queryMultiple returns reader with multiple values`` () =
        let reader = queryMultiple connection "select @a; select @b" ["a" => 1; "b" => 2]
        reader.Read<int> ()
            |> Seq.exactlyOne
            |> should equal 1
        reader.Read<int> ()
            |> Seq.exactlyOne
            |> should equal 2

    [<Fact>]
    let ``queryMultipleAsync returns reader with multiple values`` () =
        async {
            let! reader = queryMultipleAsync connection "select @a; select @b" ["a" => 1; "b" => 2]
            reader.Read<int> ()
                |> Seq.exactlyOne
                |> should equal 1
            reader.Read<int> ()
                |> Seq.exactlyOne
                |> should equal 2
        }

    [<Fact>]
    let ``querySingle with no results throws exception`` () =
        (fun () ->
            querySingle<int> connection "select null limit 0" [] |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``querySingle with 1 row returns value`` () =
        querySingle<int> connection "select @value" ["value" => 1]
            |> should equal 1

    [<Fact>]
    let ``querySingle with 2 rows throws exception`` () =
        (fun () ->
            let sql = """
                select @a
                union
                select @b
            """
            querySingle<int> connection sql ["a" => 1; "b" => 2] |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``querySingleAsync with no results throws exception`` () =
        (fun () ->
            querySingleAsync<int> connection "select null limit 0" []
                |> Async.RunSynchronously |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``querySingleAsync with 1 row returns value`` () =
        querySingleAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal 1

    [<Fact>]
    let ``querySingleAsync with 2 rows throws exception`` () =
        (fun () ->
            let sql = """
                select @a
                union
                select @b
            """
            querySingleAsync<int> connection sql ["a" => 1; "b" => 2]
                |> Async.RunSynchronously |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``querySingleOrDefault with no results returns default value`` () =
        querySingleOrDefault<int> connection "select null limit 0" []
            |> should equal Unchecked.defaultof<int>

    [<Fact>]
    let ``querySingleOrDefault with 1 row returns value`` () =
        querySingleOrDefault<int> connection "select @value" ["value" => 1]
            |> should equal 1

    [<Fact>]
    let ``querySingleOrDefaultAsync with 2 rows throws exception`` () =
        (fun () ->
            let sql = """
                select @a
                union
                select @b
            """
            querySingleOrDefaultAsync<int> connection sql ["a" => 1; "b" => 2]
                |> Async.RunSynchronously |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``queryFirstMaybe with no results returns None`` () =
        queryFirstMaybe<int> connection "select null limit 0" []
            |> should equal None

    [<Fact>]
    let ``queryFirstMaybe with 1 row returns Some value`` () =
        queryFirstMaybe<int> connection "select @value" ["value" => 1]
            |> should equal (Some 1)

    [<Fact>]
    let ``queryFirstMaybe with 2 rows returns first Some value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        queryFirstMaybe<int> connection sql ["a" => 1; "b" => 2]
            |> should equal (Some 1)

    [<Fact>]
    let ``queryFirstMaybeAsync with no results returns None`` () =
        queryFirstMaybeAsync<int> connection "select null limit 0" []
            |> Async.RunSynchronously
            |> should equal None

    [<Fact>]
    let ``queryFirstMaybeAsync with 1 row returns Some value`` () =
        queryFirstMaybeAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal (Some 1)

    [<Fact>]
    let ``queryFirstMaybeAsync with 2 rows returns first Some value`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        queryFirstMaybeAsync<int> connection sql ["a" => 1; "b" => 2]
            |> Async.RunSynchronously
            |> should equal (Some 1)

    [<Fact>]
    let ``querySingleMaybe with no results returns None`` () =
        querySingleMaybe<int> connection "select null limit 0" []
            |> should equal None

    [<Fact>]
    let ``querySingleMaybe with 1 row returns Some value`` () =
        querySingleMaybe<int> connection "select @value" ["value" => 1]
            |> should equal (Some 1)

    [<Fact>]
    let ``querySingleMaybe with 2 rows throws exception`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        (fun () ->
            querySingleMaybe<int> connection sql ["a" => 1; "b" => 2] |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``querySingleMaybeAsync with no results returns None`` () =
        querySingleMaybeAsync<int> connection "select null limit 0" []
            |> Async.RunSynchronously
            |> should equal None

    [<Fact>]
    let ``querySingleMaybeAsync with 1 row returns Some value`` () =
        querySingleMaybeAsync<int> connection "select @value" ["value" => 1]
            |> Async.RunSynchronously
            |> should equal (Some 1)

    [<Fact>]
    let ``querySingleMaybeAsync with 2 rows throws exception`` () =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x
        """
        (fun () ->
            querySingleMaybeAsync<int> connection sql ["a" => 1; "b" => 2]
            |> Async.RunSynchronously
            |> ignore
        ) |> shouldFail

    [<Fact>]
    let ``converts None to null`` () =
        addOptionHandlers ()
        querySingle<string> connection "select @value" ["value" => None]
            |> should equal null

    [<Fact>]
    let ``converts Some "x" to 'x'`` () =
        addOptionHandlers ()
        querySingle<string> connection "select @value" ["value" => Some "x"]
            |> should equal "x"

    [<Fact>]
    let ``converts null to None`` () =
        addOptionHandlers ()
        querySingle<string option> connection "select null" []
            |> should equal None

    [<Fact>]
    let ``converts 'x' to Some "x"`` () =
        addOptionHandlers ()
        querySingle<string option> connection "select 'x'" []
            |> should equal (Some "x")

    [<Fact>]
    let ``committed transaction returns 1`` () =
        let sql = """
            create table if not exists x (
                id int
            );

            delete from x;
        """
        execute connection sql [] |> ignore
        connection.Open ()
        use transaction = connection.BeginTransaction ()
        let sql = """
            insert into x (id)
            values (1);
        """
        Builder.create ()
            |> Builder.addTransaction transaction
            |> Builder.execute connection sql []
            |> ignore
        transaction.Commit ()
        connection.Close ()
        let sql = """
            select count (1)
            from x;
        """
        executeScalar<int> connection sql []
            |> should equal 1

    [<Fact>]
    let ``rolled back transaction returns 0`` () =
        let sql = """
            create table if not exists x (
                id int
            );

            delete from x;
        """
        execute connection sql [] |> ignore
        connection.Open ()
        use transaction = connection.BeginTransaction ()
        let sql = """
            insert into x (id)
            values (1);
        """
        Builder.create ()
            |> Builder.addTransaction transaction
            |> Builder.execute connection sql []
            |> ignore
        transaction.Rollback ()
        connection.Close ()
        let sql = """
            select count (1)
            from x;
        """
        executeScalar<int> connection sql []
            |> should equal 0
