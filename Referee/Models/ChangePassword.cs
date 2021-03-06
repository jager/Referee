﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Referee.Models
{
    public class ChangePassword
    {

        private int _valid = 24;

        [Key]
        public string Token { get; set; }
        public string UserId { get; set; }
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