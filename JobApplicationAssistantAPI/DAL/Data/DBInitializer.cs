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
                new Company { Name = "DataCorp", Description = "Data analytics firm", Industry = "Data Analytics", Website = "www.datacorp.com", Location = "Brussels" }
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
                }
            };
            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }

    }
}

