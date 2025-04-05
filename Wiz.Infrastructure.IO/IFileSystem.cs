using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiz.Infrastructure.IO
{
    public interface IFileSystem
    {
        /// <summary>
        /// Copies a folder with content and sub-folders to a temporary path
        /// </summary>
        /// <param name="path">Path to the directory to be copied</param>
        /// <returns>Path to the temp folder</returns>
        string CopyToTempPath(string path);

        /// <summary>
        /// Checks if a file exists
        /// </summary>
        /// <param name="path">Path for the file to check for existence</param>
        /// <returns>True if the file exists</returns>
        bool Exists(string path);

        /// <summary>
        /// Opens a stream to an existing file
        /// </summary>
        /// <param name="path">Path to the file to be opened</param>
        /// <returns>Stream to the opened file</returns>
        Stream OpenRead(string path);

        /// <summary>
        /// Creates a stream to a new file. 
        /// If the file already exists it will be overwritten
        /// </summary>
        /// <param name="path">Path to the file to be created</param>
        /// <returns>Open stream for writing to the file</returns>
        Stream Create(string path);
    }
}
