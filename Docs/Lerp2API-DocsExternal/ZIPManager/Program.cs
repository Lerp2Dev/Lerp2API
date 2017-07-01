using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZIPManager
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("You must pass at least 3 parameters to this app.\nThe first is to know if you are compressing (-c) or decompressing (-d).\nThe second is the output file.\nAnd if you want to decompress, specify the following as the folders or files that you want to compress.\n\nPress any key to exit...");
                GoToExit();
            }
            else
            {
                if (args.Length == 3 && args[0] == "-d")
                {
                    //Decompress...

                    string filePath = args[1],
                           outputPath = args[2];

                    if (IsValidPath(filePath) && IsValidPath(outputPath) && File.Exists(filePath) && Path.GetExtension(filePath).ToUpper() == "ZIP")
                        using (var archive = ZipArchive.Open(new MemoryStream(File.ReadAllBytes(filePath))))
                            foreach (ZipArchiveEntry entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                entry.WriteToDirectory(outputPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                    else
                    {
                        Console.WriteLine("Invalid paths specified!");
                        GoToExit();
                    }
                }
                else if (args[0] == "-c")
                {
                    //Compress...

                    string filePath = args[1];
                    IEnumerable<string> outputPath = args.Skip(2);

                    using (var archive = ZipArchive.Create())
                    {
                        foreach (string path in outputPath)
                            if (IsFolder(path))
                                archive.AddAllFromDirectory(path);
                        archive.SaveTo(filePath, CompressionType.Deflate);
                    }
                }
            }
        }

        private static bool IsValidPath(string path)
        {
            try
            {
                return !string.IsNullOrEmpty(Path.GetFullPath(path));
            }
            catch
            {
                return false;
            }
        }

        private static bool IsFolder(string path)
        {
            return IsValidPath(path) && File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        private static void GoToExit()
        {
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}