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
using Referee.Lib;

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
            return View("ListNominations", NominationEvents);
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
            ViewBag.Conflicts = GetConflictNominations(Event.MinDate, Event.MaxDate);
            return View();
        }

        public PartialViewResult CreateRefereeData(object EventId, string Type, int? FunctionId)
        {
            ViewBag.ButtonDeleteVisible = false;
            if (Request.IsAjaxRequest())
            {
                ViewBag.ButtonDeleteVisible = true;
            }
            ViewData["RefereeId"] = Unit.RefereeRepository.Get();
            ViewData["FunctionId"] = Unit.FunctionRepository.Get();
            
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


        [Authorize(Roles = HelperRoles.Sedzia)]
        public PartialViewResult Confirm(int id, Guid rid)
        {
            ViewBag.Message = "Potwierdzona";
            try
            {
                var Nominated = Unit.NominatedRepository.Get(filter: n => n.NominationId == id && n.RefereeId == rid && !n.Confirmed).FirstOrDefault();
                if (Nominated != null)
                {
                    Nominated.Confirmed = true;
                    Nominated.ConfirmedDate = DateTime.Now;
                    Unit.NominatedRepository.Update(Nominated);
                    Unit.Save();
                }
            } 
            catch (Exception ) 
            {
                ViewBag.Message = "Error";
            }
            return PartialView();
        }

        [Authorize(Roles = HelperRoles.Sedzia)]
        [HttpGet, ActionName("Confirmation")]
        public ActionResult ConfirmFromEmail(int NominationId, string HashConf, Guid RefereeId)
        {
            ViewBag.Class = "greenBack";
            ViewBag.Message = "Dziękujemy za potwierdzenie nominacji";
            try
            {
                var Nominated = Unit.NominatedRepository.Get(filter: 
                        n => n.NominationId == NominationId && 
                            n.RefereeId == RefereeId && 
                            n.HashConfirmation == HashConf && 
                            !n.Confirmed).FirstOrDefault();
                if (Nominated != null)
                {
                    Nominated.Confirmed = true;
                    Nominated.ConfirmedDate = DateTime.Now;
                    Unit.NominatedRepository.Update(Nominated);
                    Unit.Save();
                }
                else
                {
                    throw new Exception("Brak podanej nominacji lub jest już potwierdzona");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Class = "redBack";
                ViewBag.Message = ex.Message;
            }
            return View("ConfirmFromEmail");
        }




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
            List<Guid> ConflictedReferees = new List<Guid>();
            
            //pobierz tylko odpowiednich sędziów                
            if (Type == "game")
            {
                var game = Unit.GameRepository.GetById(GameId);
                LeagueId = game.LeagueId;
                DateTime minDate = game.DateAndTime.AddHours(-2);
                DateTime maxDate = game.DateAndTime.AddHours(2);
                ConflictedReferees = this.GetConflictReferees(minDate, maxDate);
            }
            else if (Type == "tournament")
            {
                var tournament = Unit.TournamentRepository.GetById(TournamentId);
                if (tournament.LeagueId != null)
                {
                    LeagueId = (int)tournament.LeagueId;
                }
                DateTime minDate = tournament.StartDate;
                DateTime maxDate = tournament.EndDate != null ? tournament.EndDate : tournament.StartDate;
                ConflictedReferees = this.GetConflictReferees(minDate, maxDate);
            }
            
            
            var Referees = Unit.RefereeRepository.Get(filter: r => !ConflictedReferees.Contains(r.Id));
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


        private List<Conflicts> GetConflictNominations(DateTime minDate, DateTime maxDate)
        {
            var Nominations = GetConflicts(minDate, maxDate);
            List<Conflicts> ConflictedNominations = new List<Conflicts>();
            foreach (var Nomination in Nominations)
            {
                foreach (var Nominated in Nomination.Nominateds)
                {
                    ConflictedNominations.Add( new Conflicts() 
                    {
                        Referee = Nominated.Referee.FullName,
                        Photo = FileUploader.GetUserPhotoPath(String.Format("{0}{1}", Nominated.Referee.DestinationFolder, Nominated.Referee.Photo)),
                        Event = (Nomination.GameId != null ? Nomination.Game.Name : Nomination.Tournament.Name),
                        Period = (Nomination.GameId != null ? Nomination.Game.DateAndTime : Nomination.Tournament.StartDate).ToString(),
                        NominationId = Nomination.Id,
                        RefereeId = Nominated.RefereeId
                    });
                }
            }
            return ConflictedNominations;
        }

        private List<Guid> GetConflictReferees(DateTime minDate, DateTime maxDate)
        {
            List<Guid> Conflicts = new List<Guid>();
            var Nominations = GetConflicts(minDate, maxDate);
            foreach (var Nomination in Nominations)
            {
                foreach (var Nominated in Nomination.Nominateds)
                {
                    Conflicts.Add(Nominated.RefereeId);                   
                }
            }
            return Conflicts;
        }

        private IEnumerable<Nomination> GetConflicts(DateTime minDate, DateTime maxDate)
        {
            return Unit.NominationRepository.Get(
                n => (n.GameId != null && n.Game.DateAndTime > minDate && n.Game.DateAndTime < maxDate) ||
                     (n.TournamentId != null && n.Tournament.StartDate > minDate && n.Tournament.StartDate < maxDate));
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
            nomination.Emailed = nomination.Published;
            nomination.EmailDate = DateTime.Now;
            nomination.PublishDate = DateTime.Now;
            foreach (var Nominated in nomination.Nominateds)
            {
                Nominated.Confirmed = false;
                Nominated.ConfirmedDate = DateTime.Now;
                Nominated.HashConfirmation = nomination.GetCode();
            }
            if (ModelState.IsValid)
            {
                nomination.HashConfirmation = nomination.GetCode();                
                Unit.NominationRepository.Insert(nomination);
                Unit.Save();
                if (nomination.Emailed && this.GetConfigValue("SendEmails") == "1" && this.GetConfigValue("SendNominationsEmail") == "1")
                {
                    SendConfirmationMessages(nomination);
                }
                return RedirectToAction("Index");  
            }
            return View(nomination);
        }

        /// <summary>
        /// Wysyła potwierdzenia do sędziów o zrobionych obsadach
        /// </summary>
        /// <param name="nomination">Nominacja</param>
        private void SendConfirmationMessages(Nomination nomination)
        {
            foreach (var Nominated in nomination.Nominateds)
            {
                var rf = Unit.RefereeRepository.GetById(Nominated.RefereeId);
                NominationMessage nm = new NominationMessage() 
                {
                    Type = nomination.GameId != null ? "game" : "tournament",
                    NominationId = nomination.Id,
                    HashConfirmation = Nominated.HashConfirmation,
                    Mailadr = rf.Mailadr,
                    RefereeId = Nominated.RefereeId
                };
                MailHelper.CreateNominationMessage(nm);
            }
        }


        private void GetNewNominations(Guid RefereeId)
        {
            var Nominations = Unit.NominatedRepository.Get(filter: n => n.RefereeId == RefereeId && !n.Confirmed);
            
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
            ViewData["RefereeId"] = Unit.RefereeRepository.Get();
            ViewData["FunctionId"] = Unit.FunctionRepository.Get();
            ViewBag.Event = Event;
            return View(nomination);
        }

        //
        // POST: /Nomination/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Edit(Nomination nomination, FormCollection form)
        {
            
            if (ModelState.IsValid)
            {
               
                List<Nominated> NewNominateds = nomination.Nominateds as List<Nominated>;
                List<Nominated> ExistingNominateds = Unit.NominatedRepository.Get(filter: n => n.NominationId == nomination.Id).ToList();
                Nomination NewNomination = Unit.NominationRepository.GetById(nomination.Id);

                foreach (Nominated n in ExistingNominateds)
                {
                    Unit.NominatedRepository.Delete(n);
                }

                foreach (Nominated n in NewNominateds)
                {
                    n.Confirmed = false;
                    n.ConfirmedDate = DateTime.Now;
                    n.HashConfirmation = NewNomination.GetCode();
                    NewNomination.Nominateds.Add(n);
                }
                NewNomination.Note = nomination.Note;
                NewNomination.Published = nomination.Published;
                NewNomination.Emailed = nomination.Published;
                NewNomination.Confirmed = false;
                NewNomination.HashConfirmation = NewNomination.GetCode();
                if (nomination.Published)
                {
                    NewNomination.PublishDate = DateTime.Now;
                }
                Unit.NominationRepository.Update(NewNomination);
                Unit.Save();
                if (NewNomination.Emailed)
                {
                    SendConfirmationMessages(NewNomination);
                }
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