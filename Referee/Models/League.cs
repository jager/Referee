using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Referee.Models
{
    public class League
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Visible { get; set; }

        public virtual ICollection<RefereeRole> Roles { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public int Internal
        {
            get
            {
                return 199;
            }
        }

        public int Central
        {
            get
            {
                return 399;
            }
        }

        public int European
        {
            get
            {
                return 599;
            }
        }

        public int Academic
        {
            get
            {
                return 699;
            }
        }



    }
}