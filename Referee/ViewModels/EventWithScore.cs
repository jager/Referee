using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.ViewModels
{
    public class EventWithScore : Event
    {
        public string Score { get; set; }

        public void Parse(object Event, string Type)
        {
            base.Parse(Event, Type);
            if (Type == "Game")
            {
                this.Score = ((Game)Event).Score;
            }
            else
            {
                this.Score = "";
            }
        }
    }
}