using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Guanghui.BusinessEntities;


namespace Guanghui.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Define formatters
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            var appXmlType = formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            //CORS: Microsoft.AspNet.WebApi.Cors
            var cors = new EnableCorsAttribute("*", "*", "*");  //globally enable
            config.EnableCors(cors);

            //global ApiAuthentication, disable because I want authentication happens only once, that is when user logs in (call authenciateController)
            //GlobalConfiguration.Configuration.Filters.Add(new Infrastructure.ApiAuthenticationFilter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            //register LogFilter
            config.Filters.Add(new ActionFilters.LoggingFilterAttribute());

            //add exceptionCatch
            config.Filters.Add(new ActionFilters.GlobalExceptionAttribute());


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ////OData config should be under webapi route config:
            //ODataModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<ProductEntity>("Products").EntityType.HasKey(p => p.ProductId);
            //config.MapODataServiceRoute(
            //    routeName: "ODataRoute",
            //    routePrefix: "odata",
            //    model: builder.GetEdmModel());
        }
    }
}
