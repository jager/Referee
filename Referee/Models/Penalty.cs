using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string FullComment { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int RefereeId { get; set; }
        public virtual RefereeEntity Referee { get; set; }
    }
}