using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;

namespace Referee.Helpers
{
    public class InformationLogger
    {
        private ILog log;

        public InformationLogger()
        {
            this.log = LogManager.GetLogger(typeof(InformationLogger));
            XmlConfigurator.Configure();
        }

        public void Write(string Message)
        {
            this.log.Info(Message);
        }
    }
}