﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;
using Referee.DAL;
using Referee.Lib.Security;


namespace Referee.Repositories
{
    public class ChangePasswordRepository : GenericRepository<ChangePassword>
    {
        private ChangePassword _password;
        public ChangePassword Password
        {
            get
            {
                return _password;   
            }
        }
        public ChangePasswordRepository(RefereeContext context) : base(context)
        {            
        }

        /// <summary>
        /// Creates and saves new ChangePassword object 
        /// </summary>
        /// <param name="UserId">User indentifier</param>
        /// <returns>Returns new created token</returns>
        public string Create(string UserId)
        {
            ChangePassword Password = new ChangePassword();
            Password.Added = DateTime.Now;
            Password.Updated = DateTime.Now;
            Password.UserId = UserId;
            Password.Token = HashString.SHA1(Password.Added.ToString());            
            this.Insert(Password);
            return Password.Token;
        }


        /// <summary>
        /// Returns ChangePassword object for given token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        private ChangePassword GetToken(string Token)
        {
            return this.Get(filter: p => p.Token == Token)
                       .SingleOrDefault<ChangePassword>();
        }

        /// <summary>
        /// Checks wheather token is valid
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public bool Check(string Token)
        {
            ChangePassword Pass = this.GetToken(Token);
            if (Pass != null && Pass.isValid())
            {
                _password = Pass;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes Token
        /// </summary>
        /// <param name="Token"></param>
        public void Delete(string Token)
        {
            ChangePassword Password = this.GetToken(Token);
            this.Delete(Password);
        }
    }
}