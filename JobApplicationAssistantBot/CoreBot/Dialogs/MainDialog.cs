using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using CoreBot.CognitiveModels;
using CoreBot.Models;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly JobApplicationAssistantBotCLURecognizer _recognizer;
        private readonly ILogger _logger;

        public MainDialog(
            JobApplicationAssistantBotCLURecognizer recognizer,
            GetJobsDialog getJobsDialog,
            // SearchJobsDialog searchJobsDialog,
            // ApplyForJobDialog applyForJobDialog,
            // CheckStatusDialog checkStatusDialog,
            // ScheduleInterviewDialog scheduleInterviewDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _recognizer = recognizer;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(getJobsDialog);
            // AddDialog(searchJobsDialog);
            // AddDialog(applyForJobDialog);
            // AddDialog(checkStatusDialog);
            // AddDialog(scheduleInterviewDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ??
                "How can I assist you with job applications today?\nYou can:\n" +
                // "• Search for jobs\n" +
                // "• Apply for a position\n" +
                // "• Check application status\n" +
                // "• Schedule interviews\n" +
                "• View your applications";

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                // Get the intent analysis from CLU
                var result = await _recognizer.RecognizeAsync<JobApplicationAssistantBotModel>(stepContext.Context, cancellationToken);



                // Handle different intents based on the recognition result
                switch (result.GetTopIntent().intent)
                {
                    // case JobApplicationAssistantBotModel.Intent.SearchJobs:
                    //     // Extract relevant entities
                    //     var jobTitle = luisResult.Entities.GetJobTitle();
                    //     var location = luisResult.Entities.GetJobLocation();
                    //     var jobType = luisResult.Entities.GetJobType();

                    //     return await stepContext.BeginDialogAsync(
                    //         nameof(SearchJobsDialog),
                    //         new { jobTitle, location, jobType },
                    //         cancellationToken);

                    // case JobApplicationAssistantBotModel.Intent.ApplyForJob:
                    //     // Extract application-related entities
                    //     var resumeUrl = luisResult.Entities.GetResumeUrl();
                    //     var coverLetterUrl = luisResult.Entities.GetCoverLetterUrl();

                    //     return await stepContext.BeginDialogAsync(
                    //         nameof(ApplyForJobDialog),
                    //         new { resumeUrl, coverLetterUrl },
                    //         cancellationToken);

                    // case JobApplicationAssistantBotModel.Intent.CheckApplicationStatus:
                    //     return await stepContext.BeginDialogAsync(
                    //         nameof(CheckStatusDialog),
                    //         null,
                    //         cancellationToken);

                    // case JobApplicationAssistantBotModel.Intent.ScheduleInterview:
                    //     // Extract interview-related entities
                    //     var interviewDate = luisResult.Entities.GetInterviewDate();
                    //     var interviewTime = luisResult.Entities.GetInterviewTime();

                    //     return await stepContext.BeginDialogAsync(
                    //         nameof(ScheduleInterviewDialog),
                    //         new { interviewDate, interviewTime },
                    //         cancellationToken);

                    // case JobApplicationAssistantBotModel.Intent.GetCompanyInfo:
                    //     var companyName = luisResult.Entities.GetCompanyName();
                    //     // Handle company information retrieval
                    //     await HandleCompanyInfoRequest(stepContext, companyName, cancellationToken);
                    //     return await stepContext.NextAsync(null, cancellationToken);

                    default:
                        // Handle the case when no intent matches or score is too low
                        
                            var didntUnderstandMessageText =
                                "I'm not sure I understood that. Could you please rephrase your request? " +
                                "You can view your applications.";
                            var didntUnderstandMessage = MessageFactory.Text(
                                didntUnderstandMessageText,
                                didntUnderstandMessageText,
                                InputHints.ExpectingInput);
                            await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                            return await stepContext.NextAsync(null, cancellationToken);
                        
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing dialog step");
                return await HandleDialogExceptionAsync(stepContext, ex, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage =
                "Is there anything else you'd like to know about?\nYou can:\n" +
                // "• Search for jobs\n" +
                // "• Apply for a position\n" +
                // "• Check application status\n" +
                // "• Schedule interviews\n" +
                "• View your applications";

            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

        private async Task HandleCompanyInfoRequest(WaterfallStepContext stepContext, string companyName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("Could you please specify which company you'd like to know about?"),
                    cancellationToken);
                return;
            }

            // Here you would typically query your database or service for company information
            // For now, we'll just send a placeholder message
            var message = $"Here's what I know about {companyName}. [Company information would be displayed here]";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
        }

        private async Task<DialogTurnResult> HandleDialogExceptionAsync(WaterfallStepContext stepContext, Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError(ex, "Error in dialog");

            var errorMessageText =
                "I apologize, but I'm having trouble processing your request. " +
                "Please try again or rephrase your question.";

            var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
            await stepContext.Context.SendActivityAsync(errorMessage, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}