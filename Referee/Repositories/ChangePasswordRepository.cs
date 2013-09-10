using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebVolleyballManager.Models;


namespace WebVolleyballManager.Repositories
{
    public class ChangePasswordRepository : GenericRepository<ChangePassword>
    {

        public ChangePasswordRepository(ManagerContext context) : base(context)
        {            
        }



        /// <summary>
        /// Creates and saves new ChangePasswod object 
        /// </summary>
        /// <param name="UserId"></param>
        public void Create(string UserId)
        {
            ChangePassword Password = new ChangePassword();
            Password.Added = DateTime.Now;
            Password.Token = Password.GenKey();
            Password.UserId = UserId;
            this.Insert(Password);
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
            ChangePassword Password = this.GetToken(Token);
            return Password.isValid();
        }

        /// <summary>
        /// Changes Token
        /// </summary>
        /// <param name="Token"></param>
        public void Change(string Token)
        {
            ChangePassword Password = this.GetToken(Token);
            Password.Token = Password.GenKey();
            Password.Updated = DateTime.Now;
            this.Update(Password);
        }
    }
}