using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rox.Core;
using Rox.FileSys;

namespace rox
{
    class Program
    {
        static Repo _repo;

        static void Main(string[] args)
        {
            Factory.RegisterAll();
            if (args.Contains("--usage"))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine($" {System.AppDomain.CurrentDomain.FriendlyName} [<.rox file name>] run the program");
                Console.WriteLine($" {System.AppDomain.CurrentDomain.FriendlyName} --usage            show this help");
                return;
            }
            string dbfn;
            if (args.Length < 1)
            {
                dbfn = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "default.rox");
                Console.WriteLine($".rox file not provided, using default ({dbfn}).");
            }
            else
            {
                dbfn = args[0];
            }
            var dbExists = System.IO.File.Exists(dbfn);
            _repo = new Repo();
            if (dbExists)
            {
                using (var fs = new FileStream(dbfn, FileMode.Open))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        _repo.Deserialize(br);
                    }
                }
            }
            var watching = new Watching(_repo);

            bool quit = false;
            while (!quit)
            {
                Console.Write("Command>");
                var rl = Console.ReadLine().Trim().ToLower();
                switch (rl)
                {
                    case "add":
                    case "a":
                        Add();
                        break;
                    case "remove":
                    case "r":
                        Remove();
                        break;
                    case "list":
                    case "l":
                        List();
                        break;
                    case "help":
                    case "h":
                        Console.WriteLine("a[dd]: add file with tags;");
                        Console.WriteLine("r[emove]: remove tags from file;");
                        Console.WriteLine("l[ist]: list files;");
                        Console.WriteLine("q[uit]: quit the program and save to db.");
                        Console.WriteLine("h[elp]: show show help.");
                        break;
                    case "quit":
                    case "q":
                        quit = true;
                        break;
                }
            }

            using (var fs = new FileStream(dbfn, FileMode.Create))
            {
                using (var br = new BinaryWriter(fs))
                {
                    _repo.Serialize(br);
                }
            }
        }
        static void Add()
        {
            if (PromptFileAndTags(out var fn, out var tags))
            {
                var kf = new Rox.Core.File{Path = fn};
                if (_repo.All.TryGetValue(kf, out var f))
                {
                    kf = (Rox.Core.File)f;
                }
                kf.Tags.UnionWith(tags.Select(t=>new Tag(t)));
                _repo.AddItem(kf);
            }
        }
        static void Remove()
        {
            if (PromptFileAndTags(out var fn, out var tags))
            {
                var kf = new Rox.Core.File{Path = fn};
                if (_repo.All.TryGetValue(kf, out var f))
                {
                    if (tags.Count > 0)
                    {
                        _repo.UnlinkItemTags(f, tags.Select(t=>new Tag(t)), false);
                        if (f.Tags.Count == 0)
                        {
                            _repo.RemoveItem(f);
                        }
                    }
                    else
                    {
                        Console.Write("No tags specified, delete the file?(Yes/no)>");
                        var rl = Console.ReadLine();
                        if (rl == "Yes")
                        {
                            _repo.RemoveItem(f);
                        }
                    }
                }
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
            return !rl.Contains('|')? rl : null;
        }
        static List<string> PromptTags()
        {
            Console.Write("Tags (separate with comma, put '|' to canel)>");
            var rl = Console.ReadLine().Trim();
            return !rl.Contains('|')? rl.Split(',').Select(x=>x.Trim()).ToList() : null;
        }

        static void List()
        {
            Console.Write("Search by tags (1) or file path regex pattern (2)>");
            var r = Console.ReadLine().Trim();
            switch (r)
            {
                case "1":
                    ListByTags();
                    break;
                case "2":
                    ListByFiles();
                    break;
            } 
        }

        static void ListByTags()
        {
            Console.Write("Tags>");
            var r = Console.ReadLine();
            var tags = r.Split(',');
            var files = tags.Select(t=>new Tag(t)).Select(t=> _repo.Dict.TryGetValue(t, out var col)? col: new HashSet<Item>{})
                .Aggregate((x,y)=>(HashSet<Item>)x.OfType<Rox.Core.File>().Union(y.OfType<Rox.Core.File>())).Cast<Rox.Core.File>(); 
            ListFiles(files);
        }

        static void ListByFiles()
        {
            Console.Write("File path regex>");
            var r = Console.ReadLine();
            var rex = new Regex(r);
            var files = _repo.All.OfType<Rox.Core.File>()
                .Where(x=> rex.Match(x.Path).Length==x.Path.Length).OrderBy(x=>x.Path);
            ListFiles(files);
        }

        static void ListFiles(IEnumerable<Rox.Core.File> files)
        {
            var c = 0;
            foreach (var f in files)
            {
                Console.Write($"{f.Path}:");
                Console.WriteLine($"{f.Tags.Select(x=>x.Title).Aggregate((a,b)=>a+","+b)}");
                c++;
            }
            Console.WriteLine($"{c} files in total listed");
        }
    }
}
