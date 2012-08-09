using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Models;
using Referee.DAL;
using Referee.Controllers.Base;
using Referee.Lib;
using Referee.Helpers;

namespace Referee.Controllers
{ 
    public class TournamentController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/tournament", Text = "Turnieje" }
            };
        }

        //
        // GET: /Tournament/

        public ViewResult Index()
        {
            var tournaments = Unit.TournamentRepository.Get(filter: t => t.LeagueId == null);
            ViewData["PageTitle"] = "Turnieje MWZPS";
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Tournament/Create", Text = "Dodaj turniej" }
            };
            return View(tournaments);
        }

        //
        // GET: /Tournament/Details/5

        public ViewResult Details(int id)
        {
            Tournament tournament = Unit.TournamentRepository.GetById(id);
            return View(tournament);
        }

        //
        // GET: /Tournament/Create

        public ActionResult Create(int? LeagueId)
        {
            ViewData["PageTitle"] = "Dodaj nowy turniej";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Nowy turniej" }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Tournament", Text = "Listuj turnieje" }
            };
            ViewBag.LeagueId = new SelectList(Unit.LeagueRepository.Get(filter: l => l.Type == "Tournament"), "Id", "Name", (LeagueId != null || LeagueId > 0) ? LeagueId : null);
            return View();
        } 

        //
        // POST: /Tournament/Create

        [HttpPost]
        public ActionResult Create(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                Unit.TournamentRepository.Insert(tournament);
                Unit.Save();
                return RedirectToAction("Index");  
            }
            ViewBag.LeagueId = new SelectList(Unit.LeagueRepository.Get(filter: l => l.Type == "Tournament"), "Id", "Name", tournament.LeagueId);
            return View(tournament);
        }
        
        //
        // GET: /Tournament/Edit/5
 
        public ActionResult Edit(int id)
        {
            Tournament tournament = Unit.TournamentRepository.GetById(id);
            ViewData["PageTitle"] = "Edytuj turniej";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = tournament.Name }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Tournament", Text = "Listuj turnieje" },
                new BreadcrumbHelper { Href = "/Tournament/Create", Text = "Dodaj turniej" }
            };
            
            ViewBag.LeagueId = new SelectList(Unit.LeagueRepository.Get(filter: l => l.Type == "Tournament"), "Id", "Name", tournament.LeagueId);
            ViewBag.StartTime = tournament.StartTime;
            ViewBag.StartDate = tournament.StartDate.ToShortDateString();
            ViewBag.EndDate = tournament.EndDate.ToShortDateString();
            return View(tournament);
        }

        //
        // POST: /Tournament/Edit/5

        [HttpPost]
        public ActionResult Edit(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                Unit.TournamentRepository.Update(tournament);
                Unit.Save();
                return RedirectToAction("Index");
            }
            ViewBag.LeagueId = new SelectList(Unit.LeagueRepository.Get(filter: l => l.Type == "Tournament"), "Id", "Name", tournament.LeagueId);
            ViewBag.StartTime = tournament.StartTime;
            ViewBag.StartDate = tournament.StartDate.ToShortDateString();
            ViewBag.EndDate = tournament.EndDate.ToShortDateString();
            return View(tournament);
        }

        //
        // GET: /Tournament/Delete/5
 
        public ActionResult Delete(int id)
        {
            Tournament tournament = Unit.TournamentRepository.GetById(id);
            return PartialView(tournament);
        }

        //
        // POST: /Tournament/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit.TournamentRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}