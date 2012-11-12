using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Referee.Lib;
using System.Web.Security;

namespace Referee.Models.Domain
{
    public class UserDomain
    {
        private RefereeEntity _data;
        public RefereeEntity Data
        {
            get
            {
                return _data;
            }
        }

        public UserDomain() { }

        public UserDomain(RefereeEntity referee)
	    {
            this._data = referee;
	    }



        /// <summary>
        /// Provides referees fullname and photo packed into dictionary object
        /// </summary>
        /// <returns>Dictionary<string,string>Object with path to user photo and user's fullname</returns>
        public Dictionary<string, string> GetPhotoAndFullName()
        {
            if (_data == null)
            {
                return null;
            }

            Dictionary<string, string> PhotoAndName = new Dictionary<string, string>()
            {
                {"photo", this.GetPhotoPath(_data.Photo, _data.DestinationFolder)},
                {"fullname", _data.FullName}
            };

            return PhotoAndName;
        }


        /// <summary>
        /// Returns path to user's photo to be placed into href attribute of img tag.
        /// </summary>
        /// <param name="PhotoName"></param>
        /// <param name="PhotoPath"></param>
        /// <returns></returns>
        private string GetPhotoPath(string PhotoName, string PhotoPath)
        {
            if ( String.IsNullOrEmpty(PhotoName) || String.IsNullOrEmpty(PhotoPath)) 
            {
                return String.Empty;
            }
            return FileUploader.GetUserPhotoPath(String.Format("{0}{1}", PhotoPath, PhotoName));
        }

        /// <summary>
        /// Creates Membership User
        /// </summary>
        /// <param name="Mailadr">E-mail address</param>
        /// <param name="Password">Password string</param>
        /// <param name="PasswordConfirmed">Password confirmed string</param>
        /// <param name="UserID">Outputs user ID</param>
        /// <returns>Boolean (True when user is created, False when sth goes wrong)</returns>
        public bool CreateUser(string Mailadr, string Password, string PasswordConfirmed, out Guid UserID)
        {
            UserID = Guid.Empty;
            RegisterModel NewUser = new RegisterModel { Email = Mailadr, Password = Password, ConfirmPassword = PasswordConfirmed, UserName = Mailadr };
            MembershipCreateStatus createStatus;
            Membership.CreateUser(NewUser.UserName, NewUser.Password, NewUser.Email, null, null, true, null, out createStatus);
            if (createStatus == MembershipCreateStatus.Success)
            {
                UserID = (Guid)Membership.GetUser(Mailadr).ProviderUserKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigns roles to User
        /// </summary>
        /// <param name="UserName">Name of the User</param>
        /// <param name="SelectedRoles">Table with selected roles</param>
        /// <returns>Boolean (True when roles are assigned, False when sth goes wrong)</returns>
        public bool AssignRole(string UserName, string[] SelectedRoles)
        {
            var User = Membership.GetUser(UserName);
            if (User != null)
            {
                var ExistingRoles = Roles.GetRolesForUser(UserName);
                if (ExistingRoles.Count() > 0)
                {
                    Roles.RemoveUserFromRoles(UserName, ExistingRoles);
                }
                Roles.AddUserToRoles(UserName, SelectedRoles);
                return true;
            }
            return false;
        }
    }
}