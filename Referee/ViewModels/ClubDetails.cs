using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.ViewModels
{
    public class ClubDetails
    {
        public Club Club { get; set; }
        public ICollection<Team> Teams { get; set; }
    }
}