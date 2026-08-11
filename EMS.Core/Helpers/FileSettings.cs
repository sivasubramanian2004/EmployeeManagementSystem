using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.Helpers
{
    public class FileSettings
    {
        public string BasePath { get; set; } = string.Empty;
        public int MaxFileSizeInMB { get; set; }
        public List<string> AllowedExtensions { get; set; } = new();
    }
}
