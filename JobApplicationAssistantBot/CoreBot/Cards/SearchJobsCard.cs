using AdaptiveCards;
using CoreBot.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Cards
{
    public class SearchJobsCard
    {
        public static async Task<List<Attachment>> CreateCardAttachmentsAsync(JobDataService jobDataService)
        {
            var jobs = await jobDataService.GetAllJobsAsync();
            var attachments = new List<Attachment>();

            foreach (var job in jobs)
            {
                var salaryFormatted = job.Salary.ToString("C");
                var jobTypeString = job.Type.ToString();
                var experienceLevelString = job.RequiredExperience.ToString();
                var skillsList = string.Join(", ", job.RequiredSkills);
                var postedTimeString = job.PostedDate.ToString("d");

                var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Body = new List<AdaptiveElement>
                    {
                        new AdaptiveColumnSet
                        {
                            Columns = new List<AdaptiveColumn>
                            {
                                new AdaptiveColumn
                                {
                                    Width = "stretch",
                                    Items = new List<AdaptiveElement>
                                    {
                                        new AdaptiveTextBlock
                                        {
                                            Text = job.Title,
                                            Weight = AdaptiveTextWeight.Bolder,
                                            Size = AdaptiveTextSize.Medium
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"📝 **Description**: {job.Description}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"💰 **Salary**: {salaryFormatted} per year",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"📍 **Location**: {job.Location}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"💼 **Job Type**: {jobTypeString}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"📚 **Experience**: {experienceLevelString}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"🛠️ **Skills**: {skillsList}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"🕒 Posted: {postedTimeString}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"📌 {(job.IsActive ? "✅ Active" : "❌ Inactive")}",
                                            Wrap = true
                                        },
                                        new AdaptiveTextBlock
                                        {
                                            Text = $"🏢 **Company**: {job.Company?.Name}",
                                            Wrap = true
                                        }
                                    }
                                }
                            }
                        }
                    },
                    
                };

                attachments.Add(new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JObject.FromObject(card)
                });
            }

            return attachments;
        }
    }
}