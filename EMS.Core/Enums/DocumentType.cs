using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.Enums
{
    public enum DocumentType
    {
        Resume = 1,
        IDProof = 2,
        HSCCertificate = 3,
        SSLCCertificate = 4,
        UGDegree = 5,
        PGDegree = 6,
        OfferLetter = 7
    }
    public enum EmploymentType
    {
        FullTime = 1,
        PartTime = 2,
        Contract = 3,
        Internship = 4
    }

    public enum WorkMode
    {
        Onsite = 1,
        Remote = 2,
        Hybrid = 3
    }

    public enum JobPostingStatus
    {
        Open = 1,
        Closed = 2,
        OnHold = 3
    }
}
