using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.ViewModels
{
    public class Event
    {
        public object Ident { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Venue { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Details { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public string League { get; set; }
        public string LeagueId { get; set; }

        public void Parse(object NewEvent, string Type)
        {
            if (Type == "tournament")
            {
                this.Ident = ((Tournament)NewEvent).Id;
                this.Name = ((Tournament)NewEvent).Name;
                this.Type = "tournament";
                this.Venue = ((Tournament)NewEvent).Venue;
                DateTime dtStart = ((Tournament)NewEvent).StartDate;
                DateTime dtEnd = ((Tournament)NewEvent).EndDate;
                if (dtEnd == dtStart)
                {
                    this.Date = dtStart.ToShortDateString();
                }
                else
                {
                    this.Date = String.Format("{0} - {1}", dtStart.ToShortDateString(), dtEnd.ToShortDateString());
                }
                this.Time = ((Tournament)NewEvent).StartTime;
                this.MinDate = dtStart;
                this.MaxDate = dtEnd;
                this.League = "";
                this.LeagueId = "";
                if (((Tournament)NewEvent).Type == "League")
                {
                    this.League = ((Tournament)NewEvent).LeagueName;
                    this.LeagueId = ((Tournament)NewEvent).LeagueId.ToString();
                }
            }
            else if (Type == "game")
            {
                this.Ident = ((Game)NewEvent).Id;
                this.Name = ((Game)NewEvent).Name;
                this.Type = "game";
                this.Venue = ((Game)NewEvent).Venue;
                this.Date = ((Game)NewEvent).DateAndTime.ToShortDateString();
                this.Time = ((Game)NewEvent).DateAndTime.ToShortTimeString();
                this.MinDate = ((Game)NewEvent).DateAndTime.AddHours(-2);
                this.MaxDate = ((Game)NewEvent).DateAndTime.AddHours(2);
                this.League = ((Game)NewEvent).LeagueName;
                this.LeagueId = ((Game)NewEvent).LeagueId.ToString();
            }
        }
    }
}