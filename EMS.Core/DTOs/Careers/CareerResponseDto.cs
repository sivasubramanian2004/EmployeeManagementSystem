using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Careers
{
   
        public class CareerResponseDto
        {
            public int Id { get; set; }

            public string Email { get; set; } = string.Empty;

            public string? Mobile { get; set; }

            public string JobApplicationPosition { get; set; } = string.Empty;

            //public string? LinkedInUrl { get; set; }

            //public string? GitHubUrl { get; set; }

            // Full URL returned to client
            public string? PhotoUrl { get; set; }

            public string ResumeUrl { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; }
        }
   
}
