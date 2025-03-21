using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiz.Infrastructure.IO
{
    public class FileSystem : IFileSystem
    {
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
    }
}
