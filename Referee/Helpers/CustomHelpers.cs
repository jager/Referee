﻿using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web;

namespace Referee.Helpers
{
    public static class CustomHelpers
    {
        public static MvcHtmlString LiNavigationElement(this HtmlHelper helper, string Name, string Href, string ImgSrc, bool IsActive, bool ForAuth)
        {
            if (ForAuth && !HttpContext.Current.Request.IsAuthenticated)
            {
                return new MvcHtmlString("");
            }
            return new MvcHtmlString(String.Format("<li><a href=\"{0}\" title=\"\" {1}><img src=\"{2}\")\" alt=\"\" /><span>{3}</span></a></li>",
                Href,
                IsActive ? "class=\"active\"" : "",
                ImgSrc,
                Name
                ));
        }

        public static MvcHtmlString AuthLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                return new MvcHtmlString("");
            }
            return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString AuthLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, string Role)
        {
            if (!HttpContext.Current.Request.IsAuthenticated || !HttpContext.Current.User.IsInRole(Role))
            {
                return new MvcHtmlString("");
            }
            return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static double ToUnixTimestamp(DateTime dt)
        {
            return (double)(dt - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }
    }
}