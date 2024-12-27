using CoreBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class JobDataService
    {
        private readonly IApiService _apiService;
        public JobDataService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _apiService.GetAllAsync<List<Job>>("Jobs");
        }
        public async Task<Job> GetJobByIdAsync(int id)
        {
            return await _apiService.GetByIdAsync<Job>("Jobs", id);
        }
    }
}
