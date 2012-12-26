using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;
using System.Text;

namespace Referee.Helpers
{
    public class TeamHelper
    {

        public static string GetEnrollmentsNames(List<Enrollment> Enrollment, string Separator)
        {
            StringBuilder ReturnValue = new StringBuilder();
            if (Enrollment.Count() == 0)
            {
                return String.Empty;
            }

            foreach (Enrollment E in Enrollment) 
            {
                ReturnValue.Append(String.Format("{0}{1} ", E.League.Name, Separator));
            }
            return ReturnValue.ToString().TrimEnd(Separator.ToCharArray());
        }
    }
}