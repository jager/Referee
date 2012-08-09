using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Referee.Models
{
    /// <summary>
    /// Funkcje sędziowskie
    /// </summary>
    public class Function
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Nominated> Nominateds { get; set; }
        public virtual ICollection<RefereeRole> Roles { get; set; }
    }
}