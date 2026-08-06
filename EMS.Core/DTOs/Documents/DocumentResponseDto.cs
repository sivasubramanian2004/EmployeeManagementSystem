using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Documents
{
    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
        public string FileUrl { get; set; } = string.Empty;   // ← puthusa
    }
}
