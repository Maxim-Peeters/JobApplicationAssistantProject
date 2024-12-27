using CoreBot.Models;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Cards
{
    public class GetJobsCard
    {
        public static async Task<List<Attachment>> GetJobsAttachmentsAsync(JobDataService jobDataService, CompanyDataService companyDataService)
        {
            // Fetch job and company data
            var jobs = await jobDataService.GetAllJobsAsync();
            var companies = await companyDataService.GetAllCompaniesAsync();
            var attachments = new List<Attachment>();

            // Create a dictionary for quick company lookups
            var companyDict = companies?.ToDictionary(c => c.Id) ?? new Dictionary<int, Company>();

            // Generate HeroCards for each job
            foreach (var job in jobs)
            {
                // Try to get the company info
                companyDict.TryGetValue(job.CompanyId, out var company);
                var companyName = company?.Name ?? "Company information unavailable";
                var companyWebsite = company?.Website ?? "#";

                var jobCard = new HeroCard
                {
                    Title = job.Title,
                    Subtitle = $"{companyName} - {job.Location}",
                    Text = $"Description: {job.Description}\n" +
                          $"Salary: {job.Salary:C}\n" +
                          $"Required Experience: {job.RequiredExperience}\n" +
                          $"Required Skills: {string.Join(", ", job.RequiredSkills ?? new List<string>())}\n" +
                          $"Posted Date: {job.PostedDate:d}",
                    Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Apply", value: companyWebsite),
                    new CardAction(ActionTypes.OpenUrl, "View Details", value: companyWebsite)
                }
                };

                // Add HeroCard as an attachment
                attachments.Add(jobCard.ToAttachment());
            }

            return attachments;
        }
    }
}
