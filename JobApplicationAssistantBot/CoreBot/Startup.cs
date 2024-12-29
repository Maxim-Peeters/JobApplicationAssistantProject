// Simplified Startup.cs

using CoreBot.Bots;
using CoreBot.CognitiveModels;
using CoreBot.Dialogs;
using CoreBot.Models;
using CoreBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreBot
{
    public class Startup
    {
        // Configure services for the bot.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Register the CLU recognizer
            services.AddSingleton<JobApplicationAssistantBotCLURecognizer>();
            // Create the bot services (CLU, etc.) as a singleton.
            services.AddSingleton<JobApplicationAssistantBotModel>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage for User and Conversation state.
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state.
            services.AddSingleton<UserState>();

            // Create the Conversation state.
            services.AddSingleton<ConversationState>();

            // Create the dataservices
            services.AddSingleton<ApplicationDataService>();
            services.AddSingleton<JobDataService>();
            services.AddSingleton<CompanyDataService>();
            // Create the ApiService.
            services.AddSingleton<IApiService, ApiService>();

            // Register GetJobsDialog.
            services.AddSingleton<SearchJobsDialog>();
            services.AddSingleton<CheckApplicationStatusDialog>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();


            // Create the bot as a transient.
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
        }

        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
