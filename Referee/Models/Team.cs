using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Referee.Models
{
    /// <summary>
    /// Model drużyny, która posiada swojego trenera i halę sporotową gdzie rozgrywa swoje mecze.
    /// </summary>
    public class Team
    {
        public int Id { get; set; }

        [Display(Name="Nazwa drużyny")]
        [Required(ErrorMessage="To pole jest wymagane")]
        [MaxLength(250)]
        public string Name { get; set; }        

        [Display(Name="Płeć")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public string Gender { get; set; }

        [Display(Name="Pełny adres hali Sportowej gdzie dana drużyna rozgrywa swoje mecze")]        
        public string Venue { get; set; }

        [HiddenInput(DisplayValue=false)]
        public float? Longitude { get; set; }

        [HiddenInput(DisplayValue=false)]
        public float? Latitude { get; set; }

        [Display(Name="Nazwa klubu w barwach którego gra ta drużyna")]
        [Required(ErrorMessage="To pole jest wymagane")]
        public Guid ClubId { get; set; }

        [Display(Name = "Imię i nazwisko trenera/opiekuna.")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [MaxLength(150)]
        public string Coach { get; set; }

        [Display(Name = "Telefon kontaktowy do trenera")]
        [RegularExpression(@"[0-9 \-]{11,14}", ErrorMessage = "Proszę wpisać poprawny numer telefonu (xx xxx xx xx).")]
        public string CoachPhone { get; set; }

        [Display(Name = "Telefon publiczny?")]
        public bool? CoachPhoneVisible { get; set; }

        [Display(Name = "Adres mailowy do trenera")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Proszę wpisać poprawny adres mailowy")]
        public string CoachMailAdress { get; set; }

        [Display(Name = "Adres mailowy publiczny?")]
        public bool? CoachMailAdressVisible { get; set; }
        
        public virtual Club Club { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}