using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Referee.Models
{
    public class Season
    {
        public int Id { get; set; }

        [Display(Name="Nazwa sezonu")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public string Name { get; set; }

        [Display(Name="Obecny sezon")]
        public bool Active { get; set; }

        [Display(Name="Przeniesiony do archiwum")]
        public bool Archive { get; set; }

        [Display(Name="Widoczny")]
        public bool Visible { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Nomination> Nominations { get; set; }
    }
}