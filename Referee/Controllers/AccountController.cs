using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Referee.Models;
using Referee.Controllers.Base;
using Referee.Helpers;
using Referee.Models.Domain;
using Referee.Lib.Security;

namespace Referee.Controllers
{
    public class AccountController : BaseController
    {
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
                new BreadcrumbHelper { Href = "/Referee", Text = "Profil" }
            };
        }
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public JsonResult GetPhoto(string u)
        {
            var Referee = Unit.RefereeRepository.Get(filter: r => r.Mailadr == u.Trim()).FirstOrDefault<RefereeEntity>();
            var User = new UserDomain(Referee);
            return Json(User.GetPhotoAndFullName());
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        /*
        public ActionResult Register()
        {
            
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie );
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        */        

    
        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    //var p = currentUser.ResetPassword();
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public PartialViewResult RemindPassword(Guid id)
        {
            var referee = Unit.RefereeRepository.GetById(id);

            if (referee == null)
            {
                ViewBag.Message = "Podany sędzia nie istnieje.";
            }
            else
            {
                var securityUser = Membership.GetUser(referee.Mailadr);
                //var Password = SetPassword(referee);
                var Password = securityUser.ResetPassword();

                if (this.GetConfigValue("SendEmails") == "1" && this.GetConfigValue("SendRemindPasswordEmail") == "1")
                {
                    MailHelper.RemindPasswordMessage(referee.Mailadr, Password);

                    ViewBag.Message = "Hasło zostało wysłane na adres mailowy sędziego.";

                    if (MailHelper.ErrorMessage != MailHelper._success)
                    {
                        ViewBag.Message = MailHelper.ErrorMessage;
                    }
                }
                else
                {
                    ViewBag.Message = String.Format("Nowe hasło dla tego sędziego to: {0}.", Password);
                }
            }

            return PartialView();
        }

        [HttpGet]
        public ViewResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Mailadr)
        {
            if (String.IsNullOrEmpty(Mailadr))
            {
                ViewBag.Message = "Pusty adres mailowy";
                return View();
            }
            var securityUser = Membership.GetUser(Mailadr);
            if (securityUser == null)
            {
                ViewBag.Message = "Użytkownik o podanym adresie nie istnieje w systemie.";
                return View();
            }

            string Token = Unit.ChangePasswordRepository.Create(Convert.ToString(securityUser.ProviderUserKey));
            Unit.Save();

            if (!String.IsNullOrEmpty(Token))
            {
                if (this.GetConfigValue("SendEmails") == "1")
                {
                    MailHelper.RestorePasswordMessage(Mailadr, Token);

                    ViewBag.Message = "Hasło zostało wysłane na adres mailowy sędziego.";

                    if (MailHelper.ErrorMessage != MailHelper._success)
                    {
                        ViewBag.Message = MailHelper.ErrorMessage;
                    }
                }
                else
                {
                    ViewBag.Message = "Wysyłanie maili zostało zablokowane. Skontaktuj sie z administratorem w celu odblokowania możliwości wysyłania maili.";
                }
            }
            else
            {
                ViewBag.Message = "Błąd podczas generowania tokenu. Zgłoś się proszę do administratora systemu.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult RestorePassword(string Token)
        {
            if (String.IsNullOrEmpty(Token))
            {
                return RedirectToAction("ForgotPassword");
            }            
            ViewBag.Token = Token;

            return View();
        }

        [HttpPost]
        public ActionResult RestorePassword(string Token, string NewPassword, string NewPasswordRepeated)
        {
            try
            {
                if (Unit.ChangePasswordRepository.Check(Token))
                {
                    var Password = Unit.ChangePasswordRepository.Password;
                    var User = Membership.GetUser(Guid.Parse(Password.UserId));
                    if (User == null)
                    {
                        return RedirectToAction("ForgotPassword");
                    }
                    
                    if (User.IsLockedOut)
                    {
                        User.UnlockUser();
                    }
                    
                    string NewTemporaryPassword = User.ResetPassword();
                    if (NewPassword.Trim() == NewPasswordRepeated.Trim()
                        && User.ChangePassword(NewTemporaryPassword, NewPassword.Trim()))
                    {
                        Unit.ChangePasswordRepository.Delete(Password);
                        Unit.Save();
                        return RedirectToAction("LogOn");
                    }
                }
            }
            catch (Exception e)
            {      
                return RedirectToAction("ForgotPassword");
            }
            return View();
        }


        [Authorize]
        public ActionResult Edit(Guid id)
        {
            if (id != CurrentReferee.Id)
            {
                return RedirectToAction("Profile", "Home");
            }
            RefereeEntity refereeentity = Unit.RefereeRepository.GetById(id);
            ViewData["PageTitle"] = String.Format("Edycja danych sędziego: {0} {1}", refereeentity.FirstName, refereeentity.LastName);

            ((List<BreadcrumbHelper>)ViewData["breadcrumbs"]).Add(
                new BreadcrumbHelper { Href = "#", Text = refereeentity.FirstName + " " + refereeentity.LastName }
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
        
        [HttpPost]
        [Authorize]
        public ActionResult Edit(RefereeEntity refereeentity, FormCollection form)
        {
            if (refereeentity.Id != CurrentReferee.Id)
            {
                return RedirectToAction("Profile", "Home");
            }
            DateTime dtDOB;
            DateTime.TryParse(String.Format("{0}-{1}-{2}", form["DOBYear"], form["DOBmonth"], form["DOBday"]), out dtDOB);
            refereeentity.DOB = dtDOB;
            string[] selectedRoles = Roles.GetRolesForUser(refereeentity.Mailadr);
            string Password = HashString.SHA1(String.Format("{0}{1}", refereeentity.Mailadr, DateTime.Now.ToUniversalTime().ToLongDateString())).Substring(0, 8);
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
                    return RedirectToAction("Profile", "Home");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
            }
            PopulateDropDowns(refereeentity);
            return View(refereeentity);
        }


        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }




        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
