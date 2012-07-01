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
Nuget
-----
Hyprlinkr is available via nuget: https://nuget.org/packages/Hyprlinkr
Contribute
----------
Hyprlinkr is open source, and pull requests are welcome.
When developing for Hyprlinkr, please respect the coding style already present. Look around in the source code to get a feel for it. Also, please follow the [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html). When creating pull requests, please keep the Single Responsibility Principle in mind. A pull request that does a single thing very well is more likely to be accepted.

Hyprlinkr was developed entirely using TDD. Pull requests without test coverage will be politely declined.

Please be aware that the Visual Studio solution contains a new build configuration (besides Debug and Release) called Verify. This configuration treats all warnings and errors, and runs Code Analysis. No Code Analyis warnings should be suppressed unless the documented conditions for suppression are satisfied. Otherwise, a documented agreement between at least two active developers of the project should be reached to allow a suppression of a non-suppressable warning.
Credits
-------
The strongly typed Resource Linker idea was originally presented by José F. Romaniello: http://joseoncode.com/2011/03/18/wcf-web-api-strongly-typed-resource-linker/