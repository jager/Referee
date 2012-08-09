using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int LeagueId { get; set; }

        public virtual Team Team { get; set; }
        public virtual League League { get; set; }
    }
}