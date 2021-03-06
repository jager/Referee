﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.DAL;
using Referee.Models;
using System.Web.Security;
using Referee.Filters;
using Referee.Lib.Security;
using Referee.Helpers;

namespace Referee.Controllers.Base
{
    [LogActionFilter]
    public class BaseController : Controller
    {
        protected UOW Unit = new UOW();
        protected Season CurrentSeason;
        protected MembershipUser CurrentUser = null;
        protected RefereeEntity CurrentReferee = null;
        protected int NewNominationsAmount = 0;
        protected IEnumerable<AppConfig> Configuration;
        protected ConfigHelper CHelper;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            GetConfiguration();
            SetCurrentSeason();
           
            string[] roles = { HelperRoles.Administrator, HelperRoles.RefereatObsad, HelperRoles.Sedzia, HelperRoles.WydzialGieriEwidencji };
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
                var NewNominations = Unit.NominatedRepository.Get(filter: n => n.RefereeId == CurrentReferee.Id && !n.Confirmed && n.Nomination.Published);
                if (NewNominations != null)
                {
                    NewNominationsAmount = NewNominations.Count();
                }
            }
                           
        }

        protected string SetPassword(RefereeEntity refereeentity)
        {
            return HashString.SHA1(String.Format("{0}{1}", refereeentity.Mailadr, DateTime.Now.ToUniversalTime().ToLongDateString())).Substring(0, 8);
        }

        [OutputCache]
        protected void GetConfiguration()
        {            
            this.Configuration = Unit.ConfigRepository.Get();            
        }

        protected string GetConfigValue(string key) 
        {
            if (this.Configuration != null)
            {
                var Config = this.Configuration.Where(c => c.Key == key).FirstOrDefault();
                return Config.Value;
            }
            return String.Empty;
        }


        /// <summary>
        /// Prepares data to search form used in HomeController and NominationController
        /// </summary>
        /// <param name="Nominations">List of nominations</param>
        /// <param name="dtStart">Search start date</param>
        /// <param name="dtEnd">Search End date</param>
        /// <param name="league">League name where nominations are search</param>
        protected IEnumerable<Nomination> FillSearchNominationsForm(IEnumerable<Nomination> Nominations, string dtStart = "", string dtEnd = "", string league = "0", bool Published = false, bool NotPublished = false)
        {
            
            DateTime DateStart;
            DateTime DateEnd;

            if (!String.IsNullOrEmpty(dtEnd))
            {
                dtEnd += " 23:59:59";
            }

            ViewBag.Published = Published;
            ViewBag.NotPublished = NotPublished;

            if (Published != NotPublished)
            {
                if (Published)
                {
                    Nominations = Nominations.Where(n => n.Published);
                }
                else
                {
                    Nominations = Nominations.Where(n => !n.Published);
                }
            }


            if (!String.IsNullOrEmpty(dtStart) && DateTime.TryParse(Convert.ToString(dtStart), out DateStart))
            {
                Nominations = Nominations.Where(n => (n.Game != null && n.Game.DateAndTime >= DateStart)
                                        || (n.Tournament != null && n.Tournament.StartDate >= DateStart));
                ViewBag.dtStart = dtStart;
            }

            if (!String.IsNullOrEmpty(dtEnd) && DateTime.TryParse(Convert.ToString(dtEnd), out DateEnd))
            {
                Nominations = Nominations.Where(n => (n.Game != null && n.Game.DateAndTime <= DateEnd)
                                        || (n.Tournament != null && n.Tournament.StartDate <= DateEnd));
                ViewBag.dtEnd = dtEnd;

            }
            string[] StringLeagues = new string[] { };
            if (!String.IsNullOrEmpty(Request.Form["league"]))
            {
                StringLeagues = Request.Form["league"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            int[] Leagues = StringLeagues.Select(i => Int32.Parse(i)).ToArray();

            if (Leagues.Count() == 1 && Leagues.First() == 0)
            {
                Leagues = new int[] { };
            }

            ViewBag.Leagues = Unit.LeagueRepository.Get(filter: l => l.Visible); 
            ViewBag.SelectedLeagues = Leagues;
  
            if (Leagues.Count() > 0)
            {
                Nominations = Nominations.Where(n => n.Game != null && Leagues.Contains(n.Game.LeagueId));
            }
            return Nominations;
        }


        /// <summary>
        /// Populates dropdowns inf create and edit referee form
        /// </summary>
        /// <param name="refereeentity">Referee Entity object</param>
        protected void PopulateDropDowns(RefereeEntity refereeentity = null)
        {
            ViewBag.RefClassId = new SelectList(Unit.RClassRepository.Get(), "Id", "Name", refereeentity == null ? 0 : refereeentity.RefClassId);
            ViewBag.AuthorizationId = new SelectList(Unit.AuthorizationRepository.Get(), "Id", "Name", refereeentity == null ? 0 : refereeentity.AuthorizationId);
            ViewBag.UserRoles = new string[] { };
            if (refereeentity != null)
            {
                ViewBag.UserRoles = Roles.GetRolesForUser(refereeentity.Mailadr);
            }

            if (refereeentity != null)
            {
                ViewBag.DOBYear = refereeentity.DOB.Year;
                ViewBag.DOBmonth = refereeentity.DOB.Month;
                ViewBag.DOBday = refereeentity.DOB.Day;
            }
        }


        /// <summary>
        /// Assign role to referee entity
        /// </summary>
        /// <param name="UserName"</param>
        /// <param name="SelectedRoles">Roles that are selected to referee entity</param>
        protected void AssignRole(string UserName, string[] SelectedRoles)
        {
            if (SelectedRoles.Count() == 0)
            {
                throw new Exception("Nie wybrano żadnych ról dla sędziego.");
            }
            var User = Membership.GetUser(UserName);
            if (User != null)
            {
                var ExistingRoles = Roles.GetRolesForUser(UserName);
                if (ExistingRoles.Count() > 0)
                {
                    Roles.RemoveUserFromRoles(UserName, ExistingRoles);
                }
                Roles.AddUserToRoles(UserName, SelectedRoles);
            }
            else
            {
                throw new Exception("Nie można dodać ról do pustego użytkownika.");
            }
        }


        /// <summary>
        /// Creates credentials if they don't exist
        /// </summary>
        /// <param name="Mailadr"></param>
        /// <param name="Password"></param>
        protected void AddCredentialsIfNotExists(string Mailadr, string Password)
        {
            var NewUser = Membership.GetUser(Mailadr);
            if (NewUser == null)
            {
                CreateUser(Mailadr, Password);
            }
        }


        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="Mailadr"></param>
        /// <param name="Password"></param>
        /// <param name="PasswordConfirmed"></param>
        /// <param name="UserID"></param>
        protected void CreateUser(string Mailadr, string Password, string PasswordConfirmed, out Guid UserID)
        {
            UserID = Guid.Empty;
            RegisterModel NewUser = new RegisterModel { Email = Mailadr, Password = Password, ConfirmPassword = PasswordConfirmed, UserName = Mailadr };
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(NewUser.UserName, NewUser.Password, NewUser.Email, null, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    UserID = (Guid)Membership.GetUser(Mailadr).ProviderUserKey;
                }
                else
                {
                    throw new Exception("Użytkownik o podanym adresie mailowym już istnieje w bazie danych!");
                }
            }
            else
            {
                throw new Exception("Błąd podczas dodawania danych autoryzacyjnych sędziego");
            }
        }


        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="Mailadr"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        protected bool CreateUser(string Mailadr, string Password)
        {
            RegisterModel NewUser = new RegisterModel { Email = Mailadr, Password = Password, ConfirmPassword = Password, UserName = Mailadr };
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(NewUser.UserName, NewUser.Password, NewUser.Email, null, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
