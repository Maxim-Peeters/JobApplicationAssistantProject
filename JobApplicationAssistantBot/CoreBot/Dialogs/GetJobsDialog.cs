using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using CoreBot.Models;
using CoreBot.Cards;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CoreBot.Dialogs
{
    public class GetJobsDialog : ComponentDialog
    {
        private readonly JobDataService _jobDataService;
        private readonly ILogger<GetJobsDialog> _logger;

        public GetJobsDialog(
            JobDataService jobDataService,
            ILogger<GetJobsDialog> logger)
            : base(nameof(GetJobsDialog))
        {
            _jobDataService = jobDataService;
            _logger = logger;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
            ShowJobsStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowJobsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting to fetch jobs...");
                var jobs = await _jobDataService.GetAllJobsAsync();

                if (jobs == null || !jobs.Any())
                {
                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text("No jobs are currently available."),
                        cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }

                var attachments = await GetJobsCard.GetJobsAttachmentsAsync(_jobDataService);
                var reply = MessageFactory.Carousel(attachments);
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                _logger.LogInformation($"Successfully sent carousel with {attachments.Count} job cards");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowJobsStepAsync: {Message}", ex.Message);
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text($"An error occurred: {ex.Message}"),
                    cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}