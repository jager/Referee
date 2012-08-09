using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class RefereeRole
    {
        public int Id { get; set; }
        public int FunctionId { get; set; }
        public int AuthorizationId { get; set; }
        public int LeagueId { get; set; }

        public virtual Function Function { get; set; }
        public virtual Authorization Authorization { get; set; }
        public virtual League League { get; set; }
    }
}