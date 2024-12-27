using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger _logger;

        public MainDialog(
            GetJobsDialog getJobsDialog,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(getJobsDialog);

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
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nYou can say things like:\n• Show me my job applications";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Context.Activity.Text.ToLowerInvariant();

            if (userInput.Contains("job") || userInput.Contains("application") || userInput.Contains("career"))
            {
                _logger.LogInformation("Starting GetJobs dialog");
                return await stepContext.BeginDialogAsync(nameof(GetJobsDialog), null, cancellationToken);
            }

            var didntUnderstandMessageText = "Sorry, I didn't get that. Please try asking about your job applications.";
            var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "What else can I do for you? You can ask about:\n• Your job applications";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
