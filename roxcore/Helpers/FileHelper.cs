using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rox.Helpers
{
    static class FileHelper
    {
        public delegate void Visit(FileInfo file, IList<string> relPath);
        public delegate bool Cancel();

        static bool DefaultCancel() => false;

        public static void SequentialTraverseFiles(DirectoryInfo dir, 
            Visit visit, Cancel cancel, IList<string> relPathFromBase)
        {
            foreach (var f in dir.GetFiles())
            {
                visit(f, relPathFromBase);
            }
            if (!cancel())
            {
                foreach (var d in dir.GetDirectories())
                {
                    relPathFromBase.Add(d.Name);
                    SequentialTraverseFiles(d, visit, cancel, relPathFromBase);
                    relPathFromBase.RemoveAt(relPathFromBase.Count-1);
                    if (cancel())
                    {
                        break;
                    }
                }
            }
        }

        public static void SequentialTraverseFiles(DirectoryInfo dir, Visit visit, Cancel cancel)
            => SequentialTraverseFiles(dir, visit, cancel, new List<string>());

        public static void SequentialTraverseFiles(DirectoryInfo dir, Visit visit)
            => SequentialTraverseFiles(dir, visit, DefaultCancel);

        // TODO 
        public static void ParallelTraverseFiles(DirectoryInfo dir, Visit visit, ParallelOptions po,
            IList<string> relPathFromBase)
        {
            Parallel.ForEach(dir.GetFiles(), po, f=>
            {
                visit(f, relPathFromBase);
            });
            Parallel.ForEach(dir.GetDirectories(), d=>
            {
                var newList = new string[relPathFromBase.Count+1];
                relPathFromBase.CopyTo(newList, 0);
                newList[newList.Length-1] = d.Name;
                ParallelTraverseFiles(d, visit, po, newList); 
            });
        }
    }
}
