using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Controllers.Base;
using Referee.Models;


namespace Referee.Controllers
{
    public class ConfigurationController : BaseController
    {
        //
        // GET: /Configuration/

        [HttpGet]
        public ActionResult Index()
        {
            return View(Configuration);
        }

        [HttpPost]
        public JsonResult Update(AppConfig AppConfig)
        {
            var Message = new { @Message = "", @Error = 0 };
            try
            {
                var Config = Unit.ConfigRepository.Get(filter: c => c.Key == AppConfig.Key).FirstOrDefault();
                Config.Value = AppConfig.Value;
                Unit.ConfigRepository.Update(Config);
                Unit.Save();
                
            }
            catch (Exception e)
            {
                Message = new { @Message = e.Message, @Error = 1 };
            }
            return Json(Message);
        }
    }
}
