module Hyprlinkr.UnitTest.FSharp.RouteLinkerTests

open System
open System.Linq.Expressions
open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open Ploeh.AutoFixture.Xunit
open Ploeh.Hyprlinkr
open Xunit
open Xunit.Extensions

type RouteDefaults = { id : obj }

let AddDefaultRoute (request : HttpRequestMessage) =
    request.RequestUri <- Uri(request.RequestUri, "api/qux")

    let config = request.GetConfiguration()
    config.Routes.MapHttpRoute(
        "DefaultApi",
        "api/{controller}/{id}",
        { id = RouteParameter.Optional }) |> ignore

    request.Properties.[HttpPropertyKeys.HttpRouteDataKey] <-
        config.Routes.GetRouteData(request)

[<Theory>][<AutoHyfData>]
let GetUriReturnsCorrectResult
    ([<Frozen>]request : HttpRequestMessage)
    (sut : RouteLinker)
    (id : string) =
        request |> AddDefaultRoute

        let actual = sut.GetUri(fun (c : FooController) -> c.GetById(id))

        let baseUri = request.RequestUri.GetLeftPart(UriPartial.Authority)
        let expected = Uri(Uri(baseUri), "api/foo/" + id)
        Assert.Equal(expected, actual)

[<Theory>][<AutoHyfData>]
let GetUriWithNullFuncExpressionThrows (sut : RouteLinker) =
    Assert.Throws<ArgumentNullException>(
        fun () -> sut.GetUri<FooController, obj>(null) |> ignore)

[<Theory>][<AutoHyfData>]
let GetUriFromInvalidFuncExpressionThrows(sut : RouteLinker) =
    Assert.Throws<ArgumentException>(
        fun () -> sut.GetUri(fun _ -> obj()) |> ignore)