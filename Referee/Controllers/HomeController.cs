using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Referee.Controllers.Base;
using Referee.Models;
using Referee.Helpers;
using Referee.ViewModels;

namespace Referee.Controllers
{
    public class HomeController : BaseController
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
            ViewData["PageTitle"] = "System Obsad MWZPS";

            //Pobieram Turnieje nieligowe
            ViewBag.Tournaments = Unit.TournamentRepository.Get(
                    filter: t => t.SeasonId == CurrentSeason.Id &&
                                 t.Type == "Random" &&
                                 t.StartDate > DateTime.Now,
                   orderBy: to => to.OrderByDescending(o => o.StartDate)
                   );
            

            //Pobieram ligi
            ViewBag.Leagues = Unit.LeagueRepository.Get(
                filter: l => l.Id < 200);



            //Pobieram mecze i turnieje na zapisy
            var Voluntaries = Unit.VoluntaryRepository.Get(
                filter: v => v.Active,
                orderBy: v => v.OrderByDescending(o => o.Id)
                );
            if (Voluntaries != null && Voluntaries.Count() > 0)
            {
                List<int> GameIDS = new List<int>();
                List<int> TourIDS = new List<int>();
                foreach (var vol in Voluntaries)
                {
                    if (vol.GameId != null)
                    {
                        GameIDS.Add((int)vol.GameId);
                    }
                    else if (vol.TournamentId != null)
                    {
                        TourIDS.Add((int)vol.TournamentId);
                    }
                }

                if (GameIDS.Count() > 0)
                {
                    ViewBag.VoluntaryGames = Unit.GameRepository.Get(filter: g => GameIDS.Contains(g.Id));
                }

                if (TourIDS.Count() > 0)
                {
                    ViewBag.VoluntaryTours = Unit.TournamentRepository.Get(filter: t => TourIDS.Contains(t.Id));
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult Profile()
        {
            if (this.CurrentUser != null)
            {
                string UserName = CurrentUser.UserName;
                RefereeEntity refereeentity = Unit.RefereeRepository.Get(filter: r => r.Mailadr == UserName).FirstOrDefault();
                if (refereeentity == null)
                {
                    return RedirectToAction("Index");
                }
                ViewData["PageTitle"] = String.Format("Profil: {0} {1}", refereeentity.FirstName, refereeentity.LastName);
                ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                    new BreadcrumbHelper { Href = "#", Text = refereeentity.FirstName + " " + refereeentity.LastName }
                );
              
                var NominatedReferees = Unit.NominatedRepository.Get(filter: n => n.RefereeId == refereeentity.Id); //GNRepository.Get();
                List<int> RefNominationsIDS = new List<int>();
                foreach (var nominated in NominatedReferees)
                {
                    RefNominationsIDS.Add(nominated.NominationId);
                }
                var Nominations = Unit.NominationRepository.Get(n => n.Published && RefNominationsIDS.Contains(n.Id), n => n.OrderByDescending(o => o.PublishDate));
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
                    NominationEvents.Add(new NominationDetails { Event = _event, Nomination = _nomination, NominatedReferees = _nomination.Nominateds });
                }
                ViewBag.Games = NominationEvents;
                return View("../Referee/Details", refereeentity);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult NewNominations(string dtStart = "", string dtEnd = "", int league = 0)
        {
            ViewData["PageTitle"] = "Twoje nowe nominacje";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Nowe nominacje" }
            );
            if (CurrentReferee != null)
            {
                var NominatedReferees = Unit.NominatedRepository.Get(filter: n => n.RefereeId == CurrentReferee.Id && !n.Confirmed); //GNRepository.Get();
                List<int> RefNominationsIDS = new List<int>();
                foreach (var nominated in NominatedReferees)
                {
                    RefNominationsIDS.Add(nominated.NominationId);
                }
                var Nominations = Unit.NominationRepository.Get(n => n.Published && RefNominationsIDS.Contains(n.Id), n => n.OrderByDescending(o => o.PublishDate));

                FillSearchNominationsForm(Nominations, dtStart, dtEnd, league);
                
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
                    NominationEvents.Add(new NominationDetails { Event = _event, Nomination = _nomination, NominatedReferees = _nomination.Nominateds });
                    
                }
                ViewBag.Games = NominationEvents;
                return View("ListNominations", NominationEvents);
            }
            else
            {
                return RedirectToAction("Index");
            }            
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult CalendarEvents(int start, int end)
        {
            List<CalendarEvent> Events = new List<CalendarEvent>();
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            DateTime stDate = dt.AddSeconds(start).ToLocalTime();
            DateTime enDate = dt.AddSeconds(end).ToLocalTime();

            var Nominations = Unit.NominationRepository.Get(filter: 
                    n => n.Published && ((n.TournamentId != null && n.Tournament.StartDate > stDate && n.Tournament.StartDate < enDate)
                        || (n.GameId != null && n.Game.DateAndTime > stDate && n.Game.DateAndTime < enDate))
                    );
            foreach (var Nomination in Nominations)
            {
                if (Nomination.GameId != null)
                {
                    Events.Add(
                        new CalendarEvent()
                        {
                            title = Nomination.Game.Name,
                            start = Nomination.Game.DateAndTime.ToString(),
                            end = Nomination.Game.DateAndTime.AddHours(1.30).ToString()
                        }
                        );
                }
                else
                {

                    var TournamentEvent = new CalendarEvent()
                        {
                            title = Nomination.Tournament.Name,
                            start = (Nomination.Tournament.StartDate - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds.ToString()
                        };
                    if (Nomination.Tournament.EndDate != null)
                    {
                        TournamentEvent.end = (Nomination.Tournament.EndDate - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds.ToString();
                    }
                    else
                    {
                        TournamentEvent.allDay = true;
                    }
                    Events.Add(TournamentEvent);
                }
            }

            return Json(Events, JsonRequestBehavior.AllowGet);
        }


    }
}
