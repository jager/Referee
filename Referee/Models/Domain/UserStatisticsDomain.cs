using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models.Domain
{
    public class UserStatisticsDomain
    {
        List<Nominated> _data;
        private Dictionary<int, string> Months = new Dictionary<int, string>() 
        { 
            {1, "Styczeń"},
            {2, "Luty"},
            {3, "Marzec"},
            {4, "Kwiecień"},
            {5, "Maj"},
            {6, "Czerwiec"},
            {7, "Lipiec"},
            {8, "Sierpień"},
            {9, "Wrzesień"},
            {10, "Październik"},
            {11, "Listopad"},
            {12, "Grudzień"}
        };
        private int _currentYear = DateTime.Now.Year;
        public UserStatisticsDomain(List<Nominated> UserNominations)
        {
            this._data = UserNominations;    
        }

        /// <summary>
        /// Returns dictionary object with month name and amount of nominations.
        /// </summary>
        /// <returns>Dictionary<string, int></returns>
        public Dictionary<string, int> GamesByMonth()
        {
            Dictionary<string, int> Games = new Dictionary<string, int>();
            for (int i = 1; i <= 12; i++)
            {
                Games.Add(Months[i], _data.Where(d => d.Nomination.Game.DateAndTime.Month == i && d.Nomination.Game.DateAndTime.Year == this._currentYear).Count());
            }
            return Games;
        }
    }
}