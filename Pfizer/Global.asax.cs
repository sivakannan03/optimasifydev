using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Pfizer.Controllers;
using System.Configuration;

namespace Pfizer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {


            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

                //if (Context.IsCustomErrorEnabled)
                //    ShowCustomErrorPage(Server.GetLastError());
            
        }


        

        private void ShowCustomErrorPage(Exception exception)
        {
            var httpException = exception as HttpException ?? new HttpException(500, "Internal Server Error", exception);

          //  Response.Clear();
            var routeData = new RouteData();
            routeData.Values.Add("Home", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            switch (httpException.GetHttpCode())
            {
                case 404:
                    routeData.Values.Add("action", "HttpError403");
                    break;

                //case 404:
                //    routeData.Values.Add("action", "HttpError404");
                //    break;

                //case 500:
                //    routeData.Values.Add("action", "HttpError500");
                //    break;

                default:
                    //routeData.Values.Add("Home", "Login");
                    //routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
                    break;
            }

            Server.ClearError();

            IController controller = new HomeController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

    }
}