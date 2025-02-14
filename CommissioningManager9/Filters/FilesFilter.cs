using System.Collections.Generic;
using System.IO;

namespace Filters
{
    public class FilesFilter
    {
        public FilesFilter()
        {
            Query = new List<string>();
        }
        public FileInfo InputFiles { get; set; }
        public List<string> Query { get; set; }
    }
}
