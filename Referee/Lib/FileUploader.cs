using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Referee.Lib
{
    public class FileUploader
    {
        public static string DestinationFolder
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["personalPhotoStorage"];
            }
        }

        public string Folder { get; set; }
        public HttpPostedFileBase File { get; set; }

        public string Run()
        {
            if (File != null && File.ContentLength > 0)
            {
                var fileName = Path.GetFileName(File.FileName).ToLower().Replace(" ", "-");
                string destinationPath = GetFolder();
                File.SaveAs(Path.Combine(destinationPath, fileName));
                return fileName;
            }
            return "";
        }

        private string GetFolder()
        {
            string folder = HttpContext.Current.Server.MapPath(this.Folder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public static string GetUserPhotoPath(string userPhoto) 
        {
            return userPhoto.TrimStart(new char[] { '~' });
        }
    }
}