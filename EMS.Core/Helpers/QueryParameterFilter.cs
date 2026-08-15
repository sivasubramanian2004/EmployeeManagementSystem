using EMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.Helpers
{
    public class QueryParameterFilter
    {
        private const int MaxPageSize = 100;

        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = value > MaxPageSize
                ? MaxPageSize
                : value <= 0
                    ? 10
                    : value;
        }

        public int Skip => (PageNumber - 1) * PageSize;

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }


    public class CareerFilterRequest : QueryParameterFilter
    {

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? City { get; set; }

        public int? JobPostingId { get; set; }

        public decimal? MinExperience { get; set; }

        public decimal? MaxExperience { get; set; }

        public string? SkillSet { get; set; }

        public DateOnly? FromAppliedDate { get; set; }

        public DateOnly? ToAppliedDate { get; set; }
    }

    public class EmployeeFilterRequest : QueryParameterFilter
    {
        public string? EmpNo { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public int? RoleId{ get; set; }
        public string? MaritalStatus { get; set; }
        
        public string? Phone { get; set; }
    }

    public class JobPostingFilterRequest : QueryParameterFilter
    {
        public string? Search { get; set; }

        public int? DepartmentId { get; set; }

        public EmploymentType? EmploymentType { get; set; }

        public WorkMode? WorkMode { get; set; }

        public string? Status { get; set; }

        public string? Location { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public decimal? MinExperience { get; set; }

        public decimal? MaxExperience { get; set; }

        public DateOnly? PostedFrom { get; set; }

        public DateOnly? PostedTo { get; set; }

        public DateOnly? ClosingFrom { get; set; }

        public DateOnly? ClosingTo { get; set; }
    }
}
