using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Referee.Helpers
{
    enum LogOutput : int
    {
        File = 1,
        Database = 2
    }
    public class LogHelper
    {
        private int _output = (int)LogOutput.File;
        private string _destinationFileName = String.Empty;

        public LogHelper()
        {
        }

        public LogHelper(int output = (int)LogOutput.File)
        {
            this._output = output;
        }

        public LogHelper(string DestinationFileName)
        {
            this._output = (int)LogOutput.File;
            this._destinationFileName = DestinationFileName;
        }

        public void Write(string OutputText)
        {
            switch (this._output)
            {
                case (int)LogOutput.Database:
                    this.WriteToDatabase(OutputText);
                    break;
                case (int)LogOutput.File:
                default:
                    this.WriteToFile(OutputText);
                    break;
            }
        }

        private void WriteToFile(string Text)
        {
            if (String.IsNullOrEmpty(this._destinationFileName))
            {
                this._destinationFileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["LogFilePath"]);
            }

            if (String.IsNullOrEmpty(this._destinationFileName))
            {
                return;
            }

            string _text = String.Format("{0};{1}", DateTime.Now.ToString(), Text);
            if (File.Exists(this._destinationFileName))
            {
                File.AppendAllText(this._destinationFileName, _text);
            }
            else
            {
                File.WriteAllText(this._destinationFileName, _text);
            }
        }

        private void WriteToDatabase(string Text)
        {

        }
    }
}