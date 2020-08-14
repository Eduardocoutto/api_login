using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TesteAgape
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute("CustomApi", "{controller}/{action}/{id}", new { id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(name: "ActionPagination",
            routeTemplate: "api/{controller}/{action}/{paginacao}"
            );

            config.Routes.MapHttpRoute(name: "ActionRecuperar",
            routeTemplate: "api/{controller}/{action}/{recuperar}"
            );

            config.Routes.MapHttpRoute(name: "ActionAtualizarSenha",
            routeTemplate: "api/{controller}/{action}/{atualizarsenha}"
            );
        }
    }
}
