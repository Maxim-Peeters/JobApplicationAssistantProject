using System;
using System.Collections.Generic;
using DAL.Models;

namespace DAL.Data
{
    public class DBInitializer
    {
        public static void Initialize(JobApplicationContext context)
        {
            context.Database.EnsureCreated();
            if (context.Companies.Any())
            {
                return;
            }

            // Seed Companies
            var companies = new Company[]
            {
                new Company { Name = "Tech Solutions Inc", Description = "Leading tech company", Industry = "Software", Website = "www.techsolutions.com", Location = "Antwerp" },
                new Company { Name = "DataCorp", Description = "Data analytics firm", Industry = "Data Analytics", Website = "www.datacorp.com", Location = "Brussels" },
                new Company { Name = "Innovatech", Description = "Innovative tech solutions", Industry = "Technology", Website = "www.innovatech.com", Location = "Ghent" },
                new Company { Name = "HealthTech", Description = "Healthcare technology", Industry = "Healthcare", Website = "www.healthtech.com", Location = "Leuven" },
                new Company { Name = "EcoEnergy", Description = "Renewable energy solutions", Industry = "Energy", Website = "www.ecoenergy.com", Location = "Bruges" }
            };
            context.Companies.AddRange(companies);
            context.SaveChanges();

            // Seed Jobs
            var jobs = new Job[]
            {
                new Job {
                    Title = "Software Developer",
                    Description = "C# Developer position",
                    Salary = 45000M,
                    Location = "Antwerp",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Junior,
                    CompanyId = companies[0].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "C#", ".NET", "SQL" }
                },
                new Job {
                    Title = "Data Analyst",
                    Description = "Data analysis position",
                    Salary = 40000M,
                    Location = "Brussels",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Entry,
                    CompanyId = companies[1].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Python", "SQL", "Power BI" }
                },
                new Job {
                    Title = "Project Manager",
                    Description = "Manage tech projects",
                    Salary = 60000M,
                    Location = "Ghent",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Senior,
                    CompanyId = companies[2].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Project Management", "Agile", "Scrum" }
                },
                new Job {
                    Title = "Healthcare IT Specialist",
                    Description = "IT support for healthcare",
                    Salary = 50000M,
                    Location = "Leuven",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.MidLevel,
                    CompanyId = companies[3].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Healthcare IT", "Networking", "Security" }
                },
                new Job {
                    Title = "Renewable Energy Engineer",
                    Description = "Engineer for renewable energy projects",
                    Salary = 55000M,
                    Location = "Bruges",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.MidLevel,
                    CompanyId = companies[4].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Renewable Energy", "Engineering", "Project Management" }
                },
                new Job {
                    Title = "Frontend Developer",
                    Description = "React Developer position",
                    Salary = 45000M,
                    Location = "Antwerp",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Junior,
                    CompanyId = companies[0].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "React", "JavaScript", "CSS" }
                },
                new Job {
                    Title = "Backend Developer",
                    Description = "Node.js Developer position",
                    Salary = 50000M,
                    Location = "Brussels",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.MidLevel,
                    CompanyId = companies[1].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Node.js", "Express", "MongoDB" }
                },
                new Job {
                    Title = "DevOps Engineer",
                    Description = "DevOps position",
                    Salary = 60000M,
                    Location = "Ghent",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Senior,
                    CompanyId = companies[2].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "DevOps", "CI/CD", "AWS" }
                },
                new Job {
                    Title = "Cybersecurity Analyst",
                    Description = "Cybersecurity position",
                    Salary = 55000M,
                    Location = "Leuven",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.MidLevel,
                    CompanyId = companies[3].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Cybersecurity", "Penetration Testing", "SIEM" }
                },
                new Job {
                    Title = "Data Scientist",
                    Description = "Data science position",
                    Salary = 65000M,
                    Location = "Bruges",
                    Type = JobType.FullTime,
                    RequiredExperience = ExperienceLevel.Senior,
                    CompanyId = companies[4].Id,
                    PostedDate = DateTime.Now,
                    IsActive = true,
                    RequiredSkills = new List<string> { "Data Science", "Machine Learning", "Python" }
                }
            };
            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }
    }
}

