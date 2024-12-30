using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using CoreBot.CognitiveModels;
using CoreBot.Models;
using Newtonsoft.Json.Linq;
using CoreBot.DialogDetails;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;
        private readonly JobApplicationAssistantBotCLURecognizer _recognizer;

        public MainDialog(
            JobApplicationAssistantBotCLURecognizer recognizer,
            SearchJobsDialog searchJobsDialog,
            ApplyForJobDialog applyForJobDialog,
            CheckApplicationStatusDialog checkStatusDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;
            _recognizer = recognizer;

            // Register prompt(s) and sub-dialogs
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(searchJobsDialog);
            AddDialog(applyForJobDialog);
            AddDialog(checkStatusDialog);

            // Waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                FirstActionStepAsync,
                ActionActStepAsync,
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

            // Provide user with initial prompt
            var messageText = stepContext.Options?.ToString() ??
                "How can I assist you with job applications today?\n" +
                "• Search for jobs\n" +
                "• Apply for a position\n" +
                "• View your applications";

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput)
                },
                cancellationToken
            );
        }

        private async Task<DialogTurnResult> ActionActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _recognizer.RecognizeAsync<JobApplicationAssistantBotModel>(stepContext.Context, cancellationToken);
            var topIntent = result.GetTopIntent().intent;
            _logger.LogInformation($"Top intent detected: {topIntent}");

            switch (topIntent)
            {
                case JobApplicationAssistantBotModel.Intent.SearchJobs:
                    _logger.LogInformation("Starting SearchJobs dialog from intent");
                    return await stepContext.BeginDialogAsync(nameof(SearchJobsDialog), null, cancellationToken);

                case JobApplicationAssistantBotModel.Intent.CheckApplicationStatus:
                    _logger.LogInformation("Starting CheckApplicationStatus dialog from intent");
                    return await stepContext.BeginDialogAsync(nameof(CheckApplicationStatusDialog), null, cancellationToken);

                case JobApplicationAssistantBotModel.Intent.ApplyForJob:
                    _logger.LogInformation("Starting ApplyForJob dialog from intent");

                    // Pass in an ApplyForJobDetails object if you need to store user input
                    var jobDetails = new ApplyForJobDetails();
                    return await stepContext.BeginDialogAsync(nameof(ApplyForJobDialog), jobDetails, cancellationToken);

                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sorry, I didn't understand that."), cancellationToken);
                    return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "What else can I help you with?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}