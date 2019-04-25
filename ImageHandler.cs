using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Text;
using System.IO;

using Microsoft.AspNetCore.Http;

using System.Drawing;
using System.Drawing.Imaging;

namespace JustinBB
{
    public class ImageHandler
    {
        // Save the uploaded image to disk, and also create and save the thumbnail version.
        public static async Task SaveFile(IFormFile f, string ImageID)
        {
            var uploads = Path.Combine("./uploads");
            if (f != null && f.Length > 0)
            {
                var filePath = Path.Combine(uploads, ImageID);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await f.CopyToAsync(fileStream);
                }
                ResizeAndSaveFile(f, ImageID);
            }
        }

        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            // see http://www.mikekunz.com/image_file_header.html
            var bmp    = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif    = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png    = new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff   = new byte[] { 73, 73, 42 };         // TIFF
            var tiff2  = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg   = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2  = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }

        private static byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int) image.Length);
            return CoverImageBytes;
        }

        public static bool ValidateImage(IFormFile image)
        {
            var bytes = ConvertToBytes(image);
            return GetImageFormat(bytes) != ImageFormat.unknown;
        }

        public static void ResizeAndSaveFile(IFormFile file, string ImageID)
        {
            var maxThumbW = 128;
            var maxThumbH = 128;
            Image image = Image.FromStream(file.OpenReadStream(), true, true);

            var newW = 0;
            var newH = 0;

            // Resize proportionately.
            if (image.Width <= maxThumbW && image.Height <= maxThumbH)
            {
                newW = image.Width;
                newH = image.Height;
            }
            else
            {
                if (image.Width <= image.Height)
                {
                    newH = maxThumbH;
                    newW = image.Width / (image.Height / maxThumbH);
                }
                else
                {
                    newW = maxThumbH;
                    newH = image.Height / (image.Width / maxThumbH);
                }
            }

            var newImage = new Bitmap(newW, newH);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image , 0, 0, newW, newH);
            }

            newImage.Save("./uploads/thumbnails/" + ImageID, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
