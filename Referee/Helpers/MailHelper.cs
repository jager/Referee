using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Lib;
using Referee.ViewModels;

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
            string _txt = @"W aplikacji {0} zostało założone dla ciebie konto.
                            Aby się zalogowac do systemu wejdź na stronę {1} i użyj następujących danych.
                            
                            Login: {2}
                            Hasło: {3}
                            
                            {4}";
            string _subject = String.Format("[{0}] Rejestracja.", MailHelper._systemName);
            _message.Txt = String.Format(_txt, MailHelper._systemName, MailHelper._systemUrl, Mailadr, Password, MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, _subject, _message))
            {
                MailHelper.ErrorMessage = MailHelper._success;
            }
            else
            {
                MailHelper.ErrorMessage = MailBox.ErrorMessage;
            }
        }

        public static void RemindPasswordMessage(string Mailadr, string Password)
        {
            Message _message = new Message();
            string _txt = @"Twoje hasło do aplikacji {0} zostało zmienione. Nowe hasło to:
                            
                            Hasło: {1}
                            
                            {2}";
            string _subject = String.Format("[{0}] Zmiana hasła.", MailHelper._systemName);
            _message.Txt = String.Format(_txt, MailHelper._systemName, Password, MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, _subject, _message))
            {
                MailHelper.ErrorMessage = MailHelper._success;
            }
            else
            {
                MailHelper.ErrorMessage = MailBox.ErrorMessage;
            }
        }

        public static void CreateNominationMessage(NominationMessage nMessage)
        {
            Message _message = new Message();
            string _txt = @"Zostałeś nominowany na {0}
                            Sprawdź poniższy link:
                            
                            http://{1}/Nomination/Confirmation/?NominationId={2}&amp;HashConf={3}&amp;RefereeId={4}

                            {5}
                            ";
            string _subject = String.Format("[{0}] Nowa nominacja.", MailHelper._systemName);
            _message.Txt = String.Format(_txt, nMessage.GetEventType(), MailHelper._systemUrl, nMessage.NominationId, nMessage.HashConfirmation, nMessage.GetReferee(), MailHelper._mailSignature);
            if (MailBox.Send(nMessage.Mailadr, MailHelper._mailFrom, _subject, _message))
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