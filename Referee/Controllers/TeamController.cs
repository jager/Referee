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
using Referee.Helpers;

namespace Referee.Controllers
{ 
    public class TeamController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/Club", Text = "Kluby" },
                new BreadcrumbHelper { Href = "#", Text = "Zespoły" }
            };
            ViewBag.Gender = "";
        }
        //
        // GET: /Team/

        public ViewResult Index()
        {
            return View(Unit.TeamRepository.Get());
        }

        //
        // GET: /Team/Details/5

        public ViewResult Details(int id)
        {
            return View(Unit.TeamRepository.GetById(id));
        }

        private void PopulateClubSelectBox(object selectedClub = null)
        {
            ViewBag.Clubs = new SelectList(Unit.ClubRepository.Get(), "Id", "Name", selectedClub);
        }

        private void GetLeagues(Team team = null)
        {
            List<int> enrolled = new List<int>();
            if (team != null)
            {
                var enrollments = Unit.EnrollmentRepository.Get(filter: e => e.TeamId == team.Id);
                foreach (Enrollment en in enrollments)
                {
                    enrolled.Add(en.LeagueId);
                }
            }
            ViewBag.Leagues = new MultiSelectList(Unit.LeagueRepository.Get(filter: league => league.Id < 1000), "Id", "Name", enrolled);
        }

        private void AddEnrollments(string[] enrollments, Team team)
        {
            if (enrollments.Count() == 0)
            {
                return;
            }

            var teamEnrollments = Unit.EnrollmentRepository.Get(filter: e => e.TeamId == team.Id);
            if (teamEnrollments.Count() > 0)
            {
                foreach (Enrollment en in teamEnrollments)
                {
                    Unit.EnrollmentRepository.Delete(en.Id);
                }
            }
            
            foreach (string e in enrollments)
            {
                Unit.EnrollmentRepository.Insert(new Enrollment { LeagueId = Convert.ToInt32(e), TeamId = team.Id });
            }
        }

        private string SetTeamName(Team team)
        {
            string clubName = Unit.ClubRepository.GetById(team.ClubId).Name;
            return !team.Name.Contains(clubName) ? String.Format("{0} {1}", clubName, team.Name) : team.Name;
        }

        //
        // GET: /Team/Create

        public ViewResult Create(Guid ClubId)
        {
            if (ClubId != null)
            {
                ViewBag.ClubId = ClubId;
            }
            PopulateClubSelectBox();
            GetLeagues();

            ViewData["PageTitle"] = "Dodaj drużynę";
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "#", Text = "Dodaj drużynę" }
            };

            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club", Text = "Listuj kluby" },
                new BreadcrumbHelper { Href = "/Club/Create", Text = "Dodaj klub" }
            };

            return View();
        } 

        //
        // POST: /Team/Create

        [HttpPost]
        public ActionResult Create(Team team)
        {
            if (ModelState.IsValid)
            {
                team.Name = SetTeamName(team);
                Unit.TeamRepository.Insert(team);
                
                if (!String.IsNullOrEmpty(Request.Form["selectedLeagues"]))
                {
                    var selectedLeagues = Request.Form["selectedLeagues"].Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    AddEnrollments(selectedLeagues, team);                    
                }
                Unit.Save();
                return RedirectToAction("details", "club", new { @Id = team.ClubId });
            }
            if (team.ClubId != null)
            {
                ViewBag.ClubId = team.ClubId;
            }
            else
            {
                PopulateClubSelectBox();
            }
            return View(team);
        }
        
        //
        // GET: /Team/Edit/5
 
        public ActionResult Edit(int id)
        {
            Team team = Unit.TeamRepository.GetById(id);
            PopulateClubSelectBox(team.ClubId);
            GetLeagues(team);

            ViewData["PageTitle"] = "Dodaj drużynę";
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "#", Text = "Dodaj drużynę" }
            };

            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club", Text = "Listuj kluby" },
                new BreadcrumbHelper { Href = "/Club/Create", Text = "Dodaj klub" }
            };
            ViewBag.Gender = team.Gender;
            return View(team);
        }

        //
        // POST: /Team/Edit/5

        [HttpPost]
        public ActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {
                team.Name = SetTeamName(team);
                Unit.TeamRepository.Update(team);
                if (!String.IsNullOrEmpty(Request.Form["selectedLeagues"]))
                {
                    var selectedLeagues = Request.Form["selectedLeagues"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    AddEnrollments(selectedLeagues, team);
                }
                Unit.Save();
                return RedirectToAction("details", "club", new { @Id = team.ClubId });
            }
            return View(team);
        }

        //
        // GET: /Team/Delete/5
 
        public ActionResult Delete(int id)
        {
            return PartialView(Unit.TeamRepository.GetById(id));
        }

        //
        // POST: /Team/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string ClubId)
        {
            Unit.TeamRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Details", "Club", new { @Id = ClubId });
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}