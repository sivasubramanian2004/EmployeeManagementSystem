using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.Enums;
using Microsoft.AspNetCore.Http;
namespace EMS.Core.DTOs.Documents
{
    public class UploadDocumentDto
    {
        public int EmployeeId { get; set; }
        public DocumentType DocumentType { get; set; }    // 'Resume','IDProof','Certificate','OfferLetter'
        public IFormFile File { get; set; } = null!;
    }
}
