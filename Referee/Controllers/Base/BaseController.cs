using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.DAL;
using Referee.Models;
using System.Web.Security;

namespace Referee.Controllers.Base
{
    public class BaseController : Controller
    {
        protected UOW Unit = new UOW();
        protected Season CurrentSeason;
        protected MembershipUser CurrentUser = null;
        protected RefereeEntity CurrentReferee = null;
        protected int NewNominationsAmount = 0;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            SetCurrentSeason();
            /*
            string[] roles = { "Administrator", "Sędzia", "WGiE", "RO" };
            string[] ExistingRoles = Roles.GetAllRoles();
            if (ExistingRoles.Count() < roles.Count()) 
            {
                foreach (var r in roles)
                {
                    if (!Roles.RoleExists(r))
                    {
                        Roles.CreateRole(r);
                    }
                }
            }
            else if (ExistingRoles.Count() > roles.Count())
            {
                foreach (var r in ExistingRoles)
                {
                    if (!roles.Contains(r))
                    {
                        Roles.DeleteRole(r);
                    }
                }
            }
            */
            
            if (Request.IsAuthenticated)
            {
                CurrentUser = Membership.GetUser(User.Identity.Name);
                CurrentReferee = Unit.RefereeRepository.Get(filter: r => r.Mailadr == User.Identity.Name).FirstOrDefault();
                CheckUnconfirmedNominationsAmount();
                ViewBag.NewNominationsAmount = NewNominationsAmount;
                ViewBag.CurrentReferee = CurrentReferee;
            }
        }

        /// <summary>
        /// Sets current season and stores in session.
        /// </summary>
        /// <param name="check">Boolean. Default value = false.</param>
        protected void SetCurrentSeason(bool check = false)
        {
            if (Session["CurrentSeason"] != null)
            {
                CurrentSeason = (Season)Session["CurrentSeason"];
            }
            else
            {
                check = true;
            }

            if (check)
            {
                CurrentSeason = Unit.SeasonRepository.Get(filter: season => season.Active).FirstOrDefault<Season>();
                Session.Add("CurrentSeason", CurrentSeason);
            }
        }

        /// <summary>
        /// Pokazuje błędy w model state
        /// </summary>
        protected void DebugModelState()
        {
            foreach (var obj in ModelState.Values)
            {
                foreach (var error in obj.Errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                        System.Diagnostics.Debug.WriteLine("MODEL STATE ERROR = " + error.ErrorMessage);
                }
            } 
        }

        protected void CheckUnconfirmedNominationsAmount()
        {
            if (CurrentReferee != null)
            {
                var NewNominations = Unit.NominatedRepository.Get(filter: n => n.RefereeId == CurrentReferee.Id);
                if (NewNominations != null)
                {
                    NewNominationsAmount = NewNominations
                        .Where(o => o.Nomination.Published && !o.Nomination.Confirmed)
                        .Count();
                }
            }
                           
        }
    }
}
