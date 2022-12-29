using System.IO;
using Rox.Core;

using static Rox.Helpers.FileHelper;
using static Rox.Helpers.StringHelper;

namespace Rox.AutoTagging
{
    class SmartTag
    {
        class WordInRepo : IWordsInRepo
        {
            Repo _repo;
            public WordInRepo(Repo repo)
            {
                _repo = repo;
            }

            public bool Add(string s)
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(string s)
            {
                throw new System.NotImplementedException();
            }
        }

        private WordInRepo _wordInRepo;

        public SmartTag(Repo repo)
        {
            _wordInRepo = new WordInRepo(repo);
        }

        public void Scan(string root_dir)
        {
            var d = new DirectoryInfo(root_dir);
            
            SequentialTraverseFiles(d, (f, relPath)=>
            {
                relPath.Add(f.Name);
                Tokenize(relPath, _wordInRepo);
                relPath.RemoveAt(relPath.Count-1);
            });
        }
    }
}
