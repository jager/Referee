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
using System.Web.Security;

namespace Referee.Controllers
{ 
    public class GameController : BaseController
    {
        private RefereeContext db = new RefereeContext();
        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/tournament", Text = "Mecze" }
            };
        }
        //
        // GET: /Game/

        public ActionResult Index(int? LeagueId)
        {
            var ListOfLeagues = Unit.LeagueRepository.Get(filter: l => l.Visible);
            
            League league;
            if (LeagueId == null || LeagueId == 0)
            {
                league = ListOfLeagues.First<League>();
                LeagueId = league.Id;
            }
            else
            {
                league = Unit.LeagueRepository.GetById(LeagueId);
            }

            if (league.Type == "tournament")
            {
                return RedirectToAction("IndexTournament", new { LeagueId = LeagueId });
            }

            ViewBag.DefaultLeague = league;
            ViewBag.ListOfLeagues = ListOfLeagues;
            ViewBag.ListOfTeams = Unit.EnrollmentRepository.Get(filter: e => e.LeagueId == LeagueId, IncludeProperties: "Team");

            return View(Unit.GameRepository.Get(
                g => g.SeasonId == CurrentSeason.Id && g.LeagueId == LeagueId,
                g => g.OrderBy(o => o.DateAndTime)));
        }


        public ViewResult IndexTournament(int? LeagueId)
        {
            var ListOfLeagues = Unit.LeagueRepository.Get(filter: l => l.Visible);

            League league;
            if (LeagueId == null || LeagueId == 0)
            {
                league = ListOfLeagues.Where(l => l.Type == "tournament").First<League>();
                LeagueId = league.Id;
            }
            else
            {
                league = Unit.LeagueRepository.GetById(LeagueId);
            }
            
            ViewBag.DefaultLeague = league;
            ViewBag.ListOfLeagues = ListOfLeagues;
            ViewBag.ListOfTeams = Unit.EnrollmentRepository.Get(filter: e => e.LeagueId == LeagueId, IncludeProperties: "Team");

            return View(Unit.TournamentRepository.Get(
                t => t.SeasonId == CurrentSeason.Id && t.LeagueId == LeagueId,
                t => t.OrderBy(o => o.StartDate)));
        }

        //
        // GET: /Game/Details/5

        public ViewResult Details(int id)
        {
            var MatchGame = Unit.GameRepository.GetById(id);
            ViewBag.Nomination = Unit.NominationRepository.Get(filter: n => n.GameId == id && n.Published).FirstOrDefault();
            ViewData["PageTitle"] = String.Format("Dane meczu: {0}", MatchGame.Name);
            ViewData["IconName"] = "icos-home2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = String.Format("/Game/?LeagueId={0}", MatchGame.LeagueId), Text = "Ligi" },
                new BreadcrumbHelper { Href = "#", Text = MatchGame.Name }
            };

            ViewBag.Voluntary = this.CheckVoluntary(id);
            return View(MatchGame);
        }

        private Voluntary CheckVoluntary(int GameId)
        {
            return Unit.VoluntaryRepository.Get(filter: v => v.GameId == GameId && v.Active).FirstOrDefault();
        }

        //
        // GET: /Game/Create
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Create(int LeagueId)
        {
            ViewBag.SeasonId = CurrentSeason.Id;
            ViewBag.LeagueId = LeagueId;
            ViewBag.dtDate = "";
            ViewBag.dtTime = "08:00";
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name");
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name");
            var RO = GetRefereesFromRO();
            ViewBag.RO = RO;
            return View();
        } 

        //
        // POST: /Game/Create

        [HttpPost]
        [Authorize(Roles=HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Create(Game game, FormCollection form)
        {
            game = PrepareGame(game, form);
            ModelState.Remove("HostTeam");
            ModelState.Remove("GuestTeam");
            string GameTeams = "";
            string[] selectedRO = new string[] { };
            if (!String.IsNullOrEmpty(Request.Form["RO"]))
            {
                selectedRO = Request.Form["RO"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (ModelState.IsValid)
            {
                Unit.GameRepository.Insert(game);
                GameTeams = String.Format("{0} vs. {1} ({2})", game.HostTeam, game.GuestTeam, game.LeagueName);
                Unit.Save();
                if (selectedRO.Count() > 0 && this.GetConfigValue("SendEmails") == "1")
                {
                    //wyślij maila do każdego
                    foreach (var mailadr in selectedRO)
                    {
                        MailHelper.CreateNewGameMessage(mailadr, GameTeams);
                    }
                }
                if (form["Move"] != null && !String.IsNullOrEmpty(form["Move"]) && form["Move"] == "Nomination")
                {
                    return RedirectToAction("Create", "Nomination", new { EventId = game.Id, @Type = "game" });
                }
                return RedirectToAction("Index", new { LeagueId = game.LeagueId });
            }
            
            
            ViewBag.SeasonId = CurrentSeason.Id;
            ViewBag.LeagueId = game.LeagueId;
            ViewBag.dtDate = game.DateAndTime.ToShortDateString();
            ViewBag.dtTime = game.DateAndTime.ToShortTimeString();
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == game.LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.HostTeamId);
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.GuestTeamId);
            return View(game);
        }

        [HttpPost]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult CreateAndNominate(Game game, FormCollection form)
        {
            game = PrepareGame(game, form);
            ModelState.Remove("HostTeam");
            ModelState.Remove("GuestTeam");
            if (ModelState.IsValid)
            {
                Unit.GameRepository.Insert(game);
                Unit.Save();
                return RedirectToAction("Create", "Nomination", new { EventId = game.Id, @Type = "game" });
            }
           
            ViewBag.SeasonId = CurrentSeason.Id;
            ViewBag.LeagueId = game.LeagueId;
            ViewBag.dtDate = game.DateAndTime.ToShortDateString();
            ViewBag.dtTime = game.DateAndTime.ToShortTimeString();
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == game.LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.HostTeamId);
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.GuestTeamId);
            return View(game);
        }

        private Game PrepareGame(Game game, FormCollection form)
        {
            var HostTeam = Unit.TeamRepository.GetById(game.HostTeamId);
            var League = Unit.LeagueRepository.GetById(game.LeagueId);
            
            if (game.GuestTeam == null)
            {
                var GuestTeam = Unit.TeamRepository.GetById(game.GuestTeamId);
                game.GuestTeam = GuestTeam.Name;
            }
            else
            {
                game.GuestTeamId = 0;
            }

            DateTime dtDate;
            game.LeagueName = League.Name;
            game.Score = String.IsNullOrEmpty(game.Score) ? "brak" : game.Score;
            game.HostTeam = HostTeam.Name;

            if (String.IsNullOrEmpty(form["Venue"].Trim()))
            {
                game.Venue = HostTeam.Venue;
            }
            DateTime.TryParse(String.Format("{0} {1}", form["dtDate"], form["dtTime"]), out dtDate);
            game.DateAndTime = dtDate;
            game.NominationCreated = false;
            if (game.Id > 0)
            {
                var NominationExists = Unit.NominationRepository.Get(filter: n => n.GameId == game.Id).Count() == 1 ? true : false;
                game.NominationCreated = NominationExists;
            }
            return game;
        }
        /// <summary>
        /// Gets all referees from Nomination Commitee
        /// </summary>
        /// <returns>List of RefereeEntities</returns>
        private IEnumerable<RefereeEntity> GetRefereesFromRO() 
        {
            string[] UsersFromRO = Roles.GetUsersInRole(HelperRoles.RefereatObsad);
            IEnumerable<RefereeEntity> RO = Unit.RefereeRepository.Get(r => UsersFromRO.Contains(r.Mailadr));
            return RO;
        }
        
        //
        // GET: /Game/Edit/5
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Edit(int id)
        {
            Game game = Unit.GameRepository.GetById(id);
            ViewBag.SeasonId = game.SeasonId;
            ViewBag.LeagueId = game.LeagueId;
            ViewBag.dtDate = game.DateAndTime.ToShortDateString();
            ViewBag.dtTime = game.DateAndTime.ToShortTimeString();
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == game.LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.HostTeamId);
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.GuestTeamId);
            if (game.GuestTeamId != 0)
            {
                game.GuestTeam = "";
            }
            return View(game);
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Edit(Game game, FormCollection form)
        {
            game = PrepareGame(game, form);
            ModelState.Remove("HostTeam");
            ModelState.Remove("GuestTeam");
            if (ModelState.IsValid)
            {
                Unit.GameRepository.Update(game);
                Unit.Save();
                return RedirectToAction("Index", new { LeagueId = game.LeagueId });
            }
            
            ViewBag.SeasonId = game.SeasonId;
            ViewBag.LeagueId = game.LeagueId;
            ViewBag.dtDate = game.DateAndTime.ToShortDateString();
            ViewBag.dtTime = game.DateAndTime.ToShortTimeString();
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == game.LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.HostTeamId);
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name", game.GuestTeamId);
            return View(game);
        }

        //
        // GET: /Game/Delete/5
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult Delete(int id)
        {
            return PartialView(Unit.GameRepository.GetById(id));
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = HelperRoles.WydzialGieriEwidencji)]
        public ActionResult DeleteConfirmed(int id)
        {
            var nomination = Unit.NominationRepository.Get(filter: n => n.GameId == id);
            if (nomination != null && nomination.Count() > 0)
            {
                foreach (var n in nomination)
                {
                    Unit.NominationRepository.Delete(n);
                }
            }
            Game game = Unit.GameRepository.GetById(id);            
            int LeagueId = game.LeagueId;
            Unit.GameRepository.Delete(game);
            Unit.Save();
            return RedirectToAction("Index", new { LeagueId = LeagueId });
        }

        [Authorize(Roles=HelperRoles.RefereatObsad)]
        public JsonResult AddVol(int EventId, string Type, int Amount)
        {
            var Message = new { @Message = "", @Error = 0 };
            Voluntary vol = new Voluntary();
            if (Type == "tournament")
            {
                var Tour = Unit.TournamentRepository.GetById(EventId);
                if (Tour != null)
                {
                    vol.TournamentId = EventId;
                    vol.Active = true;
                    vol.AmountOfReferees = Amount;
                    vol.Code = vol.GetCode();
                }
            }
            else
            {
                var Game = Unit.GameRepository.GetById(EventId);
                if (Game != null)
                {
                    vol.GameId = EventId;
                    vol.Active = true;
                    vol.AmountOfReferees = Amount;
                    vol.Code = vol.GetCode();
                }
            }
            try
            {
                if (vol.Code != null)
                {
                    Unit.VoluntaryRepository.Insert(vol);
                    Unit.Save();
                    Message = new { Message = "Dodano poprawnie", Error = 0 };
                }
                else
                {
                    Message = new { Message = "Brak meczu lut turnieju poprawnie", Error = 1 };
                }
            }
            catch (Exception e)
            {
                Message = new { Message = e.Message, Error = 0 };
            }
            return Json(Message);
        }


        [HttpGet]
        public PartialViewResult SaveScore(int Id)
        {
            ViewBag.GameId = Id;
            Game Game = Unit.GameRepository.GetById(Id);
            ViewBag.GameTitle = String.Format("{0} - {1}", Game.HostTeam, Game.GuestTeam);
            return PartialView();
        }

        [HttpPost]
        public ActionResult SaveScore(int GameId, string Score)
        {
            Game Game = Unit.GameRepository.GetById(GameId);
            if (Game != null)
            {
                Game.Score = Score;
                Unit.GameRepository.Update(Game);
                Unit.Save();
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}