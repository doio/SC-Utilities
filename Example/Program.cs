using System;
using System.Diagnostics;
using System.IO;
using ScLib;

namespace Example
{
    internal class Program
    {
        /// Just an example how you could use this library.    
        /// 1. Place a Texture from Clash Royale, Boom Beach or Hayday in the folder
        /// 2. Run the Program
        /// 3. A PNG file has been created and the .SC file has been deleted.
        /// <summary>
        ///     Entry point of the program.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Console.Title = "Example of SC-Utilities";

            if (!Directory.Exists("Files"))
                Directory.CreateDirectory("Files");

            // Decmpress and export SC File
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/Files"))
                if (file.EndsWith("_tex.sc"))
                {
                    try
                    {
                        using (var compression = new Compression())
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open))
                            {
                                Textures.GetBitmapBySc(
                                        new MemoryStream(compression.Decompress(fileStream,
                                            States.CompressionType.Lzmha)))
                                    .Save(file.Replace(".sc", ".png"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    File.Delete(file);
                }
                else if (file.EndsWith(".sc"))
                {
                    File.Delete(file);
                }

            // Compress CSV File Example
            /*foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/Files"))
                if (file.EndsWith(".csv"))
                {
                    try
                    {
                        using (var compression = new Compression())
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open))
                            {
                                using (var fileStreamNew = new FileStream(file.Replace(".csv", "_compressed.csv"), FileMode.Create))
                                {
                                    var buffer = compression.Compress(fileStream, States.CompressionType.Lzma);

                                    fileStreamNew.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }

                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.ReadKey();
                    }
                }*/

            // Decompress SC File Example
            /*foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/Files"))
                if (file.EndsWith(".sc"))
                {
                    try
                    {
                        using (var compression = new Compression())
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open))
                            {
                                var buffer = compression.Decompress(fileStream, States.CompressionType.Lzmha);

                                fileStream.Close();

                                using (var fileStreamNew = new FileStream(file, FileMode.Create))
                                {                                 
                                    fileStreamNew.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.ReadKey();
                    }
                }*/

            // Compress SC File Example
            /*foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/Files"))
                if (file.EndsWith(".sc"))
                {
                    try
                    {
                        using (var compression = new Compression())
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open))
                            {
                                using (var fileStreamNew = new FileStream(file.Replace("_decompressed.sc", "_compressed.sc"), FileMode.Create))
                                {
                                    var buffer = compression.Compress(fileStream, States.CompressionType.Lzma);

                                    fileStreamNew.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }

                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.ReadKey();
                    }
                }*/
        }
    }
}