using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace VPT_Repacker
{
    internal class Program
    {
        private static string mtg;
        private static string decktypes;
        private static string formats;
        private static string packs;
        private static string sets;
        private static List<string> list = new List<string>();

        private static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                var file = arg.Trim('"');
                if (File.Exists(file))
                {
                    var name = Path.GetFileName(file);
                    if (name == "mtg.jar")
                    {
                        mtg = file;
                    }
                    if (name == "decktypes.xml")
                    {
                        decktypes = file;
                    }
                    else if (name == "formats.xml")
                    {
                        formats = file;
                    }
                    else if (name == "packs.xml")
                    {
                        packs = file;
                    }
                    else if (name == "sets.xml")
                    {
                        sets = file;
                    }
                    else if (file.EndsWith(".vpt.xml"))
                    {
                        list.Add(file);
                    }
                }
            }

            if (string.IsNullOrEmpty(mtg))
            {
                Console.WriteLine("No mtg.jar package specified!");
                do
                {
                    Console.WriteLine("mtg.jar path:");
                    mtg = Console.ReadLine();
                } while (string.IsNullOrEmpty(mtg) || !mtg.Trim('"').EndsWith("mtg.jar") || !File.Exists(mtg.Trim('"')));
                mtg = mtg.Trim('"');
            }

            Console.WriteLine("following files will be repacked in: ");
            if (!string.IsNullOrEmpty(decktypes))
                Console.WriteLine(decktypes);
            if (!string.IsNullOrEmpty(formats))
                Console.WriteLine(formats);
            if (!string.IsNullOrEmpty(packs))
                Console.WriteLine(packs);
            if (!string.IsNullOrEmpty(sets))
                Console.WriteLine(sets);
            foreach (var set in list)
            {
                Console.WriteLine(set);
            }
            Console.WriteLine("Press enter to confirm. Or add more files: ");
            var input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                var file = input.Trim('"');
                if (File.Exists(file))
                {
                    if (file.EndsWith("decktypes.xml"))
                    {
                        decktypes = file;
                        Console.WriteLine("Added");
                    }
                    else if (file.EndsWith("formats.xml"))
                    {
                        formats = file;
                        Console.WriteLine("Added");
                    }
                    else if (file.EndsWith("packs.xml"))
                    {
                        packs = file;
                        Console.WriteLine("Added");
                    }
                    else if (file.EndsWith("sets.xml"))
                    {
                        sets = file;
                        Console.WriteLine("Added");
                    }
                    else if (file.EndsWith(".vpt.xml"))
                    {
                        list.Add(file);
                        Console.WriteLine("Added");
                    }
                    else
                    {
                        Console.WriteLine("Invalid");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid");
                }
                Console.WriteLine("Press enter to confirm. Or add more files: ");
                input = Console.ReadLine();
            }

            Console.WriteLine("processing...");
            Process();
            Console.ReadKey();
        }

        private static void Process()
        {
            if (!string.IsNullOrEmpty(decktypes) || !string.IsNullOrEmpty(formats) || !string.IsNullOrEmpty(packs) ||
                !string.IsNullOrEmpty(sets) || list.Count > 0)
                File.Copy(mtg, mtg.Replace("mtg.jar", "mtg.jar.backup"));
            else
            {
                Console.WriteLine("Nothing to update...Done");
            }

            using (var zip = new ZipFile(mtg))
            {
                zip.BeginUpdate();
                if (!string.IsNullOrEmpty(decktypes))
                {
                    zip.Add(decktypes, "database/decktypes.xml");
                    Console.WriteLine("Added(Replaced) " + decktypes);
                }
                if (!string.IsNullOrEmpty(formats))
                {
                    zip.Add(formats, "database/formats.xml");
                    Console.WriteLine("Added(Replaced) " + formats);
                }
                if (!string.IsNullOrEmpty(packs))
                {
                    zip.Add(packs, "database/packs.xml");
                    Console.WriteLine("Added(Replaced) " + packs);
                }
                if (!string.IsNullOrEmpty(sets))
                {
                    zip.Add(sets, "database/sets.xml");
                    Console.WriteLine("Added(Replaced) " + sets);
                }
                foreach (var set in list)
                {
                    zip.Add(set, "database/sets/" + Path.GetFileName(set).Replace(".vpt.xml", ".xml"));
                    Console.WriteLine("Added(Replaced) " + set);
                }
                zip.CommitUpdate();
            }

            Console.WriteLine("Done");
        }
    }
}