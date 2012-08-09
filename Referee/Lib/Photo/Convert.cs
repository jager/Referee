using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Referee.Lib.Photo
{
    public class Convert
    {
        string _photo;
        public Convert(string Photo)
        {
            this._photo = Photo;
        }

        public string Run()
        {
            Image image = Image.FromFile(HttpContext.Current.Server.MapPath(this._photo));
            if (image.Width > image.Height)
            {

            }
            else
            {

            }

            return "";
        }

        private void ConvertJPG(Image img)
        {
           
        }

        private void ConvertPNG(Image img)
        {
        }
    }
}