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
using Referee.Helpers;

namespace Referee.Controllers
{ 
    public class SeasonController : BaseController
    {
        private RefereeContext db = new RefereeContext();

        //
        // GET: /Season/
        [Authorize(Roles = "Administrator")]//HelperRoles.Administrator)]
        public ViewResult Index()
        {
            return View(Unit.SeasonRepository.Get());
        }

        //
        // GET: /Season/Create

        [Authorize(Roles="" + HelperRoles.Administrator)]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Season/Create

        [HttpPost]
        [Authorize(Roles = HelperRoles.Administrator)]
        public ActionResult Create(Season season)
        {
            if (ModelState.IsValid)
            {
                Season InsertedSeason = DeactivateOtherSeasons(season);
                Unit.SeasonRepository.Insert(InsertedSeason);
                Unit.Save();
                return RedirectToAction("Index");  
            }

            return View(season);
        }
        
        //
        // GET: /Season/Edit/5

        [Authorize(Roles = HelperRoles.Administrator)]
        public ActionResult Edit(int id)
        {
            return View(Unit.SeasonRepository.GetById(id));
        }

        //
        // POST: /Season/Edit/5

        [HttpPost]
        [Authorize(Roles = HelperRoles.Administrator)]
        public ActionResult Edit(Season season)
        {
            if (ModelState.IsValid)
            {
                Season updatedSeason = DeactivateOtherSeasons(season);
                Unit.SeasonRepository.Update(updatedSeason);
                Unit.Save();
                return RedirectToAction("Index");
            }
            return View(season);
        }

        //
        // GET: /Season/Delete/5

        [Authorize(Roles = HelperRoles.Administrator)]
        public ActionResult Delete(int id)
        {
            return View(Unit.SeasonRepository.GetById(id));
        }

        //
        // POST: /Season/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = HelperRoles.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit.SeasonRepository.Delete(id);
            Unit.Save();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// For all other seasons when current is activ, set active to false and archive to true.
        /// Checking if only one season is active.
        /// </summary>
        /// <param name="season">Refferee.Models.Season</param>
        /// <returns>Referee.Models.Season</returns>
        private Season DeactivateOtherSeasons(Season season)
        {
            if (season.Active)
            {
                var otherSeasons = Unit.SeasonRepository.Get(filter: s => s.Id != season.Id);
                if (otherSeasons != null)
                {
                    foreach (Season other in otherSeasons)
                    {
                        other.Active = false;
                        other.Archive = true;
                        Unit.SeasonRepository.Update(other);
                    }
                }
                if (season.Id != CurrentSeason.Id)
                {
                    SetCurrentSeason(true);
                }
                season.Archive = false;
            }
            return season;
        }

//TODO Dodać akcję switchSeason, która będzie zmieniała sezon, na którym pracujemy.

        protected override void Dispose(bool disposing)
        {
            Unit.Dispose();
            base.Dispose(disposing);
        }
    }
}