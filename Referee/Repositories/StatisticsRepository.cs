using Referee.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Repositories
{
    public class StatisticsRepository
    {
        private RefereeContext db;
        public StatisticsRepository(RefereeContext context)
        {
            db = context;
        }

        public void GetStat()
        {
            var d = db.Database.SqlQuery<string>("select 1").ToList();
        }
    }                           
}