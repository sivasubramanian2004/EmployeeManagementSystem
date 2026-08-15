using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.DTOs.Documents;
using EMS.Core.Enums;
namespace EMS.Core.DTOs.Employees
{
   
        public class CreateEmployeeDto
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Employee number is required.")]
            [StringLength(20, MinimumLength = 3, ErrorMessage = "Employee number must be between 3 and 20 characters.")]
            public string EmpNo { get; set; } = string.Empty;

            [Required(ErrorMessage = "Name is required.")]
            [StringLength(150, MinimumLength = 2)]
            public string Name { get; set; } = string.Empty;

            [Range(18, 65, ErrorMessage = "Age must be between 18 and 65.")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Gender is required.")]
            public Gender Gender { get; set; }

            [Required(ErrorMessage = "Date of birth is required.")]
            public DateOnly DOB { get; set; }

            [Phone(ErrorMessage = "Invalid phone number.")]
            public string? Phone { get; set; }

            [StringLength(500)]
            public string? Address { get; set; }

            [Required(ErrorMessage = "Date of joining is required.")]
            public DateOnly DateOfJoining { get; set; }

            public BloodGroup? BloodGroup { get; set; } 

            [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
            public int DepartmentId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Valid DesignationId is required.")]
            public int DesignationId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Valid RoleId is required.")]
            public int RoleId { get; set; }

            [Required(ErrorMessage = "Manager is required.")]
            public int? ManagerId { get; set; }
            public PersonalDetailsDto? PersonalDetails { get; set; }

           
        }

    }
