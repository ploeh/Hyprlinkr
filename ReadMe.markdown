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