using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Referee
{
    interface IRefereeEntity
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string City { get; set; }
        string Address { get; set; }
        string Mobile { get; set; }
        string Mailadr { get; set; }
        int RefClassId { get; set; }
        int AuthorizationId { get; set; }
        string FullName { get; }             
    }
}
