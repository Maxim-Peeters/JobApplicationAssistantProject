using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class CancelDialog : ComponentDialog
    {
        private const string CancelMsgText = "Cancelling...";

        public CancelDialog(string id)
            : base(id)
        {
            // Ensure this dialog has an empty waterfall step to avoid errors
            AddDialog(new WaterfallDialog(id, new WaterfallStep[] { InitialStepAsync }));
            InitialDialogId = id;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // This is a placeholder step
            return await stepContext.NextAsync(null, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text?.Trim().ToLowerInvariant();

                switch (text)
                {
                    case "cancel":
                    case "quit":
                        var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
                        await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
                        return await innerDc.CancelAllDialogsAsync(cancellationToken);
                }
            }

            return null;
        }
    }
}