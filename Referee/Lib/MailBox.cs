﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Referee.Lib
{
    public class Message
    {
        public string Html { get; set; }
        public string Txt { get; set; }
    }
    
    public class MailBox
    {
        private static string _error = "";
        public static string ErrorMessage
        {
            get
            {
                return MailBox._error;
            }
        }
        public static string Server
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
            }
        }

        public static bool Send(string To, string From, string Subject, Message Message)
        {
            MailBox._error = "";
            MailMessage Mail = new MailMessage(From, To, Subject, Message.Txt);
            SmtpClient SMTPServer = new SmtpClient(MailBox.Server);            
            try
            {
                SMTPServer.Send(Mail);
                return true;
            }
            catch (Exception ex)
            {
              
                MailBox._error = ex.Message;
                return false;
            }
        }

        public static bool Send(string To, string From, string Subject, Message Message, string[] attachments)
        {
            MailBox._error = "";
            MailMessage Mail = new MailMessage(From, To, Subject, Message.Txt);
            SmtpClient SMTPServer = new SmtpClient(MailBox.Server);

            if (attachments.Count() > 0)
            {
                foreach (string path in attachments)
                {
                    Mail.Attachments.Add(new Attachment(path));
                }
            }


            try
            {
                SMTPServer.Send(Mail);
                return true;
            }
            catch (Exception ex)
            {

                MailBox._error = ex.Message;
                return false;
            }
        }
    }


}