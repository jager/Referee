using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.DAL;
using Referee.Models;

namespace Referee.Controllers.Base
{
    public class BaseController : Controller
    {
        protected UOW Unit = new UOW();
        protected Season CurrentSeason;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            SetCurrentSeason(); 
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
    }
}
