using CoreBot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CoreBot.Models
{
    public class CompanyDataService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CompanyDataService> _logger;

        public CompanyDataService(IApiService apiService, ILogger<CompanyDataService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
               => await _apiService.GetAllAsync<List<Company>>("Companies");
        public async Task<Company> GetCompanyByIdAsync(int id)
                => await _apiService.GetByIdAsync<Company>("Companies", id);
    }
}
