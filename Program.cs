using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rox.Core;

namespace rox
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <db file name>");
                return;
            }
            var repo = new Repo();
            var fn = args[0];
            using (var fs = new FileStream(fn, FileMode.OpenOrCreate))
            {
                using (var br = new BinaryReader(fs))
                {
                    repo.Deserialize(br);
                }
            }

            while (true)
            {
                Console.Write("Command>");
                var rl = Console.ReadLine().Trim().ToLower();
                switch (rl)
                {
                    case "add":
                        Add();
                        break;
                    case "remove":
                        Remove();
                        break;
                    case "help":
                        Console.WriteLine("add: add file with tags;");
                        Console.WriteLine("remove: remove tags from file.");
                        break;
                }
            }
        }
        static void Add()
        {
            if (PromptFileAndTags(out var fn, out var tags))
            {
                // TODO...
            }
        }
        static void Remove()
        {
            if (PromptFileAndTags(out var fn, out var tags))
            {
                // TODO...                
            }
        }
        static bool PromptFileAndTags(out string fn, out List<string> tags)
        {
            fn = null;
            tags = null;
            fn = PromptFile();
            if (fn == null)
            {
                Console.WriteLine("User cancelled");
                return false;
            }
            tags = PromptTags();
            if (tags == null)
            {
                Console.WriteLine("User cancelled");
                return false;
            }
            return true;
        }
        static string PromptFile()
        {
            Console.Write("File Name (put '|' to cancel)>");
            var rl = Console.ReadLine().Trim();
            return rl.Contains('|')? rl : null;
        }
        static List<string> PromptTags()
        {
            Console.Write("Tags (separate with comma, put '|' to canel)>");
            var rl = Console.ReadLine().Trim();
            return rl.Contains('|')? rl.Split(',').Select(x=>x.Trim()).ToList() : null;
        }
    }
}
