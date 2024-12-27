using CoreBot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class JobDataService
    {
        private readonly IApiService _apiService;
        private readonly CompanyDataService _companyDataService;
        private readonly ILogger<JobDataService> _logger;

        public JobDataService(
            IApiService apiService,
            CompanyDataService companyDataService,
            ILogger<JobDataService> logger)
        {
            _apiService = apiService;
            _companyDataService = companyDataService;
            _logger = logger;
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            var jobs = await _apiService.GetAllAsync<List<Job>>("Jobs");
            if (!jobs?.Any() ?? true) return new List<Job>();

            var companies = await _companyDataService.GetAllCompaniesAsync();
            var companyDict = companies?.ToDictionary(c => c.Id) ?? new Dictionary<int, Company>();

            foreach (var job in jobs)
            {
                companyDict.TryGetValue(job.CompanyId, out var company);
                job.Company = company;
            }
            return jobs;
        }

        public async Task<Job> GetJobByIdAsync(int id)
            => await _apiService.GetByIdAsync<Job>("Jobs", id);
    }
}

