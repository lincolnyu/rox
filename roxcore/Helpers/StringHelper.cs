using System.Collections.Generic;

namespace Rox.Helpers
{
    static class StringHelper
    {
        public interface IWordsInRepo
        {
            bool Contains(string s);
            bool Add(string s);
        }
        public delegate bool ContainsWord(string s);
        public static HashSet<string> Tokenize(IList<string> str, IWordsInRepo wordsInRepo)
        {
            var res = new HashSet<string>();
            var separators= new char[] {' ', '_'};
            foreach (var s in str)
            {
                var segs = s.Split(separators);
                foreach(var seg in segs)
                {
                    if (wordsInRepo.Contains(seg)
                        || wordsInRepo.Add(seg))
                    {
                        res.Add(seg);
                    }
                }
            }
            return res;
        }
    }
}
