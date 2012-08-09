using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Referee.Lib.Photo
{
    public class ConvertUserPhoto
    {
        string _photo;
        string _destination;
        string _fullPath;
        string _finalPhoto = "avatar.jpg";

        public string PhotoName
        {
            get
            {
                return this._finalPhoto;
            }
        }

        public ConvertUserPhoto(string Destination, string Photo )
        {
            this._photo = Photo;
            this._destination = Destination;
            this._fullPath = String.Format("{0}{1}", this._destination, this._photo);
        }


        /// <summary>
        /// Converting referee image to given width and height
        /// </summary>
        /// <returns>void</returns>
        public void Run(int destWidth, int destHeight)
        {
            Image image = Image.FromFile(HttpContext.Current.Server.MapPath(this._fullPath));
            
            Size destSize = new Size(destWidth, destHeight);
            Rectangle destCropArea = new Rectangle(0, 0, destWidth, destHeight);

            image = resizeImage(image, destSize);
            if (image.Height > destHeight || image.Width > destWidth)
            {
                image = cropImage(image, destCropArea);
            }

            image.Save(HttpContext.Current.Server.MapPath(String.Format("{0}{1}", this._destination, this.PhotoName)));       
        }

        /// <summary>
        /// Method crops given image to Rectangle given area
        /// </summary>
        /// <param name="img">Image to crop</param>
        /// <param name="cropArea">Crop destination arrea</param>
        /// <returns></returns>
        private Image cropImage(Image img, Rectangle cropArea)
        {
            cropArea.Width = (cropArea.Width > img.Width) ? img.Width : cropArea.Width;
            cropArea.Height = (cropArea.Height > img.Height) ? img.Height : cropArea.Height;
            Bitmap bmpImage = new Bitmap(img);

            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }


        /// <summary>
        /// Method resizes the image with constarining image propotions
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH > nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            Image img = (Image)b;
            return (Image)b;
        }

        private void saveJpeg(string path, Image img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
    }
}