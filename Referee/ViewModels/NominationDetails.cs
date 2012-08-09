using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.ViewModels
{
    public class NominationDetails
    {
        public Game Game { get; set; }
        public Nomination Nomination { get; set; }
        public ICollection<Nominated> NominatedReferees { get; set; }
    }
}