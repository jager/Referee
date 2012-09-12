using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.ViewModels
{
    public class NominationMessage
    {
        public string Type { get; set; }        
        public string Mailadr { get; set; }
        public int NominationId { get; set; }
        public string HashConfirmation { get; set; }
        public Guid RefereeId { get; set; }

        public string GetEventType()
        {
            return (this.Type == "game" ? "mecz" : "turniej");
        }

        public string GetReferee()
        {
            return Convert.ToString(this.RefereeId);
        }
    }
}