using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiz.Infrastructure.IO
{
    public class FileSystem : IFileSystem
    {
        public string CopyToTempPath(string path)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            CopyDirectory(path, tempPath);

            return tempPath;
        }


        public Stream Create(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            return File.Create(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(targetDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }
    }
}
