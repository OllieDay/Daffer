module Tests

    open Daffer
    open FsUnit
    open Npgsql
    open Xunit

    let connection = new NpgsqlConnection ("Server=localhost; Port=5432; Database=postgres; User Id=postgres;")

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``execute`` value =
        execute connection "" ["value" => value] |> ignore

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``executeAsync`` value =
        async {
            do! executeAsync connection "" ["value" => value] |> Async.Ignore
        }

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``executeReader`` value =
        use reader = executeReader connection "select @value" ["value" => value]
        reader.Read () |> ignore
        reader.[0] |> should equal value

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``executeReaderAsync`` value =
        async {
            use! reader = executeReaderAsync connection "select @value" ["value" => value]
            reader.Read () |> ignore
            reader.[0] |> should equal value
        }

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``executeScalar`` value =
        executeScalar<int> connection "select @value" ["value" => value]
            |> should equal value

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``executeScalarAsync`` value =
        async {
            let! result = executeScalarAsync<int> connection "select @value" ["value" => value]
            result |> should equal value
        }

    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``query`` a b =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        let parameters = ["a" => a; "b" => b]
        query<int> connection sql parameters
            |> should equal [a; b]

    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``queryAsync`` a b =
        async {
            let sql = """
                select @a as x
                union
                select @b as x
                order by x"""
            let parameters = ["a" => a; "b" => b]
            let! result = queryAsync<int> connection sql parameters
            result |> should equal [a; b]
        }

    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``queryFirst`` a b =
        let sql = """
            select @a as x
            union
            select @b as x
            order by x"""
        let parameters = ["a" => a; "b" => b]
        queryFirst<int> connection sql parameters
            |> should equal a

    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``queryFirstAsync`` a b =
        async {
            let sql = """
                select @a as x
                union
                select @b as x
                order by x"""
            let parameters = ["a" => a; "b" => b]
            let! result = queryFirstAsync<int> connection sql parameters
            result |> should equal a
        }

    [<Fact>]
    let ``queryFirstOrDefault`` () =
        queryFirstOrDefault<int> connection "select null limit 0" []
            |> should equal 0

    [<Fact>]
    let ``queryFirstOrDefaultAsync`` () =
        async {
            let! result = queryFirstOrDefaultAsync<int> connection "select null limit 0" []
            result |> should equal 0
        }

    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``queryMultiple`` a b =
        let reader = queryMultiple connection "select @a; select @b" ["a" => a; "b" => b]
        reader.Read<int> ()
            |> Seq.exactlyOne
            |> should equal a
        reader.Read<int> ()
            |> Seq.exactlyOne
            |> should equal b


    [<Theory>]
    [<InlineData (1, 2)>]
    [<InlineData (3, 4)>]
    [<InlineData (5, 6)>]
    let ``queryMultipleAsync`` a b =
        async {
            let! reader = queryMultipleAsync connection "select @a; select @b" ["a" => a; "b" => b]
            reader.Read<int> ()
                |> Seq.exactlyOne
                |> should equal a
            reader.Read<int> ()
                |> Seq.exactlyOne
                |> should equal b
        }

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``querySingle`` value =
        querySingle<int> connection "select @value" ["value" => value]
            |> should equal value

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``querySingleAsync`` value =
        async {
            let! result = querySingleAsync<int> connection "select @value" ["value" => value]
            result |> should equal value
        }

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``querySingleOrDefault`` value =
        querySingleOrDefault<int> connection "select @value" ["value" => value]
            |> should equal value

    [<Theory>]
    [<InlineData 1>]
    [<InlineData 2>]
    [<InlineData 3>]
    let ``querySingleOrDefaultAsync`` value =
        async {
            let! result = querySingleOrDefaultAsync<int> connection "select @value" ["value" => value]
            result |> should equal value
        }
