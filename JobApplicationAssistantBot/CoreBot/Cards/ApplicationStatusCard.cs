using AdaptiveCards;
using CoreBot.Models;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;
using static CoreBot.Models.Enums;

namespace CoreBot.Cards
{
    public class ApplicationStatusCard
    {
        public static Attachment CreateApplicationStatusCard(List<Application> applications)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2));
            var elements = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = "Your Job Applications",
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Large,
                    Wrap = true,
                    Separator = true
                }
            };

            if (!applications.Any())
            {
                elements.Add(new AdaptiveTextBlock
                {
                    Text = "No applications found.",
                    Wrap = true,
                    IsSubtle = true
                });
            }
            else
            {
                elements.AddRange(applications.Select(application => new AdaptiveContainer
                {
                    Items = new List<AdaptiveElement>
                    {
                        new AdaptiveTextBlock
                        {
                            Text = $"💼 **Job: {(application.Job?.Title ?? $"ID: {application.JobId}")}**",
                            Weight = AdaptiveTextWeight.Bolder,
                            Size = AdaptiveTextSize.Medium,
                            Wrap = true
                        },
                        new AdaptiveTextBlock
                        {
                            Text = $"📅 Applied on: {application.ApplicationDate:MMM dd, yyyy}",
                            Wrap = true,
                            IsSubtle = true
                        },
                        new AdaptiveTextBlock
                        {
                            Text = $"📌 **Status**: {application.Status}",
                            Wrap = true
                        },
                        new AdaptiveTextBlock
                        {
                            Text = !string.IsNullOrWhiteSpace(application.Notes) ? $"📝 Notes: {application.Notes}" : "📝 No notes available.",
                            Wrap = true,
                            IsSubtle = true
                        },
                        new AdaptiveTextBlock
                        {
                            Text = "─────────────────",
                            Separator = true,
                            Wrap = true
                        }
                    }
                }));
            }

            card.Body = elements;

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }
    }
}