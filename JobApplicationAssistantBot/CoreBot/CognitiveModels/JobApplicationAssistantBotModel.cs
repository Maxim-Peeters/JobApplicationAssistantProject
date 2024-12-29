using CoreBot.Models;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static CoreBot.Models.Enums;

namespace CoreBot.CognitiveModels
{
    public class JobApplicationAssistantBotModel : IRecognizerConvert
    {
        public enum Intent
        {
            SearchJobs,
            ApplyForJob,
            CheckApplicationStatus,
            
            None
        }

        public string Text { get; set; }
        public string AlteredText { get; set; }
        public Dictionary<Intent, IntentScore> Intents { get; set; }
        public JobEntities Entities { get; set; }
        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var jsonResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var app = JsonConvert.DeserializeObject<JobApplicationAssistantBotModel>(jsonResult);

            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) GetTopIntent()
        {
            var maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }

            return (maxIntent, max);
        }

        public class JobEntities
        {
            public CluEntity[] Entities { get; set; }

            // Job-related getters
            public string GetJobTitle() =>
                Entities.FirstOrDefault(e => e.Category == "JobTitle")?.Text;

            public string GetJobLocation() =>
                Entities.FirstOrDefault(e => e.Category == "JobLocation")?.Text;

            public JobType? GetJobType()
            {
                var typeText = Entities.FirstOrDefault(e => e.Category == "JobType")?.Text;
                return typeText != null && System.Enum.TryParse<JobType>(typeText, true, out var type)
                    ? type
                    : null;
            }

            public decimal? GetSalary()
            {
                var salaryText = Entities.FirstOrDefault(e => e.Category == "Salary")?.Text;
                return decimal.TryParse(salaryText, out var salary) ? salary : null;
            }

            // Company-related getters
            public string GetCompanyName() =>
                Entities.FirstOrDefault(e => e.Category == "CompanyName")?.Text;

            public string GetIndustry() =>
                Entities.FirstOrDefault(e => e.Category == "Industry")?.Text;

            // Application-related getters
            public ApplicationStatus? GetApplicationStatus()
            {
                var statusText = Entities.FirstOrDefault(e => e.Category == "ApplicationStatus")?.Text;
                return statusText != null && System.Enum.TryParse<ApplicationStatus>(statusText, true, out var status)
                    ? status
                    : null;
            }

            public string GetResumeUrl() =>
                Entities.FirstOrDefault(e => e.Category == "ResumeUrl")?.Text;

            public string GetCoverLetterUrl() =>
                Entities.FirstOrDefault(e => e.Category == "CoverLetterUrl")?.Text;

            // Experience-related getters
            public ExperienceLevel? GetExperienceLevel()
            {
                var levelText = Entities.FirstOrDefault(e => e.Category == "ExperienceLevel")?.Text;
                return levelText != null && System.Enum.TryParse<ExperienceLevel>(levelText, true, out var level)
                    ? level
                    : null;
            }

            public List<string> GetRequiredSkills() =>
                Entities.Where(e => e.Category == "RequiredSkill")
                        .Select(e => e.Text)
                        .ToList();

            // Interview-related getters
            public string GetInterviewDate() =>
                Entities.FirstOrDefault(e => e.Category == "InterviewDate")?.Text;

            public string GetInterviewTime() =>
                Entities.FirstOrDefault(e => e.Category == "InterviewTime")?.Text;
        }

        public class CluEntity
        {
            public string Category { get; set; }
            public string Text { get; set; }
        }

        // Helper methods to convert entities to domain models
        public Job CreateJobModel()
        {
            return new Job
            {
                Title = Entities.GetJobTitle(),
                Location = Entities.GetJobLocation(),
                Type = Entities.GetJobType() ?? JobType.FullTime,
                Salary = Entities.GetSalary() ?? 0,
                RequiredExperience = Entities.GetExperienceLevel() ?? ExperienceLevel.Entry,
                RequiredSkills = Entities.GetRequiredSkills(),
                IsActive = true,
                PostedDate = System.DateTime.UtcNow
            };
        }

        public Application CreateApplicationModel(int jobId)
        {
            return new Application
            {
                JobId = jobId,
                ApplicationDate = System.DateTime.UtcNow,
                Status = Entities.GetApplicationStatus() ?? ApplicationStatus.Submitted,
                ResumeUrl = Entities.GetResumeUrl(),
                CoverLetterUrl = Entities.GetCoverLetterUrl()
            };
        }

        public Company CreateCompanyModel()
        {
            return new Company
            {
                Name = Entities.GetCompanyName(),
                Industry = Entities.GetIndustry(),
                Location = Entities.GetJobLocation()
            };
        }
    }
}