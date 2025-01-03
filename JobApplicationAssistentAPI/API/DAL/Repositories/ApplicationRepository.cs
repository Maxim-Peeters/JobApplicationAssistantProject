using API.DAL.Models;
using API.DAL.Data;

namespace API.DAL.Repositories
{
    public class ApplicationRepository : GenericRepository<Application>
    {
        public readonly JobApplicationContext _context;
        public ApplicationRepository(JobApplicationContext context) : base(context)
        {
            _context = context;
        }
    }
}
