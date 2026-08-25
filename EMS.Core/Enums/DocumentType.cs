using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EMS.Core.Enums
{
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

    public enum Status
    {
        Approved = 1,
        Pending=2,
        Rejected = 3      
      
    }
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Others = 3
    }
    public enum MaritalStatus
    {

        Single = 1,
        Married = 2,
        Divorced = 3,
        Widowed = 4
    }

    public enum BloodGroup
    {
        [Display(Name = "A+ve")]
        APositive = 1,
        [Display(Name = "A-ve")]
        ANegative = 2,
        [Display(Name = "B+ve")]
        BPositive = 3,
        [Display(Name = "B-ve")]
        BNegative = 4,
        [Display(Name = "AB+ve")]
        ABPositive = 5,
        [Display(Name = "AB-ve")]
        ABNegative = 6,
        [Display(Name = "O+ve")]
        OPositive = 7,
        [Display(Name = "O-ve")]
        ONegative = 8
    }
    public enum DocumentType
    {
        [Display(Name = "Resume")]
        Resume = 1,
        [Display(Name = "ID Proof")]
        IDProof = 2,
        [Display(Name = "HSC Certificate")]
        HSCCertificate = 3,
        [Display(Name = "SSLC Certificate")]
        SSLCCertificate = 4,
        [Display(Name = "UG Certificate")]
        UGDegree = 5,
        [Display(Name = "PG Certificate")]
        PGDegree = 6,
        [Display(Name = "Offer Letter")]
        OfferLetter = 7
    }
    public enum EmploymentType
    {
        [Display(Name = "Full Time")]
        FullTime = 1,
        [Display(Name = "Part Time")]
        PartTime = 2,
        [Display(Name = "Contract")]
        Contract = 3,
        [Display(Name = "Internship")]
        Internship = 4
    }
    public enum EducationType
    {
        [Display(Name = "SSLC")]
        SSLC = 1,
        [Display(Name = "HSC/Diploma")]
        HSCDiploma = 2,
        [Display(Name = "Bachelor")]
        Bachelor = 3,
        [Display(Name = "Master")]
        Master = 4,
    }
}
/*
    Enums
 ↓
Fixed application values
 ↓
EmploymentType
WorkMode
MaritalStatus
BloodGroup

Master / Lookup Tables
 ↓
Business data that HR/Admin can manage
 ↓
JobPosting
Department
Designation
etc.
*/