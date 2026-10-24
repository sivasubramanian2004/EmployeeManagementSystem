-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: employeemanagementsystemdb
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `careereducations`
--

DROP TABLE IF EXISTS `careereducations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `careereducations` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CareerId` int NOT NULL,
  `EducationType` varchar(100) NOT NULL,
  `InstitutionName` varchar(200) NOT NULL,
  `Degree` varchar(150) NOT NULL,
  `Percentage` decimal(5,2) NOT NULL,
  `FromDate` date NOT NULL,
  `ToDate` date DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CareerEducations_Careers` (`CareerId`),
  CONSTRAINT `FK_CareerEducations_Careers` FOREIGN KEY (`CareerId`) REFERENCES `careers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `careers`
--

DROP TABLE IF EXISTS `careers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `careers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  `Email` varchar(150) NOT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `Street` varchar(250) DEFAULT NULL,
  `City` varchar(100) NOT NULL,
  `Pincode` varchar(20) NOT NULL,
  `State` varchar(100) DEFAULT NULL,
  `Country` varchar(100) DEFAULT NULL,
  `JobApplicationPosition` varchar(150) NOT NULL,
  `Experience` decimal(5,2) NOT NULL,
  `CurrentDesignation` varchar(150) DEFAULT NULL,
  `CurrentCompany` varchar(150) DEFAULT NULL,
  `CurrentSalary` varchar(50) DEFAULT NULL,
  `ExpectedSalary` decimal(18,2) DEFAULT NULL,
  `NoticePeriod` varchar(50) DEFAULT NULL,
  `SkillSet` text NOT NULL,
  `LinkedInUrl` varchar(500) DEFAULT NULL,
  `GitHubUrl` varchar(500) DEFAULT NULL,
  `PhotoPath` varchar(500) DEFAULT NULL,
  `ResumePath` varchar(500) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `ReferralEmail` varchar(100) DEFAULT NULL,
  `JobPostingId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Careers_JobPosting` (`JobPostingId`),
  CONSTRAINT `FK_Careers_JobPosting` FOREIGN KEY (`JobPostingId`) REFERENCES `jobpostings` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `departments`
--

DROP TABLE IF EXISTS `departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `departments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DepartmentName` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `DepartmentName` (`DepartmentName`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `designations`
--

DROP TABLE IF EXISTS `designations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `designations` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DesignationName` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `employeedocuments`
--

DROP TABLE IF EXISTS `employeedocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employeedocuments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `DocumentType` varchar(50) NOT NULL,
  `DocumentName` varchar(255) NOT NULL,
  `FilePath` varchar(500) NOT NULL,
  `FileSize` int DEFAULT NULL,
  `UploadedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_EmployeeDocuments_EmployeeId` (`EmployeeId`),
  CONSTRAINT `FK_EmployeeDocuments_Employee` FOREIGN KEY (`EmployeeId`) REFERENCES `employees` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `employeepersonaldetails`
--

DROP TABLE IF EXISTS `employeepersonaldetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employeepersonaldetails` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `MaritalStatus` varchar(20) DEFAULT NULL,
  `Nationality` varchar(50) DEFAULT NULL,
  `AadharNumber` varchar(20) DEFAULT NULL,
  `PanNumber` varchar(20) DEFAULT NULL,
  `EmergencyContactName` varchar(150) DEFAULT NULL,
  `EmergencyContactPhone` varchar(20) DEFAULT NULL,
  `EmergencyContactRelation` varchar(50) DEFAULT NULL,
  `BankAccountNumber` varchar(30) DEFAULT NULL,
  `BankName` varchar(100) DEFAULT NULL,
  `IfscCode` varchar(20) DEFAULT NULL,
  `District` varchar(100) DEFAULT NULL,
  `State` varchar(100) DEFAULT NULL,
  `Pincode` varchar(10) DEFAULT NULL,
  `FatherName` varchar(150) DEFAULT NULL,
  `FatherOccupation` varchar(100) DEFAULT NULL,
  `FatherMobileNo` varchar(20) DEFAULT NULL,
  `MotherName` varchar(150) DEFAULT NULL,
  `MotherOccupation` varchar(100) DEFAULT NULL,
  `SiblingCount` int DEFAULT NULL,
  `FamilyIncome` decimal(12,2) DEFAULT NULL,
  `IsFirstGraduate` tinyint(1) DEFAULT NULL,
  `SslcSchoolName` varchar(200) DEFAULT NULL,
  `SslcPercentage` decimal(5,2) DEFAULT NULL,
  `HscOrDiplomaSchoolName` varchar(200) DEFAULT NULL,
  `HscOrDiplomaPercentage` decimal(5,2) DEFAULT NULL,
  `UgCollegeName` varchar(200) DEFAULT NULL,
  `UgCgpa` decimal(4,2) DEFAULT NULL,
  `PgDegreeCollegeName` varchar(200) DEFAULT NULL,
  `PgCgpa` decimal(4,2) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `EmployeeId` (`EmployeeId`),
  CONSTRAINT `FK_EmployeePersonalDetails_Employee` FOREIGN KEY (`EmployeeId`) REFERENCES `employees` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `employees`
--

DROP TABLE IF EXISTS `employees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employees` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `EmpNo` varchar(20) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Age` int NOT NULL,
  `Gender` varchar(10) NOT NULL,
  `DOB` date NOT NULL,
  `Email` varchar(150) NOT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `DateOfJoining` date NOT NULL,
  `BloodGroup` varchar(5) DEFAULT NULL,
  `ProfilePhotoUrl` varchar(500) DEFAULT NULL,
  `DepartmentId` int NOT NULL,
  `DesignationId` int NOT NULL,
  `RoleId` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  `ManagerId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId` (`UserId`),
  UNIQUE KEY `EmpNo` (`EmpNo`),
  KEY `FK_Employees_Department` (`DepartmentId`),
  KEY `FK_Employees_Designation` (`DesignationId`),
  KEY `FK_Employees_Role` (`RoleId`),
  CONSTRAINT `FK_Employees_Department` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`),
  CONSTRAINT `FK_Employees_Designation` FOREIGN KEY (`DesignationId`) REFERENCES `designations` (`Id`),
  CONSTRAINT `FK_Employees_Role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`),
  CONSTRAINT `FK_Employees_User` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `jobpostings`
--

DROP TABLE IF EXISTS `jobpostings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `jobpostings` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(150) NOT NULL,
  `DepartmentId` int DEFAULT NULL,
  `MinSalary` decimal(12,2) DEFAULT NULL,
  `MaxSalary` decimal(12,2) DEFAULT NULL,
  `EmploymentType` varchar(20) NOT NULL,
  `WorkMode` varchar(20) NOT NULL,
  `MinExperience` decimal(4,1) DEFAULT NULL,
  `MaxExperience` decimal(4,1) DEFAULT NULL,
  `Description` text,
  `Requirements` text,
  `Location` varchar(150) DEFAULT NULL,
  `NumberOfOpenings` int NOT NULL DEFAULT '1',
  `Status` varchar(20) NOT NULL DEFAULT 'Open',
  `PostedDate` date NOT NULL,
  `ClosingDate` date DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_JobPostings_Department` (`DepartmentId`),
  CONSTRAINT `FK_JobPostings_Department` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `leaverequests`
--

DROP TABLE IF EXISTS `leaverequests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `leaverequests` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `LeaveTypeId` int NOT NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Reason` varchar(500) DEFAULT NULL,
  `Status` varchar(20) NOT NULL DEFAULT 'Pending',
  `ApprovedBy` int DEFAULT NULL,
  `ActionDate` datetime DEFAULT NULL,
  `RejectionReason` varchar(500) DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_LeaveRequests_Employee` (`EmployeeId`),
  KEY `FK_LeaveRequests_LeaveType` (`LeaveTypeId`),
  CONSTRAINT `FK_LeaveRequests_Employee` FOREIGN KEY (`EmployeeId`) REFERENCES `employees` (`Id`),
  CONSTRAINT `FK_LeaveRequests_LeaveType` FOREIGN KEY (`LeaveTypeId`) REFERENCES `leavetypes` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `leavetypes`
--

DROP TABLE IF EXISTS `leavetypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `leavetypes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `LeaveTypeName` varchar(50) NOT NULL,
  `DefaultDaysPerYear` int NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `LeaveTypeName` (`LeaveTypeName`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(50) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleName` (`RoleName`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `salaries`
--

DROP TABLE IF EXISTS `salaries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `salaries` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `BasicPay` decimal(12,2) NOT NULL,
  `Allowances` decimal(12,2) NOT NULL DEFAULT '0.00',
  `Deductions` decimal(12,2) NOT NULL DEFAULT '0.00',
  `NetPay` decimal(12,2) NOT NULL,
  `EffectiveFrom` date NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `CreatedBy` int DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedBy` int DEFAULT NULL,
  `DeletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Salaries_Employee` (`EmployeeId`),
  CONSTRAINT `FK_Salaries_Employee` FOREIGN KEY (`EmployeeId`) REFERENCES `employees` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` varchar(50) NOT NULL DEFAULT 'Employee',
  `DateOfBirth` date DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedDate` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedDate` datetime DEFAULT NULL,
  `OtpCode` varchar(6) DEFAULT NULL,
  `OtpExpiryTime` datetime DEFAULT NULL,
  `OtpIsUsed` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-24 21:33:00
