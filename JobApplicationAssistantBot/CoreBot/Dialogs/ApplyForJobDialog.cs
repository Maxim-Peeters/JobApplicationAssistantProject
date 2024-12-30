using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs.Choices;
using CoreBot.Models;
using CoreBot.DialogDetails;
using CoreBot.Cards;
using static CoreBot.Models.Enums;

namespace CoreBot.Dialogs
{
    public class ApplyForJobDialog : CancelDialog
    {
        private readonly ApplicationDataService _applicationDataservice;
        private readonly JobDataService _jobDataService;

        private const string JobStepMsgText = "Please select the job you want to apply for.";
        private const string ResumeStepMsgText = "Please provide the URL to your resume:";
        private const string CoverLetterStepMsgText = "Please provide the URL to your cover letter:";
        private const string NotesStepMsgText = "Any additional notes or information you'd like to include?";

        public ApplyForJobDialog(
            ApplicationDataService applicationDataservice,
            JobDataService jobDataService)
            : base(nameof(ApplyForJobDialog))
        {
            _applicationDataservice = applicationDataservice;
            _jobDataService = jobDataService;

            // Register prompts
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            // Define Waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                JobStepAsync,
                ResumeStepAsync,
                CoverLetterStepAsync,
                NotesStepAsync,
                ConfirmPromptStepAsync, // prompt user if they want to apply
                ConfirmActStepAsync     // handle user choice
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> JobStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;

            try
            {
                // Get list of jobs
                var availableJobs = await _jobDataService.GetAllJobsAsync();

                if (availableJobs == null || !availableJobs.Any())
                {
                    await stepContext.Context.SendActivityAsync(
                        "Sorry, there are no jobs available at the moment.",
                        cancellationToken: cancellationToken
                    );
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }

                // Create a carousel or other adaptive cards (depending on your method)
                var cardAttachments = JobSelectionCard.CreateJobSelectionCard(availableJobs);
                var promptMessage = (Activity)MessageFactory.Carousel(cardAttachments);
                promptMessage.Text = JobStepMsgText;

                // Prompt the user to select a job. 
                // This uses a TextPrompt, but you can parse the card inputs as needed.
                return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = promptMessage,
                        RetryPrompt = MessageFactory.Text(
                            "Please select a valid job or type the job title."
                        )
                    },
                    cancellationToken
                );
            }
            catch (Exception)
            {
                await stepContext.Context.SendActivityAsync(
                    "Sorry, an error occurred while retrieving the jobs.",
                    cancellationToken: cancellationToken
                );
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ResumeStepAsync(
    WaterfallStepContext stepContext,
    CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;

            // Retrieve user input (could be "3", "Project Manager", etc.)
            var userInput = stepContext.Result?.ToString();
            var allJobs = await _jobDataService.GetAllJobsAsync();
            Job selectedJob = null;

            // 1. Try to parse userInput as a job ID (numeric)
            if (int.TryParse(userInput, out int jobId))
            {
                selectedJob = allJobs.FirstOrDefault(j => j.Id == jobId);
            }

            // 2. If that fails, match by job title
            if (selectedJob == null)
            {
                selectedJob = allJobs.FirstOrDefault(
                    j => j.Title.Equals(userInput, StringComparison.OrdinalIgnoreCase)
                );
            }

            // If a matching job was found, store it in applyForJobDetails
            if (selectedJob != null)
            {
                applyForJobDetails.Job = selectedJob;
            }
            else
            {
                // No matching job found
                await stepContext.Context.SendActivityAsync(
                    "I couldn't find that job. Try again later or pick another job.",
                    cancellationToken: cancellationToken
                );
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            // Prompt for resume URL
            if (string.IsNullOrEmpty(applyForJobDetails.ResumeUrl))
            {
                var promptMessage = MessageFactory.Text(
                    "Please provide the URL to your resume:",
                    "Please provide the URL to your resume:",
                    InputHints.ExpectingInput
                );
                return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions { Prompt = promptMessage },
                    cancellationToken
                );
            }

            return await stepContext.NextAsync(applyForJobDetails.ResumeUrl, cancellationToken);
        }

        private async Task<DialogTurnResult> CoverLetterStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;

            // Store resume URL
            applyForJobDetails.ResumeUrl = stepContext.Result?.ToString();

            // Prompt for cover letter URL
            if (string.IsNullOrEmpty(applyForJobDetails.CoverLetterUrl))
            {
                var promptMessage = MessageFactory.Text(
                    CoverLetterStepMsgText,
                    CoverLetterStepMsgText,
                    InputHints.ExpectingInput
                );
                return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions { Prompt = promptMessage },
                    cancellationToken
                );
            }

            return await stepContext.NextAsync(applyForJobDetails.CoverLetterUrl, cancellationToken);
        }

        private async Task<DialogTurnResult> NotesStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;

            // Store cover letter URL
            applyForJobDetails.CoverLetterUrl = stepContext.Result?.ToString();

            // Prompt for additional notes
            if (string.IsNullOrEmpty(applyForJobDetails.Notes))
            {
                var promptMessage = MessageFactory.Text(
                    NotesStepMsgText,
                    NotesStepMsgText,
                    InputHints.ExpectingInput
                );
                return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions { Prompt = promptMessage },
                    cancellationToken
                );
            }

            return await stepContext.NextAsync(applyForJobDetails.Notes, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmPromptStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;

            // Store notes
            if (stepContext.Result != null)
            {
                applyForJobDetails.Notes = stepContext.Result.ToString();
            }

            // Prompt user to confirm final application submission
            var promptMessage = "Would you like to submit your application now?";
            return await stepContext.PromptAsync(
                nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(promptMessage),
                    Choices = new[]
                    {
                        new Choice("Yes"),
                        new Choice("No")
                    }
                },
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> ConfirmActStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applyForJobDetails = (ApplyForJobDetails)stepContext.Options;
            var userChoice = (stepContext.Result as FoundChoice)?.Value;

            // If user said "Yes"
            if (userChoice != null && userChoice.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                // Example: store the application
                var newApplication = new ApplicationRequest
                {
                    // If your 'Job' object has an 'Id' property, use it here
                    JobId = applyForJobDetails.Job?.Id ?? 0,
                    ResumeUrl = applyForJobDetails.ResumeUrl,
                    CoverLetterUrl = applyForJobDetails.CoverLetterUrl,
                    Notes = applyForJobDetails.Notes,
                    Status = ApplicationStatus.Submitted
                };

                await _applicationDataservice.CreateApplicationAsync(newApplication);
                await stepContext.Context.SendActivityAsync(
                    "You have successfully applied for the job.",
                    cancellationToken: cancellationToken
                );
                return await stepContext.EndDialogAsync(applyForJobDetails, cancellationToken);
            }

            // If "No" or anything else
            await stepContext.Context.SendActivityAsync(
                "Your application has not been submitted.",
                cancellationToken: cancellationToken
            );
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}