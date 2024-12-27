using CoreBot.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CoreBot.Models
{
    public class CompanyDataService
    {
        private readonly IApiService _apiService;
        public CompanyDataService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _apiService.GetAllAsync<List<Company>>("Companies");
        }
        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _apiService.GetByIdAsync<Company>("Companies", id);
        }

    }
}
