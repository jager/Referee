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
        public LogHelper(int output = (int)LogOutput.File)
        {
            _output = output;
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
            string _fileName = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
            if (String.IsNullOrEmpty(_fileName))
            {
                return;
            }

            _fileName = HttpContext.Current.Server.MapPath(_fileName);
            string _text = String.Format("{0};{1}", DateTime.Now.ToString(), Text);
            if (File.Exists(_fileName))
            {
                File.AppendAllText(_fileName, _text);
            }
            else
            {
                File.WriteAllText(_fileName, _text);
            }
        }

        private void WriteToDatabase(string Text)
        {

        }
    }
}