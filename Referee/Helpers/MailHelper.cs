using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Lib;

namespace Referee.Helpers
{
    public class MailHelper
    {
        static string _systemName = System.Configuration.ConfigurationManager.AppSettings["SystemName"];
        static string _systemUrl = System.Configuration.ConfigurationManager.AppSettings["SystemUrl"];
        static string _mailSignature = System.Configuration.ConfigurationManager.AppSettings["MailSignature"];
        static string _mailFrom = String.Format("no-reply@{0}", MailHelper._systemUrl);

        public const string _success = "Mail sent successfully.";

        public static string ErrorMessage = String.Empty;


        public static void CreateNewAccountMessage(string Mailadr, string Password)
        {
            Message _message = new Message();
            string _txt = @"W aplikacji {0} zostało założone dla ciebie konto.\r\n
                            Aby się zalogowac do systemu wejdź na stronę {1} i użyj następujących danych.\r\n
                            \r\n
                            Login: {2}\r\n
                            Hasło: {3}\r\n
                            \r\n
                            {4}";
            _message.Txt = String.Format(_txt, MailHelper._systemName, MailHelper._systemUrl, Mailadr, Password, MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, "", _message))
            {
                MailHelper.ErrorMessage = MailHelper._success;
            }
            else
            {
                MailHelper.ErrorMessage = MailBox.ErrorMessage;
            }
        }
    }
}