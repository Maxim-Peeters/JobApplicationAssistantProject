using CoreBot.Cards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class SearchJobsDialog : ComponentDialog
    {
        private readonly JobDataService _jobDataService;

        public SearchJobsDialog(JobDataService jobDataService) : base(nameof(SearchJobsDialog))
        {
            _jobDataService = jobDataService;

            var waterfallSteps = new WaterfallStep[]
            {
                ShowCoursesStepAsync,
                EndDialogStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowCoursesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cards = await SearchJobsCard.CreateCardAttachmentsAsync(_jobDataService);
            var activity = MessageFactory.Carousel(cards);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
