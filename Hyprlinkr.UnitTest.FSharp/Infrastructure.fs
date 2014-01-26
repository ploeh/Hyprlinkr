namespace Hyprlinkr.UnitTest.FSharp

open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open Ploeh.AutoFixture
open Ploeh.AutoFixture.Kernel
open Ploeh.AutoFixture.AutoMoq
open Ploeh.AutoFixture.Xunit

type HttpSchemeCustomization() =
    interface ICustomization with
        member this.Customize fixture = fixture.Inject(UriScheme("http"))

type HttpRequestMessageCustomization() =
    interface ICustomization with
        member this.Customize fixture =
            let config = fixture.Create<HttpConfiguration>()
            let assignConfig (msg : HttpRequestMessage) =
                msg.Properties.[HttpPropertyKeys.HttpConfigurationKey] <-
                    config
            fixture.Customize<HttpRequestMessage>(
                fun c -> c.Do assignConfig :> ISpecimenBuilder)

type HyprlinkrCustomization() =
    inherit CompositeCustomization(
        HttpSchemeCustomization(),
        AutoMoqCustomization(),
        HttpRequestMessageCustomization())

type AutoHyfDataAttribute() =
    inherit AutoDataAttribute(Fixture().Customize(HyprlinkrCustomization()))