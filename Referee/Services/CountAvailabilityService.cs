using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Services
{
    public class CountAvailabilityService
    {
        ICountHours CountHours;
        public CountAvailabilityService(ICountHours AvailabilityCount) 
        {
            this.CountHours = AvailabilityCount;
        }

        public int Count()
        {
            return this.CountHours.Count();
        }
    }
}