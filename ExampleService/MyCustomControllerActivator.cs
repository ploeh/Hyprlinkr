using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dispatcher;
using System.Web.Http.Controllers;
using System.Net.Http;
using Ploeh.Hyprlinkr;

namespace Ploeh.Samples.Hyprlinkr.ExampleService
{
    public class MyCustomControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            if (controllerType == typeof(HomeController))
                return new HomeController(
                    new RouteLinker(
                        request));

            return new DefaultHttpControllerActivator().Create(
                request,
                controllerDescriptor,
                controllerType);
        }
    }
}