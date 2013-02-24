using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Models;
using Referee.Controllers.Base;
using Referee.Lib;
using Referee.Lib.Security;
using Referee.Lib.Photo;
using Referee.Helpers;
using Referee.Repositories;
using Referee.ViewModels;
using System.Web.Security;
using System.Diagnostics;

namespace Referee.Controllers
{ 
    public class RefereeController : BaseController
    {

        //
        // GET: /Referee/

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ViewBag.DOBYear = 0;
            ViewBag.DOBmonth = 0;
            ViewBag.DOBday = 0;
            ViewData["IconName"] = "icon-user-2";
            ViewData["breadcrumbs"] = new List<BreadcrumbHelper>
            {
                new BreadcrumbHelper { Href = "/", Text = "Pulpit" },
                new BreadcrumbHelper { Href = "/Referee", Text = "Sędziowie" }
            };
        }

        public ViewResult Index()
        {
            ViewData["PageTitle"] = "Sędziowie MWZPS";
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Referee/Create", Text = "Dodaj sędziego", Role = HelperRoles.RefereatObsad }
            };
            return View(Unit.RefereeRepository.Get(orderBy: l => l.OrderBy(r => r.LastName)));
        }

        //
        // GET: /Referee/Details/5
        [Authorize(Roles=HelperRoles.Sedzia)]
        public ViewResult Details(Guid id)
        {
            RefereeEntity refereeentity = Unit.RefereeRepository.GetById(id);
            ViewData["PageTitle"] = String.Format("Dane sędziego: {0} {1}", refereeentity.FirstName, refereeentity.LastName);
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = refereeentity.FirstName + " " + refereeentity.LastName }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Referee", Text = "Listuj sędziów" },
                new BreadcrumbHelper { Href = "/Referee/Create", Text = "Dodaj sędziego", Role = HelperRoles.RefereatObsad }
            };
            //GamesNominatedRepository GNRepository = new GamesNominatedRepository(id);
            var NominatedReferees = Unit.NominatedRepository.Get(filter: n => n.RefereeId == id); //GNRepository.Get();
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
            return View(refereeentity);
        }

        //
        // GET: /Referee/Create
        //[Authorize(Roles=HelperRoles.RefereatObsad)]
        public ActionResult Create()
        {
            ViewData["PageTitle"] = "Dodaj nowego sędziego";
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = "Nowy sędzia" }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Referee", Text = "Listuj sędziów" }
            };
            PopulateDropDowns();
            return View();
        } 

        //
        // POST: /Referee/Create

        [HttpPost]
        //[Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Create(RefereeEntity refereeentity, FormCollection form, HttpPostedFileBase Photo)
        {
            Guid NewUserGuid = Guid.Empty;

            string[] selectedRoles = new string[] { };
            if (!String.IsNullOrEmpty(Request.Form["Roles"]))
            {
                selectedRoles = Request.Form["Roles"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            string Password = SetPassword(refereeentity);
            ///Trzeba to zmienić w wersji docelowej.
            //Password = "qawseD123";
            try {
                if (ModelState.IsValid)
                {
                    CreateUser(refereeentity.Mailadr, Password, Password, out NewUserGuid);
                    AssignRole(refereeentity.Mailadr, selectedRoles);
                    refereeentity.Id = NewUserGuid;
                    DateTime dtDOB;
                    DateTime.TryParse(String.Format("{0}-{1}-{2}", form["DOBYear"], form["DOBmonth"], form["DOBday"]), out dtDOB);
                    refereeentity.DOB = dtDOB;
                    ///Upload Referee Photo               
                    if (Photo != null)
                    {
                        refereeentity.DestinationFolder = String.Format("{0}{1}/", FileUploader.DestinationFolder, HashString.SHA1(refereeentity.Id.ToString()));
                        refereeentity.Photo = UploadRefereePhoto(Photo, refereeentity.DestinationFolder);
                    }
                    else
                    {
                        refereeentity.DestinationFolder = "";
                        refereeentity.Photo = "";
                    }

                    Unit.RefereeRepository.Insert(refereeentity);
                    Unit.Save();
                    if (this.GetConfigValue("SendEmails") == "1" && this.GetConfigValue("SendNewAccountEmail") == "1")
                    {
                        MailHelper.CreateNewAccountMessage(refereeentity.Mailadr, Password);
                        if (MailHelper.ErrorMessage != MailHelper._success)
                        {
                            ///TODO: logowanie błędów
                        }
                    }
                    return RedirectToAction("Index");
                }
            } 
            catch(Exception e)
            {
                if (NewUserGuid != Guid.Empty)
                {
                    Membership.DeleteUser(refereeentity.Mailadr);
                }
                ModelState.AddModelError(String.Empty, e.Message);
            }
      
            PopulateDropDowns(refereeentity);
            return View(refereeentity);
        }

        private void CreateUser(string Mailadr, string Password, string PasswordConfirmed, out Guid UserID )
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

        private bool CreateUser(string Mailadr, string Password)
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

        private string UploadRefereePhoto(HttpPostedFileBase Photo, string DestinationFolder)
        {
            string PhotoName = "";
            FileUploader fu = new FileUploader();            
            fu.Folder = DestinationFolder;
            fu.File = Photo;
            PhotoName = fu.Run();
            string PathToPhoto = String.Format("{0}{1}", DestinationFolder, PhotoName);            

            if (!CheckPhoto.isImage(PathToPhoto))
            {
                System.IO.File.Delete(PathToPhoto);
                PhotoName = "";
            }
            else
            {
                ConvertUserPhoto cup = new ConvertUserPhoto(DestinationFolder, PhotoName);
                cup.Run(110, 108);
                PhotoName = cup.PhotoName;
            }
            return PhotoName;
        }

        private void AssignRole(string UserName, string[] SelectedRoles)
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

        [HttpPost]
        [Authorize(Roles = HelperRoles.Sedzia)]
        public ActionResult ChangePhoto(Guid Id, HttpPostedFileBase Photo)
        {
            var Referee = Unit.RefereeRepository.GetById(Id);
            if (String.IsNullOrEmpty(Referee.DestinationFolder))
            {
                Referee.DestinationFolder = String.Format("{0}{1}/", FileUploader.DestinationFolder, HashString.SHA1(DateTime.Now.ToString()));
            }
            ///Upload Referee Photo
            Referee.Photo = UploadRefereePhoto(Photo, Referee.DestinationFolder);
            
            Unit.RefereeRepository.Update(Referee);
            Unit.Save();
            return RedirectToAction("Details", new { Id = Id });
        }


        private void PopulateDropDowns(RefereeEntity refereeentity = null)
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
        //
        // GET: /Referee/Edit/5
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Edit(Guid id)
        {
            RefereeEntity refereeentity = Unit.RefereeRepository.GetById(id);
            ViewData["PageTitle"] = String.Format("Edycja danych sędziego: {0} {1}", refereeentity.FirstName, refereeentity.LastName);
            
            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href= "#", Text = refereeentity.FirstName + " " + refereeentity.LastName }
            );
            ViewData["breadlinks"] = new List<BreadcrumbHelper> 
            { 
                new BreadcrumbHelper { Href = "/Referee", Text = "Listuj sędziów" },
                new BreadcrumbHelper { Href = "/Referee/Create", Text = "Dodaj sędziego" }
            };
            ViewBag.DOBYear = refereeentity.DOB.Year;
            ViewBag.DOBmonth = refereeentity.DOB.Month;
            ViewBag.DOBday = refereeentity.DOB.Day;
            
            PopulateDropDowns(refereeentity);
            return View(refereeentity);
        }

        //
        // POST: /Referee/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Edit(RefereeEntity refereeentity, FormCollection form)
        {
            DateTime dtDOB;
            DateTime.TryParse(String.Format("{0}-{1}-{2}", form["DOBYear"], form["DOBmonth"], form["DOBday"]), out dtDOB);
            refereeentity.DOB = dtDOB;
            string[] selectedRoles = new string[] { };
            if (!String.IsNullOrEmpty(Request.Form["Roles"]))
            {
                selectedRoles = Request.Form["Roles"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            
            string Password = HashString.SHA1(String.Format("{0}{1}", refereeentity.Mailadr, DateTime.Now.ToUniversalTime().ToLongDateString())).Substring(0, 8);
            ///Trzeba to zmienić w wersji docelowej.
            //Password = "qawseD123";
            AddCredentialsIfNotExists(refereeentity.Mailadr, Password);
            try
            {
                if (ModelState.IsValid)
                {
                    AssignRole(refereeentity.Mailadr, selectedRoles);
                    if (refereeentity.Id == CurrentReferee.Id)
                    {
                        Unit.RefereeRepository.UpdateProfile(refereeentity, CurrentReferee);
                    }
                    else
                    {
                        Unit.RefereeRepository.Update(refereeentity);
                    }
                    Unit.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }
            PopulateDropDowns(refereeentity);
            return View(refereeentity);
        }

        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult ChangeMailadr(Guid id)
        {
            var Referee = Unit.RefereeRepository.GetById(id);
            return PartialView(Referee);
        }

        [HttpPost]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult ChangeMailadr(FormCollection form)
        {
            Guid RId = Guid.Empty;
            Guid.TryParse(form["Id"], out RId);
            string Mailadr = form["mailadr"];  
            string OldMailadr = String.Empty;
            if (RId != Guid.Empty && Mailadr != null)
            {
                var Referee = Unit.RefereeRepository.GetById(RId);
                OldMailadr = Referee.Mailadr;
                if (Referee != null && Unit.RefereeRepository.UpdateUserNameAndEmail(Referee.Mailadr, Mailadr))
                {
                    Referee.Mailadr = Mailadr;                       
                    Unit.RefereeRepository.Update(Referee);
                    Unit.Save();   
                }
                
                
            }
            if (User.Identity.Name == OldMailadr)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Details", new { id = RId });
        }
        //
        // GET: /Referee/Delete/5
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult Delete(Guid id)
        {
            RefereeEntity refereeentity = Unit.RefereeRepository.GetById(id);
            return PartialView(refereeentity);
        }

        //
        // POST: /Referee/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = HelperRoles.RefereatObsad)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var referee = Unit.RefereeRepository.GetById(id);
            Membership.DeleteUser(referee.Mailadr);
            Unit.RefereeRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Available(string date, string time)
        {
            DateTime eventDate = System.Convert.ToDateTime(String.Format("{0} {1}", date, time));
            var av = Unit.AvailabilityRepository.Get(filter: a => a.DateStart < eventDate && a.DateEnd > eventDate);
            if (av.Count() > 0)
            {
            }
            var r = Unit.RefereeRepository.Get();
            //var r1 = from r2 in r where r => r.Availabilities.Except(av);
            
            return View("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }

        private bool CheckUserExists(string Mailadr)
        {
            return (Unit.RefereeRepository.Get(filter: r => r.Mailadr == Mailadr).Count() > 0 ? true : false);            
        }

        private void AddCredentialsIfNotExists(string Mailadr, string Password)
        {
            var NewUser = Membership.GetUser(Mailadr);
            if (NewUser == null)
            {
                CreateUser(Mailadr, Password);
            }
        }
    }
}