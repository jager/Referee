using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class CalendarEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
        public string className { get; set; }
    }
}