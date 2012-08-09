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
    public class AvailController : BaseController
    {


        //
        // GET: /Avail/

        public PartialViewResult Index(string id)
        {
            ViewBag.RefereeId = id;
            return PartialView(Unit.AvailabilityRepository.Get(filter: a => a.RefereeId == id,orderBy: a => a.OrderByDescending(s => s.Created)));
        }

        

        //
        // GET: /Avail/Create

        public ActionResult Create(string RefereeId)
        {
            ViewBag.RefereeId = RefereeId;
            return View();
        } 

        //
        // POST: /Avail/Create

        [HttpPost]
        public ActionResult Create(Availability availability, FormCollection form)
        {
            DateTime dtStart;
            DateTime dtEnd;
            DateTime.TryParse(String.Format("{0} {1}", form["dtStartDate"], form["dtStartTime"]), out dtStart);
            DateTime.TryParse(String.Format("{0} {1}", form["dtEndDate"], form["dtEndTime"]), out dtEnd);
            availability.DateStart = dtStart;
            availability.DateEnd = dtEnd;
            availability.Created = DateTime.Now;
            availability.Valid = true;
            if (ModelState.IsValid)
            {
                Unit.AvailabilityRepository.Insert(availability);
                Unit.Save();
                return RedirectToAction("Details", "Referee", new { id = availability.RefereeId });
            }
            
            ViewBag.RefereeId = availability.RefereeId;
            
            return View(availability);
        }
        
        //
        // GET: /Avail/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(Unit.AvailabilityRepository.GetById(id));
        }

        //
        // POST: /Avail/Edit/5

        [HttpPost]
        public ActionResult Edit(Availability availability)
        {
            if (ModelState.IsValid)
            {
                Unit.AvailabilityRepository.Update(availability);
                Unit.Save();
                return RedirectToAction("Index");
            }
            return View(availability);
        }

        //
        // GET: /Avail/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(Unit.AvailabilityRepository.GetById(id));
        }

        //
        // POST: /Avail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var av = Unit.AvailabilityRepository.GetById(id);
            string RefereeId = av.RefereeId;
            Unit.AvailabilityRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Index", new { id = RefereeId });
        }

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}