﻿using System;
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

        /// <summary>
        /// Powiadomienie o założeniu nowego konta
        /// </summary>
        /// <param name="Mailadr"></param>
        /// <param name="Password"></param>
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
                            
                            http://{1}/Nomination/Confirmation/?NominationId={2}&HashConf={3}&RefereeId={4}

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

        public static void RestorePasswordMessage(string Mailadr, string Token)
        {
            Message _message = new Message();
            string _txt = @"Ktoś wysłał prośbę o zmianę hasła dla konta w {0}.
                            Jeżeli to NIE byłeś ty to zignoruj tą wiadomość.
                            Jeżeli natomiast to ty wypełniłeś formularz zmiany hasła, to kliknij w poniższy link lub przeklej
                            go do paska adresu przeglądarki i postępuj wg zaleceń na stronie {1}.
                            
                            Link: {2}
                            
                            {3}";
            string _subject = String.Format("[{0}] Zmiana hasła.", MailHelper._systemName);
            string _link = String.Format("http://{0}/Account/RestorePassword/{1}", MailHelper._systemUrl, Token);
            _message.Txt = String.Format(_txt, 
                            MailHelper._systemName, 
                            MailHelper._systemName, 
                            _link, 
                            MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, _subject, _message))
            {
                MailHelper.ErrorMessage = MailHelper._success;
            }
            else
            {
                MailHelper.ErrorMessage = MailBox.ErrorMessage;
            }
        }

        /// <summary>
        /// Powiadomienie o nowym meczu w systemie do Referatu Obsad
        /// </summary>
        /// <param name="Mailadr"></param>
        /// <param name="GameInfo"></param>
        public static void CreateNewGameMessage(string Mailadr, string GameInfo)
        {
            Message _message = new Message();
            string _txt = @"W aplikacji {0} został dodany nowy mecz: {1}.
                            Osoba, która dodawała mecz zaznaczyła ciebie jako członka Referatu Obsad jako odpowiedzialnego
                            za nominowanie sędziów na mecz.

                            {2}
                            ";
            string _subject = String.Format("[{0}] Nowy mecz.", MailHelper._systemName);
            _message.Txt = String.Format(_txt, MailHelper._systemName, GameInfo, MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, _subject, _message))
            {
                MailHelper.ErrorMessage = MailHelper._success;
            }
            else
            {
                MailHelper.ErrorMessage = MailBox.ErrorMessage;
            }
        }

        public static void NoticeAboutChangeInGame(string Mailadr, Referee.Models.Game game)
        {
            Message _message = new Message();
            string _txt = @"W aplikacji {0} zmieniono dane  meczu: 
{1},                   
{2},
{3},
{4}";
            string _subject = String.Format("[{0}] Zmiana danych meczu.", MailHelper._systemName);
            string GameInfo = String.Format("{0} - {1}, {2}", game.HostTeam, game.GuestTeam, game.LeagueName);
            _message.Txt = String.Format(_txt, MailHelper._systemName, GameInfo, game.Venue, game.DateAndTime.ToString("dd-MM-yyyy HH:mm"), MailHelper._mailSignature);
            if (MailBox.Send(Mailadr, MailHelper._mailFrom, _subject, _message))
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