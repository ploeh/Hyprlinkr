Hyprlinkr
=========
Hyprlinkr is a small and very focused helper library for the ASP.NET Web API. It does one thing only: it creates URIs according to the application's route configuration in a type-safe manner.
Example
-------
Imagine that you're using the standard route configuration created by the Visual Studio project template:
```C#
name: "API Default",
routeTemplate: "api/{controller}/{id}",
defaults: new { id = RouteParameter.Optional }
```
In that case, you can create an URI to a the resource handled by the FooController.GetById Action Method like this:
```C#
var uri = linker.GetUri<FooController>(r => r.GetById(1337));
```
This will create a URI like this:
```
http://localhost/api/foo/1337
```
assuming that the current host is http://localhost.
Creating a RouteLinker instance
-------------------------------
The RouteLinker class has two constructor overloads:
```C#
public RouteLinker(HttpRequestMessage request)

public RouteLinker(HttpRequestMessage request, IRouteDispatcher dispatcher)
```
In both cases it requires an instance of the HttpRequestMessage class, which is provided by the ASP.NET Web API for each request. The preferred way to get this instance is to implement a custom IHttpControllerActivator and create the RouteLinker instance from there.
```C#
public IHttpController Create(
    HttpRequestMessage request,
    HttpControllerDescriptor controllerDescriptor,
    Type controllerType)
{
    var linker = new RouteLinker(request);

    // Use linker and other services to create the appropriate Controller.
    // If desired, a DI Container can be used for this task.
}
```
Such a custom IHttpControllerActivator can be registered in Global.asax like this:
```C#
GlobalConfiguration.Configuration.Services.Replace(
    typeof(IHttpControllerActivator),
    new MyCustomControllerActivator());
```
Custom route dispatching
------------------------
The default behavior for RouteLinker is to assume that there's only a single configured route for the ASP.NET Web API, and that this route is named "API Default". This behavior is implemented by the DefaultRouteDispatcher class. If you require different dispatching behavior, you can implement a custom IRouteDispatcher and inject it into the RouteLinker instances.
Nuget
-----
Hyprlinkr is [available via nuget](https://nuget.org/packages/Hyprlinkr)
Example code
------------
The *ExampleService* project, included in the source code, provides a very simple example of how to wire and use Hyprlinkr. As an example, in HomeController.cs you can see that links are added to a model instance like this:
```C#
public HomeModel Get(string id)
{
    return new HomeModel
    {
        Name = id,
        Links = new[]
        {
            new AtomLinkModel
            {
                Href = this.linker.GetUri<HomeController>(r =>
                    r.Get(id)).ToString(),
                Rel = "self"
            },
            new AtomLinkModel
            {
                Href = this.linker.GetUri<HomeController>(r =>
                    r.Get()).ToString(),
                Rel = "http://sample.ploeh.dk/rels/home"
            }
        }
    };
}
```
This produces a representation equivalent to this:
```XML
<home xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      xmlns:xsd="http://www.w3.org/2001/XMLSchema"
      xmlns="http://www.ploeh.dk/hyprlinkr/sample/2012">
    <links>
        <link xmlns="http://www.w3.org/2005/Atom"
              href="http://localhost:6788/home/ploeh"
              rel="self"/>
        <link xmlns="http://www.w3.org/2005/Atom"
              href="http://localhost:6788/"
              rel="http://sample.ploeh.dk/rels/home"/>
    </links>
    <name>ploeh</name>
</home>
```
In order to run the sample application, open the Hyprlinkr.sln solution and set *ExampleService* as the startup project, then run the application by hitting F5 (or Ctrl+F5).
Contribute
----------
Hyprlinkr is open source, and pull requests are welcome.
When developing for Hyprlinkr, please respect the coding style already present. Look around in the source code to get a feel for it. Also, please follow the [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html). When creating pull requests, please keep the Single Responsibility Principle in mind. A pull request that does a single thing very well is more likely to be accepted.

Hyprlinkr was developed entirely using TDD. Pull requests without test coverage will be politely declined.

Please be aware that the Visual Studio solution contains a new build configuration (besides Debug and Release) called Verify. This configuration treats all warnings and errors, and runs Code Analysis. No Code Analyis warnings should be suppressed unless the documented conditions for suppression are satisfied. Otherwise, a documented agreement between at least two active developers of the project should be reached to allow a suppression of a non-suppressable warning.
Credits
-------
The strongly typed Resource Linker idea was [originally presented by José F. Romaniello](http://joseoncode.com/2011/03/18/wcf-web-api-strongly-typed-resource-linker/).