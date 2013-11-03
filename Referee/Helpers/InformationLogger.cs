using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace Referee.Helpers
{
    public class InformationLogger
    {
        private volatile static InformationLogger instance;
        private ILog log;
        private static Object thisLock = new Object();

        public InformationLogger()
        {
            this.log = LogManager.GetLogger(typeof(InformationLogger));
        }

        public static InformationLogger GetInstance()
        {
            
            if (instance == null)
            {
                lock (thisLock)
                {
                    instance = new InformationLogger();
                }
            }
            return instance;
        }

        public void Write(string Message)
        {
            this.log.Info(Message);
        }
    }
}