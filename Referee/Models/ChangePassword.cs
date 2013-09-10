using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Referee.Models
{
    public class ChangePassword
    {

        private int _valid = 5;

        [Key]
        public string Token { get; set; }
        public string RefereeId { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }

        [NotMapped]
        public int ValidPeriod
        {
            get
            {
                return _valid;
            }
        }

        /// <summary>
        /// Generates hash string.
        /// </summary>
        /// <returns>string</returns>
        public string GenKey()
        {
            byte[] data = new byte[100];
            byte[] result;
            using (SHA256 shaM = new SHA256Managed())
            {
                result = shaM.ComputeHash(data);
            }
            return System.Text.UTF8Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// Chcecks whether token is still valid
        /// </summary>
        /// <returns>bool</returns>
        public bool isValid()
        {
            if (this.Added != null)
            {
                return (DateTime.Now <= this.Added.AddHours(this.ValidPeriod));
            }
            return false;
        }
    }
}