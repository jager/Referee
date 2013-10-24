using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using log4net;
using log4net.Config;

namespace Referee.Helpers
{

    /// <summary>
    /// Severity level
    /// </summary>
    public enum LogOutput : int
    {
        Information = 1,
        Error = 2
    }

    public static class LogHelper
    {
        /// <summary>
        /// Saves messages to log, depending on severity level
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        public static void Log(string Message, int Level)
        {
            switch (Level)
            {
                case (int) LogOutput.Error:
                    Error(Message);
                    break;
                case (int) LogOutput.Information:
                default:
                    Information(Message);
                    break;

            }

        }

        /// <summary>
        /// Saves to log application informations
        /// </summary>
        /// <param name="Message"></param>
        public static void Information(string Message)
        {
            InformationLogger logger = new InformationLogger();
            logger.Write(Message);
        }

        /// <summary>
        /// Saves to log application errors
        /// </summary>
        /// <param name="Message"></param>
        public static void Error(string Message)
        {
            ExceptionLogger logger = new ExceptionLogger();
            logger.Write(Message);
        }
    }
}