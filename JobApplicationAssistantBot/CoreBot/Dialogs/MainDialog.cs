using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using CoreBot.CognitiveModels;
using CoreBot.Models;
using CoreBot.Cards;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly JobApplicationAssistantBotCLURecognizer _recognizer;
        private readonly ILogger _logger;

        public MainDialog(
            JobApplicationAssistantBotCLURecognizer recognizer,
            SearchJobsDialog searchJobsDialog,
            //ApplyForJobDialog applyForJobDialog,
            CheckApplicationStatusDialog checkStatusDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _recognizer = recognizer;
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(searchJobsDialog);
            // AddDialog(applyForJobDialog);
            AddDialog(checkStatusDialog);

            var waterfallSteps = new WaterfallStep[]
            {
                FirstActionStepAsync,
                ActStepAsync,
                FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> FirstActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_recognizer.IsConfigured)
            {
                throw new InvalidOperationException("Error: Recognizer not configured properly.");
            }

            var messageText = stepContext.Options?.ToString() ??
                            "How can I assist you with job applications today?\nYou can:\n" +
                             "• Search for jobs\n" +
                            "• Apply for a position\n" +
                            "• View your applications"; 
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput) }, cancellationToken);
        }
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                // Get the intent analysis from CLU
                var result = await _recognizer.RecognizeAsync<JobApplicationAssistantBotModel>(stepContext.Context, cancellationToken);
                var topIntent = result.GetTopIntent().intent;
                _logger.LogInformation($"Top intent detected: {topIntent}");
                // Handle different intents based on the recognition result
                switch (result.GetTopIntent().intent)
                {
                    case JobApplicationAssistantBotModel.Intent.SearchJobs:
                        return await stepContext.BeginDialogAsync(
                            nameof(SearchJobsDialog),
                            null,
                            cancellationToken);

                    case JobApplicationAssistantBotModel.Intent.CheckApplicationStatus:
                        return await stepContext.BeginDialogAsync(
                                nameof(CheckApplicationStatusDialog),
                                null,
                                cancellationToken);

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

                            "How can I assist you with job applications today?\nYou can:\n" +
                             "• Search for jobs\n" +
                            "• Apply for a position\n" +
                            "• View your applications";
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

                           "How can I assist you with job applications today?\nYou can:\n" +
                            "• Search for jobs\n" +
                           "• Apply for a position\n" +
                           "• View your applications";

            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
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
