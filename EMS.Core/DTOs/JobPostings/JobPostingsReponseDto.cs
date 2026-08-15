using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.JobPostings
{
    public class JobPostingsReponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string EmploymentType { get; set; } = null!;
        public string WorkMode { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
