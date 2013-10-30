using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Services;
using Referee.Lib.CountHoursStrategies;

namespace Referee.Helpers
{
    public class AvailabilityHelper
    {

        private int count = 0;
        public int Count
        {
            get
            {
                return count;
            }
        }

        private DateTime StartDate;
        private DateTime EndDate;

        /*
        private int MinHourWeekDay = 17;
        private int MaxHourWeekDay = 22;
        private int MinHourWeekendDay = 8;
        private int MaxHourWeekendDay = 22;
        private DayOfWeek DOW;
        private bool IsWeekend = false;
        */

        public AvailabilityHelper(DateTime start, DateTime end)
        {
            this.StartDate = start;
            this.EndDate = end;
            this.doCounting();
        }

        public static int Count(CountDaysStrategy cas)
        {
            return cas.Count();
        }


        private void doCounting()
        {
            if (StartDate == null || EndDate == null)
            {
                return;
            }


            CountAvailabilityService CAS = new CountAvailabilityService(new CountDaysStrategy(this.StartDate, this.EndDate));
            count = CAS.Count();






            /*
            if (StartDate < EndDate)
            {
                DateTime Start = StartDate;
                DateTime End = EndDate;
                do
                {
                    if (Start.DayOfWeek == DayOfWeek.Saturday || Start.DayOfWeek == DayOfWeek.Sunday)
                    {
                        Start = (Start == StartDate) ? new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartDate.Hour, 0, 0)
                                                     : new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 8, 0, 0);
                        End = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 22, 0, 0);
                        CountHoursService CHS = new CountHoursService(new WeekendCountStrategy(Start, End));
                        count += CHS.Count();
                    }
                    else
                    {
                        Start = (Start == StartDate) ? new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartDate.Hour, 0, 0)
                                                     : new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 17, 0, 0);
                        End = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 22, 0, 0);
                        CountHoursService CHS = new CountHoursService(new WeekCountStrategy(Start, End));
                        count += CHS.Count();
                    }
                    Start = Start.AddDays(1);
                } while (Start < End);
                
            }
             */ 


                        
        }
    }
}