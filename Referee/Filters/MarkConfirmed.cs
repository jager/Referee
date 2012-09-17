using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Referee.Models;

namespace Referee.Filters
{
    public class MarkConfirmed : ActionFilterAttribute
    {
        private RefereeEntity _referee;
        public MarkConfirmed(RefereeEntity CurrentReferee)
        {
            this._referee = CurrentReferee;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
          
        }

    }
}