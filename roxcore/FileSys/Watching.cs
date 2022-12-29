using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Rox.Core;

namespace Rox.FileSys
{
    public class Watching
    {
        private FileSystemWatcher _watcher = new FileSystemWatcher();

        private Repo _repo;

        private bool _watching = false; 

        public Watching(Repo repo, bool startNow = true)
        {
            _repo = repo;
            _repo.ItemCollectionChanged += OnRepoItemCollectionChanged;
        
            SetUpWatcherEventHandler();
            
            if (startNow)
            {
                Start();
            }
        }

        private void SetUpWatcherEventHandler()
        {
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnRenamed;
        }

        private void ResetWatcherPath()
        {
            UpdateWatcherPath(_repo.All.OfType<Rox.Core.File>(), null);
        }

        private void UpdateWatcherPath(IEnumerable<Rox.Core.File> files, string commonDir)
        {
            _watcher.EnableRaisingEvents=false;
            foreach (var f in files)
            {
                UpdateCommon(ref commonDir, GetNormalizedDirectory(f.Path));
            }
            _watcher.Path = commonDir;
            System.Console.WriteLine($"Watching directory set to: {commonDir}");
            if(_watching && commonDir != null)
            {
                _watcher.EnableRaisingEvents=true;
            }
        }

        private void OnRepoItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    UpdateWatcherPath(e.NewItems.OfType<Rox.Core.File>(), _watcher.Path);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Do nothing as it's hard to update
                    //TODO Optimize this...
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetWatcherPath();
                    break;
            }
        }

        public void Start()
        {
            _watching=true;
            if (!string.IsNullOrWhiteSpace(_watcher.Path))
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            _watching=false;
            _watcher.EnableRaisingEvents = false;
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var key = new Rox.Core.File { Path = e.OldFullPath};
            _repo.ModifyItem(key, (actual)=>
            {
                ((Rox.Core.File)actual).Path = e.FullPath;
            });
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                {
                    var key = new Rox.Core.File { Path = e.FullPath};
                    _repo.RemoveItem(key);
                    break;
                }
            }
        }

        private string GetNormalizedDirectory(string filepath)
        {
            return Path.GetFullPath(Path.GetDirectoryName(filepath));
        }

        private void UpdateCommon(ref string commonDir, string newDir)
        {
            if (commonDir == null)
            {
                commonDir = newDir;
            }
            else
            {
                var i = 0;
                for (; i < commonDir.Length; i++)
                {
                    if (commonDir[i] != newDir[i])
                    {
                        break;
                    }
                }
                commonDir = commonDir.Substring(0, i);
            }
        }
    }
}
