using System;
using System.IO;
using ExifLib;

namespace OrganizePhotos
{
    class Program
    {
        //todo: delete old (empty) directories
        static void Main(string[] args)
        {
            string folder = args.Length == 0 ? "." : args[0];
            string ext = "*.jpg";

            // create a new directory to hold all of the organized photos, this directory will be ignored when the program is run multiple times
            // this is the directory where the photos will be stored
            string creDirectory = folder + Path.DirectorySeparatorChar + "OrganizePhotos";
            Directory.CreateDirectory(creDirectory);
            Console.WriteLine("Cre dir: " + creDirectory);

            foreach (string dir in Directory.EnumerateDirectories(folder))
            {
                // skip the creDirectory
                if (dir == creDirectory) { continue; }

                foreach (string file in Directory.EnumerateFiles(dir, ext, SearchOption.AllDirectories))
                {
                    int dupSeq = 0;
                    DateTime dateTime;
                    using (var reader = new ExifReader(file))
                    {
                        if (!reader.GetTagValue(ExifTags.DateTimeOriginal, out dateTime)) continue;
                    }

                    string subFolder = Path.Combine(creDirectory, dateTime.ToString("yyyy-MM-dd"));
                    Directory.CreateDirectory(subFolder);
                    string newFileName = Path.Combine(subFolder, Path.GetFileName(file));

                    if (File.Exists(newFileName))
                    {
                        // a sequential number is appended to each duplicate file name (may not be the same photo)
                        string seqFileName = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(newFileName), ++dupSeq, Path.GetExtension(newFileName));
                        newFileName = Path.Combine(subFolder, Path.GetFileName(seqFileName));

                        while (File.Exists(newFileName))
                        {
                            seqFileName = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(file), ++dupSeq, Path.GetExtension(newFileName));
                            Console.WriteLine("duplicate file {0} - renaming {1}", newFileName, seqFileName);
                            newFileName = Path.Combine(subFolder, Path.GetFileName(seqFileName));
                        }
                    }
                    try
                    {
                        File.Move(file, newFileName);
                    }
                    catch (Exception m)
                    {
                        Console.WriteLine(m.Message);
                        Console.WriteLine(newFileName);
                    }
                }
            }
        }
    }
}
