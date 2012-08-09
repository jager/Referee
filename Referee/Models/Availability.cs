using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime Created { get; set; }
        public bool Valid { get; set; }
        public string RefereeId { get; set; }

        public virtual RefereeEntity Referee { get; set; }

    }
}