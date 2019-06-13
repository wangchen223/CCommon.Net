using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    public class FileWatcherHelper
    {
        private Action _action;
        public FileWatcherHelper(string path,Action action)
        {
            _action = action;

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));
            watcher.Filter = System.IO.Path.GetFileName(path);
            watcher.Changed += new FileSystemEventHandler(OnProcess);
            watcher.Deleted += new FileSystemEventHandler(OnProcess);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess

                                   | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.IncludeSubdirectories = true;
        }

        private void OnProcess(object source, FileSystemEventArgs e)
        {
            _action();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            _action();
        }
    }
}
