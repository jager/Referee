using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Referee.Models
{
    public class Tournament
    {
        public int Id { get; set; }

        [Display(Name="Nazwa turnieju")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public string Name { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Miejsce")]
        public string Venue { get; set; }
        
        [Display(Name="Kontakt z organizatorem")]
        public string Organizer { get; set; }
        
        [Display(Name="Początek turnieju")]
        public DateTime StartDate { get; set; }
        
        [Display(Name = "Koniec turnieju")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Godzina rozpoczęcia")]
        public string StartTime { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Zespoły zgłoszone")]
        public string TeamsEnrolled { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name="Uwagi")]
        public string Note { get; set; }

        [Display(Name="Typ turnieju")]
        public string Type { get; set; }

        public bool Published { get; set; }
        
        public int? LeagueId { get; set; }
        public string LeagueName { get; set; }

        public int SeasonId { get; set; }
        public virtual Season Season { get; set; } 
    }
}