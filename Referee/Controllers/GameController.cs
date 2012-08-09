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

namespace Referee.Controllers
{ 
    public class GameController : BaseController
    {
        private RefereeContext db = new RefereeContext();

        //
        // GET: /Game/

        public ActionResult Index(int? LeagueId)
        {
            var ListOfLeagues = Unit.LeagueRepository.Get();
            
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

            return View(Unit.GameRepository.Get(
                g => g.SeasonId == CurrentSeason.Id && g.LeagueId == LeagueId,
                g => g.OrderBy(o => o.DateAndTime)));
        }


        public ViewResult IndexTournament(int? LeagueId)
        {
            var ListOfLeagues = Unit.LeagueRepository.Get();

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
            return View(Unit.TournamentRepository.Get(
                t => t.SeasonId == CurrentSeason.Id && t.LeagueId == LeagueId,
                t => t.OrderBy(o => o.StartDate)));
        }

        //
        // GET: /Game/Details/5

        public ViewResult Details(Guid id)
        {
            return View(Unit.GameRepository.GetById(id));
        }

        //
        // GET: /Game/Create

        public ActionResult Create(int LeagueId)
        {
            ViewBag.SeasonId = CurrentSeason.Id;
            ViewBag.LeagueId = LeagueId;
            ViewBag.dtDate = "";
            ViewBag.dtTime = "08:00";
            var teams = Unit.EnrollmentRepository.Get(e => e.LeagueId == LeagueId, e => e.OrderBy(o => o.TeamId), "Team");
            ViewBag.HostTeamsId = new SelectList(teams, "TeamId", "Team.Name");
            ViewBag.GuestTeamsId = new SelectList(teams, "TeamId", "Team.Name");
            return View();
        } 

        //
        // POST: /Game/Create

        [HttpPost]
        public ActionResult Create(Game game, FormCollection form)
        {
            game = PrepareGame(game, form);
            ModelState.Remove("HostTeam");
            ModelState.Remove("GuestTeam");
            if (ModelState.IsValid)
            {
                Unit.GameRepository.Insert(game);
                Unit.Save();
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
        public ActionResult CreateAndNominate(Game game, FormCollection form)
        {
            game = PrepareGame(game, form);
            ModelState.Remove("HostTeam");
            ModelState.Remove("GuestTeam");
            if (ModelState.IsValid)
            {
                Unit.GameRepository.Insert(game);
                Unit.Save();
                return RedirectToAction("Create", "Nomination", new { GameId = game.Id });
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
            
            game.Venue = HostTeam.Venue;
            DateTime.TryParse(String.Format("{0} {1}", form["dtDate"], form["dtTime"]), out dtDate);
            game.DateAndTime = dtDate;
            return game;
        }
        
        //
        // GET: /Game/Edit/5
 
        public ActionResult Edit(Guid id)
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
 
        public ActionResult Delete(Guid id)
        {
            return PartialView(Unit.GameRepository.GetById(id));
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Game game = Unit.GameRepository.GetById(id);
            int LeagueId = game.LeagueId;
            Unit.GameRepository.Delete(game);
            Unit.Save();
            return RedirectToAction("Index", new { LeagueId = LeagueId });
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}