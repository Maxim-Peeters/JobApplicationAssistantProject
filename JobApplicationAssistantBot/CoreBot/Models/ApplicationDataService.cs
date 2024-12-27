using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Services;

namespace CoreBot.Models
{
    
    public class ApplicationDataService
    {
        private readonly IApiService _apiService;

        public ApplicationDataService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _apiService.GetAllAsync<List<Application>>("Applications");
        }
        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            return await _apiService.GetByIdAsync<Application>("Applications", id);
        }
        
    }
}