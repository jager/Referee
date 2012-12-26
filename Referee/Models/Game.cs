using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Referee.Models
{
    /// <summary>
    /// Mecze
    /// </summary>
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Numer meczu")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public string GameNo { get; set; }

        [HiddenInput(DisplayValue=false)]
        public string Venue { get; set; }

        [Display(Name="Data i godzina meczu")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public DateTime DateAndTime { get; set; }

        [Display(Name="Wynik meczu")]
        public string Score { get; set; }

        [Display(Name="Opublikowany")]
        public bool Published { get; set; }

        [Display(Name = "Drużyna gospodarzy")]
        [HiddenInput(DisplayValue=false)]
        public int HostTeamId { get; set; }

        [Display(Name = "Drużyna gości")]
        [HiddenInput(DisplayValue=false)]
        public int GuestTeamId { get; set; }

        [Display(Name="Drużyna gospodarzy")]
        [Required]
        public string HostTeam { get; set; }

        [Display(Name="Drużyna gości")]
        public string GuestTeam { get; set; }

        [Display(Name="Sezon")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public int SeasonId { get; set; }

        [Display(Name="Liga")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }

        public virtual Season Season { get; set; }

        public string Name
        {
            get
            {
                return String.Format("{0} - {1}", this.HostTeam, this.GuestTeam);
            }
        }
    }
}