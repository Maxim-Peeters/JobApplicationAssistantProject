using CoreBot.Cards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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
                ShowApplicationsStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowApplicationsStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var applications = await _applicationDataService.GetAllApplicationsAsync();

            var jobApplications = applications.ToList();

            var cardAttachment = ApplicationStatusCard.CreateApplicationStatusCard(jobApplications);
            var response = MessageFactory.Attachment(cardAttachment);
            await stepContext.Context.SendActivityAsync(response, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}