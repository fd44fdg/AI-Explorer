using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WJJ.AIExplorer;

public static class IconHelper
{
    public static void ConvertPngToIcon(string pngPath, string icoPath)
    {
        if (!File.Exists(pngPath)) return;

        byte[] pngData;
        using (Bitmap bitmap = new Bitmap(pngPath))
        {
            Rectangle bounds = GetContentBounds(bitmap);
            using (Bitmap cropped = bitmap.Clone(bounds, PixelFormat.Format32bppArgb))
            {
                // Make white transparent with fuzzy matching
                using (Bitmap finalized = SetTransparentFuzzy(cropped, Color.White, 30))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        finalized.Save(ms, ImageFormat.Png);
                        pngData = ms.ToArray();
                    }
                }
            }
        }

        // Write ICO header (22 bytes) + PNG data
        using (FileStream fs = new FileStream(icoPath, FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                // Reserved (0), Type (1 = Icon), Count (1)
                bw.Write((short)0); bw.Write((short)1); bw.Write((short)1);

                // Icon Entry
                bw.Write((byte)0); // Width (0 means 256)
                bw.Write((byte)0); // Height (0 means 256)
                bw.Write((byte)0); // Color Count
                bw.Write((byte)0); // Reserved
                bw.Write((short)1); // Color Planes
                bw.Write((short)32); // Bits per pixel
                bw.Write((int)pngData.Length); // Size of data
                bw.Write((int)22); // Offset to image data (header is 22 bytes)

                // Raw PNG data
                bw.Write(pngData);
            }
        }
    }

    private static Rectangle GetContentBounds(Bitmap bmp)
    {
        int minX = bmp.Width, minY = bmp.Height, maxX = -1, maxY = -1;
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color c = bmp.GetPixel(x, y);
                // Detect non-white/non-transparent pixel
                if (c.A > 20 && (c.R < 240 || c.G < 240 || c.B < 240))
                {
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (maxX <= minX || maxY <= minY) return new Rectangle(0, 0, bmp.Width, bmp.Height);
        
        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private static Bitmap SetTransparentFuzzy(Bitmap bmp, Color color, int tolerance)
    {
        Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color c = bmp.GetPixel(x, y);
                // If pixel matches target color within tolerance, make it transparent
                if (Math.Abs(c.R - color.R) < tolerance && 
                    Math.Abs(c.G - color.G) < tolerance && 
                    Math.Abs(c.B - color.B) < tolerance)
                {
                    newBmp.SetPixel(x, y, Color.Transparent);
                }
                else
                {
                    // Copy original pixel
                    newBmp.SetPixel(x, y, c);
                }
            }
        }
        return newBmp;
    }
}
