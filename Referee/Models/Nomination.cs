using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Referee.Lib.Security;

namespace Referee.Models
{
    public class Nomination
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue=false)]
        [DataType(DataType.DateTime)]
        public DateTime Added { get; set; }

        [Display(Name="Uwagi")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Display(Name="Opublikowana")]
        public bool Published { get; set; }

        [HiddenInput(DisplayValue=false)]
        public DateTime PublishDate { get; set; }

        [Display(Name="Potwierdzona")]
        public bool Confirmed { get; set; }

        [HiddenInput(DisplayValue=false)]
        public DateTime ConfirmationDate { get; set; }

        [Display(Name="Wysłana mailem")]
        public bool Emailed { get; set; }

        [HiddenInput(DisplayValue=false)]
        public DateTime EmailDate { get; set; }

        [HiddenInput(DisplayValue=false)]
        public string HashConfirmation { get; set; }        

        [Display(Name="Mecz")]
        public int? GameId { get; set; }
        public int? TournamentId { get; set; }
        public int? SeasonId { get; set; }

        public virtual Game Game { get; set; }
        public virtual Tournament Tournament { get; set; }
        public virtual Season Season { get; set; }
        public virtual ICollection<Nominated> Nominateds { get; set; }

        public string GetCode()
        {
            return HashString.SHA1(String.Format("{0}{1}", Id.ToString(), DateTime.Now.ToLongDateString()));
        }

    }
}