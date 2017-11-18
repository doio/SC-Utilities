using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScLib
{
    public class Textures
    {
        public static Bitmap GetBitmapBySc(Stream inStream)
        {
            using (var reader = new BinaryReader(inStream))
            {
                var id = reader.ReadByte();
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                var pxFomat = reader.ReadByte();

                ushort width = reader.ReadUInt16(), heigth = reader.ReadUInt16();

                int mtWidth = id == 1 ? width : width % 32,
                    ttWidth = id == 1 ? width - mtWidth : (width - mtWidth) / 32,
                    mtHeigth = id == 1 ? heigth : heigth % 32,
                    ttHeigth = id == 1 ? heigth - mtHeigth : (heigth - mtHeigth) / 32;

                var pixelArray = new Color[heigth, width];

                for (var index = 0; index < ttHeigth + 1; index++)
                {
                    var lHeigth = 32;

                    if (index == ttHeigth)
                        lHeigth = mtHeigth;

                    for (var t = 0; t < ttWidth; t++)
                    for (var y = 0; y < lHeigth; y++)
                    for (var x = 0; x < 32; x++)
                    {
                        var xOffset = t * 32;
                        var yOffset = index * 32;

                        pixelArray[y + yOffset, x + xOffset] = GetColorByPxFormat(reader, pxFomat);
                    }

                    for (var y = 0; y < lHeigth; y++)
                    for (var x = 0; x < mtWidth; x++)
                    {
                        int pxOffsetX = ttWidth * 32, pxOffsetY = index * 32;

                        pixelArray[y + pxOffsetY, x + pxOffsetX] = GetColorByPxFormat(reader, pxFomat);
                    }
                }

                var bitmap = new Bitmap(width, heigth, PixelFormat.Format32bppArgb);

                for (var row = 0; row < pixelArray.GetLength(0); row++)
                for (var column = 0; column < pixelArray.GetLength(1); column++)
                    bitmap.SetPixel(column, row, pixelArray[row, column]);

                return bitmap;
            }
        }

        private static Color GetColorByPxFormat(BinaryReader reader, int pxFormat)
        {
            Color Color;

            switch (pxFormat)
            {
                case 0:
                {
                    var r = reader.ReadByte();
                    var g = reader.ReadByte();
                    var b = reader.ReadByte();
                    var a = reader.ReadByte();

                    Color = Color.FromArgb((a << 24) | (r << 16) | (g << 8) | b);

                    break;
                }

                case 2:
                {
                    var color = reader.ReadUInt16();

                    var r = ((color >> 12) & 0xF) << 4;
                    var g = ((color >> 8) & 0xF) << 4;
                    var b = ((color >> 4) & 0xF) << 4;
                    var a = (color & 0xF) << 4;

                    Color = Color.FromArgb(a, r, g, b);

                    break;
                }

                case 4:
                {
                    var color = reader.ReadUInt16();

                    var r = ((color >> 11) & 0x1F) << 3;
                    var g = ((color >> 5) & 0x3F) << 2;
                    var b = (color & 0X1F) << 3;

                    Color = Color.FromArgb(r, g, b);

                    break;
                }

                case 6:
                {
                    var color = reader.ReadUInt16();

                    var r = color >> 8;
                    var g = color >> 8;
                    var b = color >> 8;
                    var a = color >> 0xFF;

                    Color = Color.FromArgb(a, r, g, b);

                    break;
                }

                default:
                {
                    throw new Exception("Unknown pixelformat.");
                }
            }

            return Color;
        }
    }
}