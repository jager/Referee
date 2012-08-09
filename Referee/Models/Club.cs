using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Referee.Models
{
    public class Club
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name="Nazwa klubu")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Nr telefonu")]
        public string Phone { get; set; }

        [Display(Name = "Adres mailowy")]
        public string Mailadr { get; set; }

        [Display(Name = "Kod pocztowy")]
        public string Zip { get; set; }

        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Display(Name = "Ulica")]
        public string Address { get; set; }

        [Display(Name = "Strona www")]
        public string WebSite { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}