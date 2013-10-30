using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Lib.CountHoursStrategies
{
    public class WeekCountStrategy : ICountHours
    {
        private DateTime _startDate;
        private DateTime _endDate;
        protected int Min = 17;
        protected int Max = 22;
        private int _count = 0;

        public WeekCountStrategy(DateTime start, DateTime end)
        {
            this._startDate = start;
            this._endDate = end;
        }

        public int Count()
        {
            if (_startDate.Day == _endDate.Day)
            {
                int min = _startDate.Hour > Min ? _startDate.Hour : Min;
                int max = _endDate.Hour < Max ? _endDate.Hour : Max;
                _count = max - min;
            }
            return _count;
        }
    }
}