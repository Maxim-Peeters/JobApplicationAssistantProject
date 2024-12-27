using CoreBot.Models;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Cards
{
    public class GetJobsCard
    {
        public static async Task<List<Attachment>> GetJobsAttachmentsAsync(JobDataService jobDataService)
        {
            try
            {
                var jobs = await jobDataService.GetAllJobsAsync();
                var attachments = new List<Attachment>();

                foreach (var job in jobs)
                {
                    // Format job type and experience level more user-friendly
                    var jobTypeString = job.Type.ToString().Replace("_", " ");
                    var experienceLevelString = job.RequiredExperience.ToString().Replace("_", " ");

                    // Format required skills with emojis
                    var skillsList = job.RequiredSkills != null && job.RequiredSkills.Any()
                        ? "🔹 " + string.Join("\n🔹 ", job.RequiredSkills)
                        : "No specific skills required";

                    // Calculate how many days ago the job was posted
                    var daysAgo = (DateTime.UtcNow - job.PostedDate).Days;
                    var postedTimeString = daysAgo == 0 ? "Posted today" :
                                         daysAgo == 1 ? "Posted yesterday" :
                                         $"Posted {daysAgo} days ago";

                    // Format salary with currency and thousands separator
                    var salaryFormatted = job.Salary.ToString("C", new System.Globalization.CultureInfo("en-US"));

                    // Create the card text with Unicode symbols and formatting
                    var cardText = $"📝 **Description**\n{job.Description}\n\n" +
                                 $"💰 **Salary**\n{salaryFormatted} per year\n\n" +
                                 $"📍 **Location**\n{job.Location}\n\n" +
                                 $"💼 **Job Type**\n{jobTypeString}\n\n" +
                                 $"📚 **Required Experience**\n{experienceLevelString}\n\n" +
                                 $"🛠️ **Required Skills**\n{skillsList}\n\n" +
                                 $"🕒 {postedTimeString}\n" +
                                 $"📌 {(job.IsActive ? "✅ Active" : "❌ Inactive")}\n"+
                                 $"\n\n🏢 **Company**\n{job.Company.Name}";                 

                    var jobCard = new HeroCard
                    {
                        Title = $"🚀 {job.Title}",
                        Subtitle = $"📍 {job.Location} | 💼 {jobTypeString}",
                        Text = cardText,
                        Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Title = "Apply Now",
                        Type = ActionTypes.PostBack,
                        Value = $"apply:{job.Id}",
                        Image = "https://raw.githubusercontent.com/microsoft/botframework-sdk/main/icon.png",
                    },
                    new CardAction
                    {
                        Title = "View Details",
                        Type = ActionTypes.PostBack,
                        Value = $"details:{job.Id}",
                        Image = "https://raw.githubusercontent.com/microsoft/botframework-sdk/main/icon.png",
                    }
                }
                    };

                    attachments.Add(jobCard.ToAttachment());
                }

                return attachments;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating job cards: {ex.Message}");
                throw;
            }
        }
    }
}
