using DAL.Models;
using DAL.Data;

namespace DAL.Repositories
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
