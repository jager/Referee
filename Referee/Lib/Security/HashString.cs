using System;
using System.Security.Cryptography;
using System.Text;

namespace Referee.Lib.Security
{
    public class HashString
    {

        /// <summary>
        /// method to hash the users password. To match the CAPICOM hash
        /// we convert the string to UNICODE first
        /// </summary>
        /// <returns>string</returns>
        public static string SHA1(string str)
        {
            //create our SHA1 provider
            SHA1 sha = new SHA1CryptoServiceProvider();
            string hashedValue = string.Empty;
            //hash the data
            byte[] hashedData = sha.ComputeHash(Encoding.Unicode.GetBytes(str));
    
            //loop through each byte in the byte array
            foreach (byte b in hashedData)
            {
                //convert each byte and append
                hashedValue += String.Format("{0,2:X2}", b);
            }

            //return the hashed value
            return hashedValue;
        }
    }
}