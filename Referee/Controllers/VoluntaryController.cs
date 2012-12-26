using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Controllers.Base;
using Referee.Helpers;

namespace Referee.Controllers
{
    public class VoluntaryController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ViewData["IconName"] = "icon-screen";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" }
            };
        }

        public ActionResult Index()
        {
            var Voluntaries = Unit.VoluntaryRepository.Get(orderBy: o => o.OrderByDescending(b => b.Id));
            return View(Voluntaries);
        }

    }
}
