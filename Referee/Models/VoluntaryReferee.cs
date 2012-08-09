using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class VoluntaryReferee
    {
        public int Id { get; set; }
        public string RefereeId { get; set; }
        public int VoluntaryId { get; set; }
        public DateTime Added { get; set; }

        public virtual Voluntary Voluntary { get; set; }
    }
}