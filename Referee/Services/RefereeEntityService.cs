using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;
using Referee.DAL;
using Referee.Lib.CountHoursStrategies;
namespace Referee.Services
{
    public class RefereeEntityService
    {

        RefereeEntity RefereeObject;
     

        public RefereeEntityService(RefereeEntity Referee)
        {
            this.RefereeObject = Referee;
        }

        public int DaysOfNonAvailability(List<Availability> avail)
        {
            int count = 0;
            foreach (var a in avail)
            {
                count += Referee.Helpers.AvailabilityHelper.Count(new CountDaysStrategy(a.DateStart, a.DateEnd));
            }
            return count;
        }

    }
}