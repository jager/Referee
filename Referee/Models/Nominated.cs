using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class Nominated
    {
        public int Id { get; set; }
        public Guid RefereeId { get; set; }
        public int NominationId { get; set; }
        public int FunctionId { get; set; }

        public virtual RefereeEntity Referee { get; set; }
        public virtual Nomination Nomination { get; set; }
        public virtual Function Function { get; set; }
    }
}