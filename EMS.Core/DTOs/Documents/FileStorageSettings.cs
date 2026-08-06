using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Documents
{
    public class FileStorageSettings
    {
        public string BasePath { get; set; } = string.Empty;
        public int MaxFileSizeInMB { get; set; }
        public List<string> AllowedExtensions { get; set; } = new();
    }
}
