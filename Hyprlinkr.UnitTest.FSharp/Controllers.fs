namespace Hyprlinkr.UnitTest.FSharp

open System.Web.Http

type FooController() = 
    inherit ApiController()
    member this.GetById id = obj()