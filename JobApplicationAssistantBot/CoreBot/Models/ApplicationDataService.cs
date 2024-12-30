using CoreBot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class ApplicationDataService
    {
        private readonly IApiService _apiService;
        private readonly JobDataService _jobDataService;
        private readonly ILogger<ApplicationDataService> _logger;

        public ApplicationDataService(
            IApiService apiService,
            JobDataService jobDataService,
            ILogger<ApplicationDataService> logger)
        {
            _apiService = apiService;
            _jobDataService = jobDataService;
            _logger = logger;
        }

        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            var applications = await _apiService.GetAllAsync<List<Application>>("Applications");
            if (!applications?.Any() ?? true)
                return new List<Application>();

            var jobs = await _jobDataService.GetAllJobsAsync();
            var jobDict = jobs?.ToDictionary(j => j.Id) ?? new Dictionary<int, Job>();

            foreach (var application in applications)
            {
                jobDict.TryGetValue(application.JobId, out var job);
                application.Job = job;
            }

            return applications;
        }

        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            var application = await _apiService.GetByIdAsync<Application>("Applications", id);
            if (application == null)
                return null;

            var job = await _jobDataService.GetJobByIdAsync(application.JobId);
            application.Job = job;

            return application;
        }

        public async Task<Application> CreateApplicationAsync(ApplicationRequest application)
        {
            // Map Application to ApplicationRequest
            var applicationRequest = new ApplicationRequest
            {
                Status = Enums.ApplicationStatus.Submitted,
                ResumeUrl = application.ResumeUrl,
                CoverLetterUrl = application.CoverLetterUrl,
                Notes = application.Notes,
                JobId = application.JobId
            };

            // Send the POST request
            var newApplication = await _apiService.PostAsync<Application, ApplicationRequest>("Applications", applicationRequest);

            if (newApplication == null)
                return null;

            // Fetch the job details for the newly created application
            var job = await _jobDataService.GetJobByIdAsync(newApplication.JobId);
            newApplication.Job = job;

            return newApplication;
        }

    }
}