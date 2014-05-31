using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Mvc;
using MITD.Fuel.Service.Host.Infrastructure;
using Thinktecture.IdentityModel.Tokens.Http;

namespace MITD.Fuel.Service.Host.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var authConfig = new AuthenticationConfiguration
            {
                RequireSsl = false,
                ClaimsAuthenticationManager = new ClaimsAuthenticationManager(),
                EnableSessionToken = true,
                //SessionToken = new SessionTokenConfiguration()
                //               {
                //                   EndpointAddress = "apiArea/fuel/token"
                //               }
            };
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer(System.Configuration.ConfigurationManager.AppSettings["SigningThumbPrint"], System.Configuration.ConfigurationManager.AppSettings["IssuerURI"]);
            var handlerConfig = new SecurityTokenHandlerConfiguration();
            handlerConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(System.Configuration.ConfigurationManager.AppSettings["AudianceUri"]));
            handlerConfig.IssuerNameRegistry = registry;
            handlerConfig.CertificateValidator = X509CertificateValidator.None;
            handlerConfig.ServiceTokenResolver = new X509CertificateStoreTokenResolver(StoreName.My, StoreLocation.LocalMachine);
            authConfig.AddSaml2(handlerConfig, AuthenticationOptions.ForAuthorizationHeader("SAML"), AuthenticationScheme.SchemeOnly("SAML"));

            config.MessageHandlers.Add(new AuthenticationHandler(authConfig));


            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new
               {
                   id = RouteParameter.Optional
               }
           );

            config.Routes.MapHttpRoute(
                name: "OffhirePricingValueRoute",
                routeTemplate: "apiArea/{area}/OffhirePricingValue/",
                defaults: new
                {
                    controller = "OffhirePricingValue"
                }
            );

            //{startDateTime:datetime:regex(\d{4}\d{2}\d{2} \d{2}\d{2}\d{2})}

            //{startDateTime:datetime:regex(\\d{4}\\d{2}\\d{2}\\d{2}\\d{2}\\d{2})}

            //     [Route("date/{pubdate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
            //[Route("date/{*pubdate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]

            config.Routes.MapHttpRoute(
                name: "OffhireManagementSystemPreparedDataRoute",
                routeTemplate: "apiArea/{area}/OffhireManagementSystem/{referenceNumber}/PreparedData/{introducerId}",
                defaults: new
                {
                    controller = "OffhireManagementSystemPreparedData"
                }
            );

            config.Routes.MapHttpRoute(
                name: "OffhireManagementSystemRoute",
                routeTemplate: "apiArea/{area}/OffhireManagementSystem/{referenceNumber}",
                defaults: new
                {
                    controller = "OffhireManagementSystem",
                    referenceNumber = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "OffhireDetailRoute",
                routeTemplate: "apiArea/{area}/Offhire/{id}/Detail/{detailId}",
                defaults: new
                {
                    controller = "OffhireDetail",
                    detailId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "OffhireRoute",
                routeTemplate: "apiArea/{area}/Offhire/{id}",
                defaults: new
                {
                    controller = "Offhire",
                    vesselId = RouteParameter.Optional
                }
            );




            config.Routes.MapHttpRoute(
                name: "CompanyOwnedVesselRoute",
                routeTemplate: "apiArea/{area}/Company/{id}/OwnedVessel/{vesselId}",
                defaults: new
                {
                    controller = "CompanyOwnedVessel",
                    vesselId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
               name: "ScrapInventoryOperationRoute",
               routeTemplate: "apiArea/{area}/Scrap/{id}/InventoryOperation/{operationId}",
               defaults: new
               {
                   controller = "ScrapInventoryOperation",
                   operationId = RouteParameter.Optional
               }
            );

            config.Routes.MapHttpRoute(
                name: "ScrapDetailRoute",
                routeTemplate: "apiArea/{area}/Scrap/{id}/Detail/{detailId}",
                defaults: new
                {
                    controller = "ScrapDetail",
                    detailId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "ScrapRoute",
                routeTemplate: "apiArea/{area}/Scrap/{id}",
                defaults: new
                {
                    controller = "Scrap",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
               name: "VoyageLogRoute",
               routeTemplate: "apiArea/{area}/Voyage/{voyageId}/Log/",
               defaults: new
               {
                   controller = "VoyageLog",
               }
            );

            config.Routes.MapHttpRoute(
               name: "VoyageRoute",
               routeTemplate: "apiArea/{area}/Voyage/{id}",
               defaults: new
               {
                   controller = "Voyage",
                   id = RouteParameter.Optional
               }
            );

            config.Routes.MapHttpRoute(
               name: "FuelReportInventoryResultRoute",
               routeTemplate: "apiArea/{area}/FuelReport/{id}/InventoryResult/",
               defaults: new
               {
                   controller = "FuelReportInventoryResult",
               }
            );

            config.Routes.MapHttpRoute(
               name: "FuelReportDetailInventoryOperationRoute",
               routeTemplate: "apiArea/{area}/FuelReport/{id}/Detail/{detailId}/InventoryOperation/{operationId}",
               defaults: new
               {
                   controller = "FuelReportDetailInventoryOperation",
                   operationId = RouteParameter.Optional
               }
            );

            config.Routes.MapHttpRoute(
                name: "FuelReportDetailRoute",
                routeTemplate: "apiArea/{area}/FuelReport/{id}/Detail/{detailId}",
                defaults: new
                {
                    controller = "FuelReportDetail",
                    detailId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "OrderItemRoute",
                routeTemplate: "apiArea/{area}/Order/{id}/OrderItem/{orderItemId}",
                defaults: new
                {
                    controller = "OrderItem",
                    orderItemId = RouteParameter.Optional
                }
            );


            config.Routes.MapHttpRoute(
                name: "InvoiceItemRoute",
                routeTemplate: "apiArea/{area}/Invoice/{id}/InvoiceItem/{invoiceItemId}",
                defaults: new
                {
                    controller = "InvoiceItem",
                    invoiceItemId = RouteParameter.Optional
                }
            );


            config.Routes.MapHttpRoute(
                name: "MainUnitValue",
                routeTemplate: "apiArea/{area}/MainUnit/{goodId}/{goodUnitId}/{value}",
                defaults: new
                {
                    controller = "OrderItem",
                });

            config.Routes.MapHttpRoute(
                name: "CharterItemRoute",
                routeTemplate: "apiArea/{area}/Charter/{id}/CharterItem/{charterItemId}",
                defaults: new
                {
                    controller = "CharterItem",
                    charterItemId = RouteParameter.Optional
                }
                );





            config.Routes.MapHttpRoute(
                name: "Default_apiArea",
                routeTemplate: "apiArea/{area}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                      name: "apiAreaWithAction",
                      routeTemplate: "apiArea/{area}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                  );

            config.Routes.MapHttpRoute(
                             name: "Default_areaApi",

                routeTemplate: "apiArea/{area}/{controller}/{action}/{id}",
                // "apiArea/Order/{id}/OrderDetail/{DetailId}",
                defaults: new { action = "UpdateOrderItem", id = RouteParameter.Optional }
                         );


            config.Filters.Add(DependencyResolver.Current.GetService<GlobalExceptionHandlingAttribute>());
        }
    }
}
