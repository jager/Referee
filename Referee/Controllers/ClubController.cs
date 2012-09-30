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
using Referee.ViewModels;
using Referee.Helpers;

namespace Referee.Controllers
{ 
    public class ClubController : BaseController
    {

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
     
            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/Club", Text = "Kluby" }
            };
        }


        //
        // GET: /Club/

        public ViewResult Index(int? League)
        {

            
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club/Create", Text = "Dodaj klub", Role = HelperRoles.WydzialGieriEwidencji }
            };
            IEnumerable<Club> Clubs;

            //get all leagues
            ViewBag.LeaguesSelect = Unit.LeagueRepository.Get(filter: l => l.Id < 1000);



            if (League != null)
            {
                var Enrollments = Unit.EnrollmentRepository.Get(filter: e => e.LeagueId == League);
                List<Club> Teams = new List<Club>();
                foreach (var Enrollment in Enrollments)
                {
                    Teams.Add(Enrollment.Team.Club);
                }
                Clubs = (IEnumerable<Club>)Teams;
                ViewData["PageTitle"] = String.Format("Kluby MWZPS - {0}",((IEnumerable<League>)ViewBag.LeaguesSelect).Where( l => l.Id == League).FirstOrDefault().Name);
            } 
            else 
            {
                Clubs = Unit.ClubRepository.Get();
                ViewData["PageTitle"] = "Kluby MWZPS";
            }
            return View(Clubs);            
        }

        //
        // GET: /Club/Details/5

        public ViewResult Details(Guid id)
        {
            
            var ClubWithTeams = new ClubDetails
            {
                Club = Unit.ClubRepository.GetById(id),
                Teams = Unit.TeamRepository.Get( filter : team => team.ClubId == id ).ToList<Team>()
            };


            ViewData["PageTitle"] = String.Format("Dane klubu: {0}", ClubWithTeams.Club.Name);
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = ClubWithTeams.Club.Name }
            );

            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club", Text = "Listuj kluby" },
                new BreadcrumbHelper { Href = "/Club/Create", Text = "Dodaj klub", Role = HelperRoles.WydzialGieriEwidencji }
            };

            return View(ClubWithTeams);
        }

        //
        // GET: /Club/Create
        [Authorize(Roles=HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Create()
        {
            ViewData["PageTitle"] = "Dodaj nowy klub";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Nowy klub" }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club", Text = "Listuj kluby" }
            };
            return View();
        } 

        //
        // POST: /Club/Create

        [HttpPost]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Create(Club club)
        {
            if (ModelState.IsValid)
            {
                club.Id = Guid.NewGuid();
                Unit.ClubRepository.Insert(club);
                Unit.Save();
                return RedirectToAction("Index");  
            }

            return View(club);
        }
        
        //
        // GET: /Club/Edit/5

        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Edit(Guid id)
        {
            var Club = Unit.ClubRepository.GetById(id);
            ViewData["PageTitle"] = String.Format("Edytuj klub: {0}", Club.Name);
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = Club.Name }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Club", Text = "Listuj kluby" },
                new BreadcrumbHelper { Href = "/Club/Create", Text = "Dodaj klub", Role = HelperRoles.WydzialGieriEwidencji }
            };
            return View(Club);
        }

        //
        // POST: /Club/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Edit(Club club)
        {
            if (ModelState.IsValid)
            {
                Unit.ClubRepository.Update(club);
                Unit.Save();
                return RedirectToAction("Index");
            }
            return View(club);
        }

        //
        // GET: /Club/Delete/5

        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Delete(Guid id)
        {
            return PartialView(Unit.ClubRepository.GetById(id));
        }

        //
        // POST: /Club/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Unit.ClubRepository.Delete(id);
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