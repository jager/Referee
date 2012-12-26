using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Referee.ViewModels;
using Referee.Models;

namespace RefereeTest.ViewModels
{
    class EventTest
    {
        Event EventObject = new Event();
        Tournament TournamentObject = new Tournament()
        {
            Id = 2,
            Name = "TestTournament",
            Venue = "TestVenue",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddHours(2),
            StartTime = DateTime.Now.ToLocalTime().ToShortTimeString()
        };

        public EventTest()
        {
            ParseTournament();
        }
        

        private void ParseTournament()
        {
            string Type = "tournament";
            EventObject.Parse(TournamentObject, Type);
        }

        [Fact]
        public void IdentTournamentTest()
        {
            int Ident = TournamentObject.Id;
            Assert.Equal(Ident, EventObject.Ident);
        }

        [Fact]
        public void NameTournamentTest()
        {
            string Name = TournamentObject.Name;
            Assert.Equal(Name, EventObject.Name);
        }

        [Fact]
        public void VenueTournamentTest()
        {
            string Venue = TournamentObject.Venue;
            Assert.Equal(Venue, EventObject.Venue);
        }

        [Fact]
        public void DateTournamentTest()
        {
            string Dt = String.Format("{0} - {1}", TournamentObject.StartDate.ToShortDateString(), TournamentObject.EndDate.ToShortDateString());
            Assert.Equal(Dt, EventObject.Date);
        }


    }
}
