using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.ViewModels
{
    public class Conflicts
    {
        public string Referee { get; set; }
        public string Photo { get; set; }
        public string Event { get; set; }
        public string Period { get; set; }
        public int NominationId { get; set; }
        public Guid RefereeId { get; set; }
        public string Time { get; set; }
        public string League { get; set; }
    }
}