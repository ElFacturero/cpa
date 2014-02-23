using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Tesseract;

namespace CPAServices
{
    public static class OCRHelper
    {
        private static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        public static String GetText(byte[] img)
        {
            return GetText(ByteArrayToImage(img));
        }

        public static String GetText(Image img)
        {
            var builder = new StringBuilder();
            using (var engine = new TesseractEngine(Path.Combine(HttpRuntime.AppDomainAppPath, "tessdata"), "eng", EngineMode.Default))
            {
                // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                using (var image = new System.Drawing.Bitmap(img))
                {
                    using (var pix = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(pix))
                        {
                            builder.Append(page.GetText());
                        }
                    }
                }
            }
            return builder.ToString();
        }

    }
}