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
                new BreadcrumbHelper { Href = "/Referee/Create", Text = "Dodaj sędziego" }
            };
            return View(Unit.RefereeRepository.Get(orderBy: l => l.OrderBy(r => r.LastName)));
        }

        //
        // GET: /Referee/Details/5

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
                new BreadcrumbHelper { Href = "/Referee/Create", Text = "Dodaj sędziego" }
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
        public ActionResult Create(RefereeEntity refereeentity, FormCollection form, HttpPostedFileBase Photo)
        {

            if (ModelState.IsValid)
            {                
                refereeentity.Id = Guid.NewGuid();
                DateTime dtDOB;
                DateTime.TryParse(String.Format("{0}-{1}-{2}", form["DOBYear"], form["DOBmonth"], form["DOBday"]), out dtDOB);
                refereeentity.DOB = dtDOB;
                ///Upload Referee Photo               
                refereeentity.DestinationFolder = String.Format("{0}{1}/", FileUploader.DestinationFolder, HashString.SHA1(refereeentity.Id.ToString() ));
                refereeentity.Photo = UploadRefereePhoto(Photo, refereeentity.DestinationFolder);
                
                Unit.RefereeRepository.Insert(refereeentity);
                Unit.Save();
                return RedirectToAction("Index");  
            }

            PopulateDropDowns(refereeentity);
            return View(refereeentity);
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

        [HttpPost]
        public ActionResult ChangePhoto(Guid Id, HttpPostedFileBase Photo)
        {
            var Referee = Unit.RefereeRepository.GetById(Id);
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
        }
        //
        // GET: /Referee/Edit/5
 
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
        public ActionResult Edit(RefereeEntity refereeentity, FormCollection form)
        {
            DateTime dtDOB;
            DateTime.TryParse(String.Format("{0}-{1}-{2}", form["DOBYear"], form["DOBmonth"], form["DOBday"]), out dtDOB);
            refereeentity.DOB = dtDOB;
            if (ModelState.IsValid)
            {
                Unit.RefereeRepository.Update(refereeentity);
                Unit.Save();
                return RedirectToAction("Index");
            }
            PopulateDropDowns(refereeentity);
            return View(refereeentity);
        }

        //
        // GET: /Referee/Delete/5
 
        public ActionResult Delete(Guid id)
        {
            RefereeEntity refereeentity = Unit.RefereeRepository.GetById(id);
            return PartialView(refereeentity);
        }

        //
        // POST: /Referee/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Unit.RefereeRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Index");
        }

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
    }
}