using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace Referee.Lib.Photo
{
    public class CheckPhoto
    {
        public static bool isImage(string PathToImage)
        {
            Image image = Image.FromFile(HttpContext.Current.Server.MapPath(PathToImage));

            if (image.RawFormat.Guid == ImageFormat.Jpeg.Guid)
            {
                return true;
            }
            else if (image.RawFormat.Guid == ImageFormat.Png.Guid)
            {
                return true;
            }
            return false;

            /*
                if (img.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid) {do something}

               // Or, if you want to return the type of the image, try this:

                    public static string MimeType(System.Drawing.Image imgPhoto)
                    {
                        foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                        {
                            if (codec.FormatID == imgPhoto.RawFormat.Guid)
                                return codec.MimeType;
                        }
                        return "image/unknown";
                    }
                        }
             * */
        }
    }
}