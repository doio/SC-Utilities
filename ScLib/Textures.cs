using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ScLib.SC;

namespace ScLib
{
    public class Textures
    {
        private static readonly int[] Convert5To8 =
        {
            0x00, 0x08, 0x10, 0x18, 0x20, 0x29, 0x31, 0x39, 0x41, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x73, 0x7B, 0x83, 0x8B,
            0x94, 0x9C, 0xA4, 0xAC, 0xB4, 0xBD, 0xC5, 0xCD, 0xD5, 0xDE, 0xE6, 0xEE, 0xF6, 0xFF
        };

        /// <summary>
        ///     Exports the texture from a SC File
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
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

        public static ScInfoFile ReadScInfo(Stream inStream)
        {
            var info = new ScInfoFile();

            using (var reader = new BinaryReader(inStream))
            {
                info.ShapeCount = reader.ReadUInt16();
                info.MovieClipCount = reader.ReadUInt16();
                info.TextureCount = reader.ReadUInt16();
                info.TextCount = reader.ReadUInt16();
                info.MatrixCount = reader.ReadUInt16();
                info.ColorTransformCount = reader.ReadUInt16();

                if (reader.BaseStream.CanSeek)
                    reader.BaseStream.Seek(5, SeekOrigin.Current);
                else
                    throw new Exception("Can't skip bytes.");

                info.ExportCount = reader.ReadUInt16();

                for (var i = 0; i < info.ExportCount; i++)
                    info.Exports.Add(new Export
                    {
                        Id = reader.ReadUInt16()
                    });

                for (var i = 0; i < info.ExportCount; i++)
                    info.Exports[i].Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadByte()));

                /*if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var blockType = reader.ReadByte();
                    var blockSize = reader.ReadUInt32();

                    Console.WriteLine($"Type: {blockType}, Size: {blockSize}.");

                    switch (blockType)
                    {
                        case 23:
                        {
                            break;
                        }
                    }
                }*/
            }

            return info;
        }

        /// <summary>
        ///     Get the color by the given pixel format
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pxFormat"></param>
        /// <returns></returns>
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

                case 3:
                {
                    var color = reader.ReadUInt16();

                    var red = Convert5To8[(color >> 11) & 0x1F];
                    var green = Convert5To8[(color >> 6) & 0x1F];
                    var blue = Convert5To8[(color >> 1) & 0x1F];
                    var alpha = (color & 0x0001) == 1 ? 0xFF : 0x00;

                    Color = Color.FromArgb(alpha, red, green, blue);

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

                    var rgb = color >> 8;

                    Color = Color.FromArgb(color >> 0xFF, rgb, rgb, rgb);

                    break;
                }

                case 10:
                {
                    var color = reader.ReadByte();

                    Color = Color.FromArgb(color, color, color);

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