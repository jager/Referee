using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Controllers.Base;
using Referee.Models;
using Referee.Helpers;

namespace Referee.Controllers
{
    [Authorize(Roles=HelperRoles.Administrator)]
    public class ConfigurationController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/Configuration", Text = "Konfiguracja systemu" }
            };
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Configuration", Text = "Ogólna" },
                new BreadcrumbHelper { Href = "/Configuration/League", Text = "Ligi" },
                new BreadcrumbHelper { Href = "/Configuration/LeagueAuthorization", Text = "Uprawnienia" }
            };
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["PageTitle"] = "Konfiguracja systemu";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Ogólna" }
            );
            
            return View(Configuration);
        }

        [HttpPost]
        public JsonResult Update(AppConfig AppConfig)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                var Config = Unit.ConfigRepository.Get(filter: c => c.Key == AppConfig.Key).FirstOrDefault();
                Config.Value = AppConfig.Value;
                Unit.ConfigRepository.Update(Config);
                Unit.Save();
                
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }

        public ActionResult League()
        {
            var Leagues = Unit.LeagueRepository.Get();
            return View(Leagues);
        }

        public JsonResult UpdateLeague(League League)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                if (ModelState.IsValid)
                {
                    Unit.LeagueRepository.Update(League);
                    Unit.Save();
                }
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }

        public ActionResult LeagueAuthorization()
        {
            var Roles = Unit.RefRoleRepository.Get();
            ViewBag.Auths = Unit.AuthorizationRepository.Get();
            ViewBag.Leagues = Unit.LeagueRepository.Get();
            return View(Roles);
        }

        public JsonResult UpdateAuth(RefereeRole RefereeRole)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                if (ModelState.IsValid)
                {
                    Unit.RefRoleRepository.Update(RefereeRole);
                    Unit.Save();
                }
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }

        public JsonResult AddAuth(RefereeRole RefereeRole)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                if (ModelState.IsValid)
                {
                    Unit.RefRoleRepository.Insert(RefereeRole);
                    Unit.Save();
                }
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }

        public JsonResult DeleteAuth(RefereeRole RefereeRole)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                if (ModelState.IsValid)
                {
                    Unit.RefRoleRepository.Delete(RefereeRole);
                    Unit.Save();
                }
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }
    }
}
