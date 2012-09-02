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
    public class NominationController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/Nomination", Text = "Obsady" }
            };
        }

        //
        // GET: /Nomination/
        [Authorize(Roles = HelperRoles.Sedzia)]
        public ViewResult Index()
        {
            var Nominations = Unit.NominationRepository.Get();
            List<NominationDetails> NominationEvents = new List<NominationDetails>();
            foreach (Nomination _nomination in Nominations)
            {
                Event _event = new Event();
                if (_nomination.GameId != null)
                {
                    _event.Parse(_nomination.Game, "game");
                }
                else if (_nomination.TournamentId != null)
                {
                    _event.Parse(_nomination.Tournament, "tournament");
                }
                else
                {
                    throw new Exception("Brak typu nominacji");
                }
                NominationEvents.Add(new NominationDetails {  Event = _event, Nomination = _nomination, NominatedReferees = _nomination.Nominateds } );
            }
            return View(NominationEvents);
        }

        //
        // GET: /Nomination/Details/5
        [Authorize(Roles = HelperRoles.Sedzia)]
        public ViewResult Details(int id)
        {
            Nomination nomination = Unit.NominationRepository.GetById(id);
            Event Event = new Event();
            if (nomination.GameId != null)
            {
                Event.Parse(nomination.Game, "game");
            }
            else
            {
                Event.Parse(nomination.Tournament, "tournament");
            }
            ViewBag.Event = Event;
            return View(nomination);
        }

        //
        // GET: /Nomination/Create
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Create(int EventId, string Type)
        {
            Event Event = new Event();
            switch (Type)
            {
                case "tournament":
                    int tournamentId = EventId;
                    if (tournamentId > 0)
                    {
                        Event.Parse(Unit.TournamentRepository.GetById(tournamentId), Type);
                    }
                    break;
                
                case "game":                    
                default:
                    int gameId = EventId;                    
                    if (gameId > 0)
                    {
                        var g = Unit.NominationRepository.Get(filter: n => n.GameId == gameId);
                        if (g.Count() > 0)
                        {
                            return RedirectToAction("Edit", new { id = g.FirstOrDefault().Id });
                        }
                        Event.Parse(Unit.GameRepository.GetById(gameId), Type);
                    }
                    break;
            }
            if (Event.Name == null)
            {
                throw new Exception("Unknown event. Could not parse this event from given ident");
            }


            ViewData["PageTitle"] = "Dodaj nową obsadę";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Nowa obsada" }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Nomination", Text = "Pokaż nominacje" }
            };

            ViewBag.Event = Event;
            return View();
        }

        public PartialViewResult CreateRefereeData(object EventId, string Type, int? FunctionId)
        {
            ViewBag.ButtonDeleteVisible = false;
            if (Request.IsAjaxRequest())
            {
                ViewBag.ButtonDeleteVisible = true;
            }
            ViewData["RefereeId"] = new SelectList(Unit.RefereeRepository.Get(), "Id", "FullName");            
            ViewData["FunctionId"] = new SelectList(Unit.FunctionRepository.Get(), "Id", "Name");
            
            return PartialView();
        }

        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public PartialViewResult Update( string what, string val, int nominationId )
        {
            var Nomination = Unit.NominationRepository.GetById(nominationId);
            var PartialViewTemplate = "UpdateNote";
            switch (what)
            { 
                case "note":
                    Nomination.Note = val;
                    PartialViewTemplate = "UpdateNote";
                    break;
                case "publish":
                    if (val == "1")
                    {
                        Nomination.Published = true;
                        Nomination.PublishDate = DateTime.Now;
                    }
                    else
                    {
                        Nomination.Published = false;
                    }
                    PartialViewTemplate = "UpdatePublish";
                    break;
                case "referee":
                    int NominatedId = 0;
                    Int32.TryParse(val, out NominatedId);
                    if (NominatedId > 0)
                    {
                        var NominatedReferee = Nomination.Nominateds.Where(n => n.Id == NominatedId).FirstOrDefault();
                        PartialViewTemplate = "UpdateRefereeData";
                        return PartialView(PartialViewTemplate, NominatedReferee);
                    }
                    break;
                default:
                   break;

            }            
            Unit.NominationRepository.Update(Nomination);
            Unit.Save();
            return PartialView(PartialViewTemplate, Nomination);
        }
        /*
        public void GetCurrentRefereesSelectBox(int FunctionId, string Type, int? GameId, int? TournamentId)
        {
            if (!Request.IsAjaxRequest())
            {
            }
            if (FunctionId == null)
            {
                FunctionId = 0;
            }
            int LeagueId = 0;
            if (FunctionId == 1001 || FunctionId == 2002)
            {
                //pobierz tylko odpowiednich sędziów                
                if (Type == "game")
                {
                    var game = Unit.GameRepository.GetById(GameId);
                    LeagueId = game.LeagueId;
                }
                else if (Type == "tournament")
                {
                    var tournament = Unit.TournamentRepository.GetById(TournamentId);
                    if (tournament.LeagueId != null)
                    {
                        LeagueId = (int)tournament.LeagueId;
                    }
                }
            }

            var Referees = Unit.RefereeRepository.Get();
            if ((FunctionId == 1001 || FunctionId == 2002) && LeagueId > 0)
            {
                var RefereeAuthorization = Unit.RefRoleRepository.Get(
                                            filter: r => r.FunctionId == FunctionId && r.LeagueId == LeagueId);
                if (RefereeAuthorization.Count() > 0)
                {
                    List<int> auths = new List<int>();
                    foreach (var item in RefereeAuthorization)
                    {
                        auths.Add(item.AuthorizationId);
                    }
                    ViewBag.RefereeId = new SelectList(Referees.Where(r => auths.Contains(r.AuthorizationId)), "Id", "FullName");
                }
                else
                {
                    ViewBag.RefereeId = new SelectList(Referees, "Id", "FullName");
                }
            }
            else
            {
                ViewBag.RefereeId = new SelectList(Referees, "Id", "FullName");
            }
        }


        */






        public PartialViewResult GetCurrentReferees(int FunctionId, string Type, int? GameId, int? TournamentId)
        {
            if (!Request.IsAjaxRequest())
            {
            }
            if (FunctionId == null)
            {
                FunctionId = 0;
            }
            int LeagueId = 0;
            if (FunctionId == 1001 || FunctionId == 2002)
            {
                //pobierz tylko odpowiednich sędziów                
                if (Type == "game")
                {
                    var game = Unit.GameRepository.GetById(GameId);
                    LeagueId = game.LeagueId;
                }
                else if (Type == "tournament")
                {
                    var tournament = Unit.TournamentRepository.GetById(TournamentId);
                    if (tournament.LeagueId != null)
                    {
                        LeagueId = (int)tournament.LeagueId;
                    }
                }
            }
            
            var Referees = Unit.RefereeRepository.Get();
            if ((FunctionId == 1001 || FunctionId == 2002) && LeagueId > 0)
            {
                var RefereeAuthorization = Unit.RefRoleRepository.Get(
                                            filter: r => r.FunctionId == FunctionId && r.LeagueId == LeagueId);
                if (RefereeAuthorization.Count() > 0)
                {
                    List<int> auths = new List<int>();
                    foreach(var item in RefereeAuthorization) 
                    {
                        auths.Add(item.AuthorizationId);
                    }
                    ViewBag.RefereeId = new SelectList(Referees.Where(r => auths.Contains(r.AuthorizationId)) , "Id", "FullName");
                }
                else
                {
                    ViewBag.RefereeId = new SelectList(Referees, "Id", "FullName");
                }
            }
            else
            {
                ViewBag.RefereeId = new SelectList(Referees, "Id", "FullName");
            }
            return PartialView();
        }

        //
        // POST: /Nomination/Create

        [HttpPost]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Create(Nomination nomination, FormCollection form)
        {
            nomination.Added = DateTime.Now;
            nomination.Confirmed = false;
            nomination.ConfirmationDate = DateTime.Now;
            nomination.Emailed = false;
            nomination.EmailDate = DateTime.Now;
            nomination.PublishDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                nomination.HashConfirmation = nomination.GetCode();
                Unit.NominationRepository.Insert(nomination);
                Unit.Save();
                return RedirectToAction("Index");  
            }
            return View(nomination);
        }
        
        //
        // GET: /Nomination/Edit/5
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Edit(int id)
        {
            Nomination nomination = Unit.NominationRepository.GetById(id);
            string Type = "";
            if (nomination.GameId != null)
            {
                Type = "game";
            }
            else if (nomination.TournamentId != null) 
            {
                Type = "tournament";
            }

            Event Event = new Event();
            switch (Type)
            {
                case "tournament":
                    int TournamentId = (int)nomination.TournamentId;
                    if (TournamentId > 0)
                    {
                        Event.Parse(Unit.TournamentRepository.GetById(TournamentId), Type);
                    }
                    break;

                case "game":
                default:
                    int gameId = (int)nomination.GameId;
                    if (gameId > 0)
                    {
                        Event.Parse(Unit.GameRepository.GetById(gameId), Type);
                    }
                    break;
            }
            if (Event.Name == null)
            {
                throw new Exception("Unknown event. Could not parse this event from given ident");
            }
            ViewData["PageTitle"] = "Edytuj obsadę";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = String.Format("Obsada dla {0}", Event.Name) }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Nomination", Text = "Pokaż nominacje" }
            };
            ViewData["RefereeId"] = new SelectList(Unit.RefereeRepository.Get(), "Id", "FullName");
            ViewData["FunctionId"] = new SelectList(Unit.FunctionRepository.Get(), "Id", "Name");
            ViewBag.Event = Event;
            return View(nomination);
        }

        //
        // POST: /Nomination/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Edit(Nomination nomination, FormCollection form)
        {
            nomination.Emailed = false;
            nomination.Confirmed = false;
            nomination.HashConfirmation = nomination.GetCode();
            if (nomination.Published)
            {
                nomination.PublishDate = DateTime.Now;
            }
            if (ModelState.IsValid)
            {
                
                foreach (var n in nomination.Nominateds)
                {                    
                    if (n.Id > 0)
                    {
                        if (n.FunctionId == 0)
                        {
                            Unit.NominatedRepository.Delete(n);
                            Unit.Save();
                        }
                        else
                        {
                            Unit.NominatedRepository.Update(n);
                        }
                    }
                    else if (n.FunctionId > 0)
                    {
                        n.NominationId = nomination.Id;
                        Unit.NominatedRepository.Insert(n);
                    }
                }
                Unit.NominationRepository.Update(nomination);
                Unit.Save();
                return RedirectToAction("Index");
            }
            //ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", nomination.TournamentId);
            return View(nomination);
        }

        //
        // GET: /Nomination/Delete/5
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Delete(int id)
        {
            return PartialView(Unit.NominationRepository.GetById(id));
        }

        //
        // POST: /Nomination/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit.NominationRepository.Delete(id);
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