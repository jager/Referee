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
        public ExceptionLogger()
        {
            this.log = LogManager.GetLogger(typeof(ExceptionLogger));
            XmlConfigurator.Configure();
        }

        public void Write(string Message)
        {
            this.log.Error(Message);
        }
    }
}