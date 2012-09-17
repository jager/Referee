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
        public ActionResult NewNominations()
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


    }
}
