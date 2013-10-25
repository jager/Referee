using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;

namespace Referee.Helpers
{
    public class ExceptionLogger
    {
        private ILog log;
        private volatile static ExceptionLogger instance;
        private static Object thisLock = new Object();

        public ExceptionLogger()
        {
            this.log = LogManager.GetLogger(typeof(ExceptionLogger));
            XmlConfigurator.Configure();
        }

        public static ExceptionLogger GetInstance()
        {
            if (instance == null)
            {
                lock (thisLock)
                {
                    instance = new ExceptionLogger();
                }
            }
            return instance;
        }

        public void Write(string Message)
        {
            this.log.Error(Message);
        }
    }
}