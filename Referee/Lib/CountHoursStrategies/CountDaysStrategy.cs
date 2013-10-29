using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Lib.CountHoursStrategies
{
    public class CountDaysStrategy : ICountHours
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private int _count = 0;
        public CountDaysStrategy(DateTime start, DateTime end)
        {
            this._startDate = start;
            this._endDate = end;
        }

        public int Count()
        {
            _count = this._endDate.Subtract(this._startDate).Days;
            return _count;
        }

    }
}