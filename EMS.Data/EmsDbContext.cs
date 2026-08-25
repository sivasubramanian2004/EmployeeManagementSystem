using System;
using System.Collections.Generic;
using EMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EMS.Data;

public partial class EmsDbContext : DbContext
{
    public EmsDbContext()
    {
    }

    public EmsDbContext(DbContextOptions<EmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Career> Careers { get; set; }

    public virtual DbSet<Careereducation> Careereducations { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Employeedocument> Employeedocuments { get; set; }

    public virtual DbSet<Employeepersonaldetail> Employeepersonaldetails { get; set; }

    public virtual DbSet<Jobposting> Jobpostings { get; set; }

    public virtual DbSet<Leaverequest> Leaverequests { get; set; }

    public virtual DbSet<Leavetype> Leavetypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Salary> Salaries { get; set; }

    public virtual DbSet<User> Users { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Career>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("careers");

            entity.HasIndex(e => e.JobPostingId, "FK_Careers_JobPosting");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentCompany).HasMaxLength(150);
            entity.Property(e => e.CurrentDesignation).HasMaxLength(150);
            entity.Property(e => e.CurrentSalary).HasMaxLength(50);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.ExpectedSalary).HasPrecision(18, 2);
            entity.Property(e => e.Experience).HasPrecision(5, 2);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.GitHubUrl).HasMaxLength(500);
         //   entity.Property(e => e.JobApplicationPosition).HasMaxLength(150);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LinkedInUrl).HasMaxLength(500);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NoticePeriod).HasMaxLength(50);
            entity.Property(e => e.PhotoPath).HasMaxLength(500);
            entity.Property(e => e.Pincode).HasMaxLength(20);
            entity.Property(e => e.ReferralEmail).HasMaxLength(100);
            entity.Property(e => e.ResumePath).HasMaxLength(500);
            entity.Property(e => e.SkillSet).HasColumnType("text");
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(250);

            entity.HasOne(d => d.JobPosting).WithMany(p => p.Careers)
                .HasForeignKey(d => d.JobPostingId)
                .HasConstraintName("FK_Careers_JobPosting");
        });

        modelBuilder.Entity<Careereducation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("careereducations");

            entity.HasIndex(e => e.CareerId, "FK_CareerEducations_Careers");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Degree).HasMaxLength(150);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.EducationType).HasMaxLength(100);
            entity.Property(e => e.InstitutionName).HasMaxLength(200);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Percentage).HasPrecision(5, 2);

            entity.HasOne(d => d.Career).WithMany(p => p.Careereducations)
                .HasForeignKey(d => d.CareerId)
                .HasConstraintName("FK_CareerEducations_Careers");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments");

            entity.HasIndex(e => e.DepartmentName, "DepartmentName").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("designations");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DesignationName).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employees");

            entity.HasIndex(e => e.EmpNo, "EmpNo").IsUnique();

            entity.HasIndex(e => e.DepartmentId, "FK_Employees_Department");

            entity.HasIndex(e => e.DesignationId, "FK_Employees_Designation");

          //  entity.HasIndex(e => e.RoleId, "FK_Employees_Role");

            entity.HasIndex(e => e.UserId, "UserId").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BloodGroup).HasMaxLength(5);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmpNo).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Department");

            entity.HasOne(d => d.Designation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Designation");

           // entity.HasOne(d => d.Role).WithMany(p => p.Employees)
            //    .HasForeignKey(d => d.RoleId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Employees_Role");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_User");
        });

        modelBuilder.Entity<Employeedocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employeedocuments");

            entity.HasIndex(e => e.EmployeeId, "IX_EmployeeDocuments_EmployeeId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(255);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.Employeedocuments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeDocuments_Employee");
        });

        modelBuilder.Entity<Employeepersonaldetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employeepersonaldetails");

            entity.HasIndex(e => e.EmployeeId, "EmployeeId").IsUnique();

            entity.Property(e => e.AadharNumber).HasMaxLength(20);
            entity.Property(e => e.BankAccountNumber).HasMaxLength(30);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(150);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
            entity.Property(e => e.EmergencyContactRelation).HasMaxLength(50);
            entity.Property(e => e.FamilyIncome).HasPrecision(12, 2);
            entity.Property(e => e.FatherMobileNo).HasMaxLength(20);
            entity.Property(e => e.FatherName).HasMaxLength(150);
            entity.Property(e => e.FatherOccupation).HasMaxLength(100);
            entity.Property(e => e.HscOrDiplomaPercentage).HasPrecision(5, 2);
            entity.Property(e => e.HscOrDiplomaSchoolName).HasMaxLength(200);
            entity.Property(e => e.IfscCode).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.MaritalStatus).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.MotherName).HasMaxLength(150);
            entity.Property(e => e.MotherOccupation).HasMaxLength(100);
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.PanNumber).HasMaxLength(20);
            entity.Property(e => e.PgCgpa).HasPrecision(4, 2);
            entity.Property(e => e.PgDegreeCollegeName).HasMaxLength(200);
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.SslcPercentage).HasPrecision(5, 2);
            entity.Property(e => e.SslcSchoolName).HasMaxLength(200);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.UgCgpa).HasPrecision(4, 2);
            entity.Property(e => e.UgCollegeName).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithOne(p => p.Employeepersonaldetail)
                .HasForeignKey<Employeepersonaldetail>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePersonalDetails_Employee");
        });

        modelBuilder.Entity<Jobposting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("jobpostings");

            entity.HasIndex(e => e.DepartmentId, "FK_JobPostings_Department");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EmploymentType).HasMaxLength(20);
            entity.Property(e => e.Location).HasMaxLength(150);
            entity.Property(e => e.MaxExperience).HasPrecision(4, 1);
            entity.Property(e => e.MaxSalary).HasPrecision(12, 2);
            entity.Property(e => e.MinExperience).HasPrecision(4, 1);
            entity.Property(e => e.MinSalary).HasPrecision(12, 2);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NumberOfOpenings).HasDefaultValueSql("'1'");
            entity.Property(e => e.Requirements).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Open'");
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.WorkMode).HasMaxLength(20);

            entity.HasOne(d => d.Department).WithMany(p => p.Jobpostings)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_JobPostings_Department");
        });

        modelBuilder.Entity<Leaverequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("leaverequests");

            entity.HasIndex(e => e.EmployeeId, "FK_LeaveRequests_Employee");

            entity.HasIndex(e => e.LeaveTypeId, "FK_LeaveRequests_LeaveType");

            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'");

            entity.HasOne(d => d.Employee).WithMany(p => p.Leaverequests)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequests_Employee");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.Leaverequests)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequests_LeaveType");
        });

        modelBuilder.Entity<Leavetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("leavetypes");

            entity.HasIndex(e => e.LeaveTypeName, "LeaveTypeName").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.LeaveTypeName).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "RoleName").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Salary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("salaries");

            entity.HasIndex(e => e.EmployeeId, "FK_Salaries_Employee");

            entity.Property(e => e.Allowances).HasPrecision(12, 2);
            entity.Property(e => e.BasicPay).HasPrecision(12, 2);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Deductions).HasPrecision(12, 2);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NetPay).HasPrecision(12, 2);

            entity.HasOne(d => d.Employee).WithMany(p => p.Salaries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Salaries_Employee");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

           
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.OtpCode).HasMaxLength(6);
            entity.Property(e => e.OtpExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.OtpIsUsed).HasDefaultValueSql("'1'");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Employee'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
