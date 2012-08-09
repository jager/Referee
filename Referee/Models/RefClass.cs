using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Referee.Models
{
    public class RefClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Central { get; set; }
        public bool International { get; set; }
        public ICollection<RefereeEntity> Referees { get; set; }
    }
}