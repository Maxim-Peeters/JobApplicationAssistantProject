using Microsoft.Bot.Schema;
using CoreBot.Models;
using System.Collections.Generic;

namespace CoreBot.Cards
{
    public static class JobSelectionCard
    {
        // Create a carousel of Hero cards for jobs
        public static Attachment[] CreateJobSelectionCard(List<Job> jobs)
        {
            var attachments = new List<Attachment>();

            foreach (var job in jobs)
            {
                var card = new HeroCard
                {
                    Title = job.Title,
                    Subtitle = job.Description,
                    Text = $"Location: {job.Location}, Company: {job.Company?.Name}",
                    Buttons = new List<CardAction>
                    {
                        new CardAction
                        {
                            Type = ActionTypes.ImBack,
                            Title = $"Apply for {job.Title}",
                            Value = job.Id.ToString()  // Send the JobId as the value when the user selects the job
                        }
                    }
                };

                attachments.Add(card.ToAttachment());
            }

            return attachments.ToArray();
        }
    }
}