using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
// 2017 Revisions
// NB Break points are on all code paths.  disabled once it has been tested.
//  3/16 JMz    copied from Blog project
//  3.1?    Jacob Crawford  posted on Slack [10:50 AM] from Antonio Lecture
namespace BugTracker306.Helpers
{
    public static class ImageValidator
    {
        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            // Exception Ex= new Exception;
            if (file == null) // throw new Exception { message = "file is null [E10913305]" };
                return false;
            if (file.ContentLength > 3 * 1024 * 1024 || file.ContentLength < 1024)
                return false;
            if (file.FileName.Contains(".jpg")
            ||  file.FileName.Contains(".png")
            ||  file.FileName.Contains(".gif")
            ||  file.FileName.Contains(".JPG")
            ||  file.FileName.Contains(".PNG")  )
                return IsWebFriendlyFile(file);
            return true;
        }

        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            if (file.ContentLength > 3 * 1024 * 1024 || file.ContentLength < 1024)
                return false;
            try
            {
                using (var img = System.Drawing.Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                        ImageFormat.Png.Equals(img.RawFormat) ||
                        ImageFormat.Gif.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}