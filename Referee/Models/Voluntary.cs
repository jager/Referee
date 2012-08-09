using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Lib.Security;

namespace Referee.Models
{
    public class Voluntary
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int TournamentId { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public int AmountOfReferees { get; set; }

        public ICollection<VoluntaryReferee> VoluntaryReferees { get; set; }

        public string GetCode()
        {
            return HashString.SHA1(String.Format("{0}{1}", Id.ToString(), DateTime.Now.ToLongDateString()));     
        }
    }
}