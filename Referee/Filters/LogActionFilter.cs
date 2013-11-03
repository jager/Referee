using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Helpers;
using System.Web.Routing;

namespace Referee.Filters
{
    public class LogActionFilter : ActionFilterAttribute
    {        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var _request = context.HttpContext.Request;
            string Message = String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                context.HttpContext.Session.SessionID,
                _request.HttpMethod,
                _request.Path,
                _request.UrlReferrer,
                _request.Browser.Browser,
                _request.Browser.Version,
                _request.PhysicalPath,                
                _request.UserHostAddress,
                String.IsNullOrEmpty(context.HttpContext.User.Identity.Name) ? "Public" : context.HttpContext.User.Identity.Name
                );
            Log(context.RouteData, Message);
        }

        private void Log(RouteData routeData, string Message)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            string OutputData = String.Format("Controller: {0}; Action: {1}; {2} \r\n", controllerName, actionName, Message);
            LogHelper.Information(OutputData);
        }
    }
}