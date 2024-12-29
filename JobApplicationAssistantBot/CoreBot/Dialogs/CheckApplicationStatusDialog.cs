using CoreBot.Cards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class CheckApplicationStatusDialog : ComponentDialog
    {
        private readonly ApplicationDataService _applicationDataService;

        public CheckApplicationStatusDialog(ApplicationDataService applicationDataService)
            : base(nameof(CheckApplicationStatusDialog))
        {
            _applicationDataService = applicationDataService;

            var waterfallSteps = new WaterfallStep[]
            {
                ShowApplicationsStepAsync,
                HandleSelectionStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowApplicationsStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Fetch the list of applications from the data service
            var applications = await _applicationDataService.GetAllApplicationsAsync();

            // Convert List<Application> to new List<Application> while preserving Job information
            var jobApplications = applications.Select(a => new Application
            {
                Id = a.Id,
                ApplicationDate = a.ApplicationDate,
                Status = a.Status,
                ResumeUrl = a.ResumeUrl,
                CoverLetterUrl = a.CoverLetterUrl,
                Notes = a.Notes,
                JobId = a.JobId,
                Job = a.Job // Include the Job property
            }).ToList();

            var cardAttachment = ApplicationStatusCard.CreateApplicationStatusCard(jobApplications);
            var response = MessageFactory.Attachment(cardAttachment);
            await stepContext.Context.SendActivityAsync(response, cancellationToken);

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(
                        "What would you like to do? You can view details, refresh status, or type 'done' to finish.")
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> HandleSelectionStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Value != null)
            {
                // Handle button clicks
                dynamic value = stepContext.Context.Activity.Value;
                string action = value.action?.ToString().ToLower();

                switch (action)
                {
                    case "refreshstatus":
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);

                    case "viewdetails":
                        await stepContext.Context.SendActivityAsync(
                            "Detailed view coming soon!",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);

                    case "done":
                        return await stepContext.EndDialogAsync(null, cancellationToken);
                }
            }

            // Handle text input
            var selection = stepContext.Result?.ToString().ToLower();
            if (selection == "done")
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(
                "Invalid input. Please choose a valid option.",
                cancellationToken: cancellationToken);

            return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
        }
    }
}