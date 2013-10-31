using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Referee.Models
{
    /// <summary>
    /// Model sędziego, który będzie połączony z userem systemu przez GUID, jego dane teleadresowe, klasa sędziowska oraz uprawnienia
    /// </summary>
    public class RefereeEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name="Imię")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [MaxLength(50, ErrorMessage="Wartość pola jest zbyt długa. Nie powinna przekraczać 50 znaków.")]
        public string FirstName { get; set; }

        [Display(Name="Nazwisko")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [MaxLength(50, ErrorMessage = "Wartość pola jest zbyt długa. Nie powinna przekraczać 50 znaków.")]
        public string LastName { get; set; }

        [Display(Name="Kod pocztowy")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [RegularExpression("[0-9]{2}-[0-9]{3}", ErrorMessage="Proszę poprawnie wpisać kod pocztowy (xx-xxx)")]
        public string Zip { get; set; }

        [Display(Name="Miasto")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [MaxLength(100, ErrorMessage = "Wartość pola jest zbyt długa. Nie powinna przekraczać 100 znaków.")]
        public string City { get; set; }

        [Display(Name="Adres zamieszkania - ulica, nr domu, nr mieszkania")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [MaxLength(100, ErrorMessage = "Wartość pola jest zbyt długa. Nie powinna przekraczać 100 znaków.")]
        public string Address { get; set; }


        [Display(Name="Nr telefonu")]
        [RegularExpression(@"[0-9 \-]{11,14}", ErrorMessage="Proszę wpisać poprawny numer telefonu (xx xxx xx xx).")]
        public string Phone { get; set; }

        [Display(Name="Nr telefonu komórkowego")]
        [RegularExpression(@"[0-9 \-]{11}", ErrorMessage = "Proszę wpisać poprawny numer telefonu komórkowego (xxx xxx xxx).")]
        public string Mobile { get; set; }

        [Display(Name="Adres mailowy")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        [DataType(DataType.EmailAddress, ErrorMessage="Proszę wpisać poprawny adres mailowy")]
        public string Mailadr { get; set; }

        [HiddenInput(DisplayValue = false)]
        public float? Longitude { get; set; }

        [HiddenInput(DisplayValue = false)]
        public float? Latitude { get; set; }

        [Display(Name="Wybierz zdjęcie do załadowania")]
        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }

        [Display(Name="Klasa sędziowska")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public int RefClassId { get; set; }

        [Display(Name="Uprawnienia")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public int AuthorizationId { get; set; }

        [Display(Name = "Data urodzenia")]
        public DateTime DOB { get; set; }

        [Display(Name = "Data ukończenia kursu")]
        public string DateOfEnrollment { get; set; }

        public string DestinationFolder { get; set; }
        
        public virtual RefClass RefClass { get; set; }
        public virtual ICollection<Penalty> Penalties { get; set; }
        public virtual Authorization Authorization { get; set; }
        public virtual ICollection<Availability> Availabilities { get; set; }
        public virtual ICollection<Voluntary> Voluntaries { get; set; }


        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }
    }
}