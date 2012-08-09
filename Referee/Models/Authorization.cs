using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Referee.Models
{
    /// <summary>
    /// Uprawnienia do sędziowania określonych meczów
    /// </summary>
    public class Authorization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RefereeRole> Roles { get; set; }
        public virtual ICollection<RefereeEntity> Referees { get; set; }
    }
}