using AdaptiveCards;
using CoreBot.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
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
            var elements = new List<AdaptiveElement>();

            // Add title
            elements.Add(new AdaptiveTextBlock
            {
                Text = "Your Job Applications",
                Weight = AdaptiveTextWeight.Bolder,
                Size = AdaptiveTextSize.Large,
                Wrap = true,
                Separator = true
            });

            if (!applications?.Any() ?? true)
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
                // Add applications
                foreach (var application in applications)
                {
                    elements.Add(new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = $"💼 **Job: {(application.Job?.Title ?? $"ID: {application.Job.Title}")}**",
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
                                Color = GetStatusColor(application.Status),
                                Wrap = true
                            },
                            // Display notes if available
                            new AdaptiveTextBlock
                            {
                                Text = !string.IsNullOrWhiteSpace(application.Notes) ? $"📝 Notes: {application.Notes}" : "📝 No notes available.",
                                Wrap = true,
                                IsSubtle = true
                            },
                            // Separator
                            new AdaptiveTextBlock
                            {
                                Text = "─────────────────",
                                Separator = true,
                                Wrap = true
                            }
                        }
                    });
                }
            }

            card.Body = elements;

            // Add actions
            card.Actions = new List<AdaptiveAction>
            {
                new AdaptiveSubmitAction
                {
                    Title = "Refresh Status",
                    Data = new { action = "refreshStatus" }
                },
                new AdaptiveSubmitAction
                {
                    Title = "View Details",
                    Data = new { action = "viewDetails" }
                },
                new AdaptiveSubmitAction
                {
                    Title = "Done",
                    Data = new { action = "done", text = "done" }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private static AdaptiveTextColor GetStatusColor(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.UnderReview => AdaptiveTextColor.Warning,
                ApplicationStatus.InterviewScheduled => AdaptiveTextColor.Good,
                ApplicationStatus.Rejected => AdaptiveTextColor.Attention,
                ApplicationStatus.Accepted => AdaptiveTextColor.Good,
                _ => AdaptiveTextColor.Default
            };
        }
    }
}